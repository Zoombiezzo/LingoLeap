using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants;
using _Client.Scripts.Infrastructure.Services.TimeService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public class StorageService : IStorageService
    {
        private static readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private static readonly byte[] _iv = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        private static Dictionary<Type, IStorableData> _storage = new Dictionary<Type, IStorableData>();
        private static Dictionary<Type, IStorageVariantService> _storageVariants = new Dictionary<Type, IStorageVariantService>();
        private static Dictionary<Type, IStorageVariantService> _secondaryStorageVariants = new Dictionary<Type, IStorageVariantService>();

        private static Dictionary<Type, Dictionary<int, Dictionary<int, IMigration>>> _migration =
            new Dictionary<Type, Dictionary<int, Dictionary<int, IMigration>>>();

        private static Dictionary<Type, Dictionary<int, HashSet<int>>> _migrationVariants =
            new Dictionary<Type, Dictionary<int, HashSet<int>>>();
        
        private string DataPath => Application.persistentDataPath;

        private readonly bool _encrypt;
        private readonly string _password;
        private readonly ITimeService _timeService;
        private readonly StorageInfo _storageInfo;

        private IStorageVariantService _preferredStorage;

        public StorageInfo CurrentStorageInfo => _storageInfo;

        public StorageService(int version, ITimeService timeService, string password, bool encrypt = true)
        {
            _timeService = timeService;
            _password = password;
            _encrypt = encrypt;
            
            _storageInfo = new StorageInfo(version, 0);
        }

        public async Task Initialize()
        {
            if (_preferredStorage == null)
                return;

            var result = await TryLoadInfo(_preferredStorage);
            
            if(result.Item1 == false)
                return;

            var info = result.Item2;
            
            _storageInfo.SaveTime = info.SaveTime;
            _storageInfo.Version = info.Version;
        }

        public void Register<T>(IStorableData data) where T : IStorable
        {
            var type = typeof(T);

            if (_storage.ContainsKey(type))
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} already registered");
#endif
                return;
            }

            _storage.Add(type, data);

#if UNITY_EDITOR
            Debugger.Log($"SaveService: {type.Name} registered");
#endif
        }

        public IStorableData Get<T>() where T : IStorable =>
            _storage.TryGetValue(typeof(T), out var data) == false ? null : data;

        public void RegisterMigrate<T>(IMigration migration) where T : IStorable
        {
            var type = typeof(T);
            if (_migration.TryGetValue(type, out var migrationFromTo) == false)
            {
                migrationFromTo = new Dictionary<int, Dictionary<int, IMigration>>();
                _migration.Add(type, migrationFromTo);
            }

            if (migrationFromTo.TryGetValue(migration.From.Version, out var migrationTo) == false)
            {
                migrationTo = new Dictionary<int, IMigration>();
                migrationFromTo.Add(migration.From.Version, migrationTo);
            }

            if (migrationTo.TryGetValue(migration.To.Version, out var migrationData))
            {
#if UNITY_EDITOR
                Debugger.Log(
                    $"SaveService: {type.Name} already registered migration from {migration.From.Version} to {migration.To.Version}");
#endif
                return;
            }

            if (_migrationVariants.TryGetValue(type, out var migrationVariants) == false)
            {
                migrationVariants = new Dictionary<int, HashSet<int>>();
                _migrationVariants.Add(type, migrationVariants);
            }

            if (migrationVariants.TryGetValue(migration.From.Version, out var variants) == false)
            {
                variants = new HashSet<int>();
                migrationVariants.Add(migration.From.Version, variants);
            }

            variants.Add(migration.To.Version);
            migrationTo.Add(migration.To.Version, migration);
        }

        public string Migrate<T>(string data, int fromVersion, int toVersion) where T : IStorable
        {
            var type = typeof(T);
            if (_migration.TryGetValue(type, out var migrationFromTo) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} not registered migration");
#endif
                return null;
            }

            if (migrationFromTo.TryGetValue(fromVersion, out var migrationTo) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} not registered migration from {fromVersion}");
#endif
                return null;
            }

            if (migrationTo.TryGetValue(toVersion, out var migration) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} not registered migration from {fromVersion} to {toVersion}");

                Debugger.Log("SaveService: Trying to find migration path");
#endif

                if (TryGetMigrationPath<T>(fromVersion, toVersion, out var path) == false)
                {
#if UNITY_EDITOR
                    Debugger.Log($"SaveService: Migration path not found");
#endif

                    return null;
                }

                path.RemoveAt(0);

                foreach (var version in path)
                {
                    data = Migrate<T>(data, fromVersion, version);
                    fromVersion = version;
                }

                return data;
            }

            return migration.Migrate(data);
        }

        public bool TryGetMigrationPath<T>(int fromVersion, int toVersion, out List<int> path) where T : IStorable
        {
            path = new List<int>() { fromVersion };
            var type = typeof(T);
            if (_migrationVariants.TryGetValue(type, out var migrationFromTo) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} not registered migration");
#endif
                return false;
            }

            if (migrationFromTo.TryGetValue(fromVersion, out var variants) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {type.Name} not registered migration from {fromVersion}");
#endif
                return false;
            }

            if (variants.Contains(toVersion))
            {
                path.Add(toVersion);
                return true;
            }

            foreach (var variant in variants)
            {
                if (TryGetMigrationPath<T>(variant, toVersion, out var variantPath))
                {
                    path.AddRange(variantPath);
                    return true;
                }
            }

#if UNITY_EDITOR
            Debugger.Log($"SaveService: {type.Name} path not found from {fromVersion} to {toVersion}");
#endif

            return false;
        }

        public void RegisterStorage<T>(IStorageVariantService storage) where T : IStorageVariantService
        {
            var type = typeof(T);

            if (_storageVariants.ContainsKey(type))
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Storage {type.Name} already registered");
#endif
                return;
            }

            _storageVariants.Add(type, storage);
            _secondaryStorageVariants.TryAdd(type, storage);

            if (_storageVariants.Count == 1 && _preferredStorage == null)
            {
                SetPreferredStorage(storage);
            }

#if UNITY_EDITOR
            Debugger.Log($"SaveService: Storage {type.Name} registered");
#endif
        }

        public void UnregisterStorage<T>() where T : IStorageVariantService
        {
            var type = typeof(T);
            
            if (_storageVariants.ContainsKey(type) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Storage {type.Name} not registered");
#endif
                return;
            }
            
            _storageVariants.Remove(type);
            
            var preferredStorage = _preferredStorage;
            
            if (preferredStorage != null && preferredStorage.GetType() == type)
            {
                _preferredStorage = null;

                foreach (var (_, storage) in _storageVariants)
                {
                    if(storage != null)
                    {
                        SetPreferredStorage(storage);
                        break;
                    }
                }
            }
            
            if (_secondaryStorageVariants.ContainsKey(type))
            {
                _secondaryStorageVariants.Remove(type);
            }
        }

        public void SetPreferredStorage<T>() where T : IStorageVariantService
        {
            var type = typeof(T);

            if (_storageVariants.TryGetValue(type, out var storage) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Storage {type.Name} not registered");
#endif
                return;
            }

            SetPreferredStorage(storage);
        }

        public void SetPreferredStorage(IStorageVariantService storage)
        {
            if (_preferredStorage != null)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Storage {_preferredStorage.GetType().Name} unset as preferred");
#endif
                _secondaryStorageVariants.Add(_preferredStorage.GetType(), _preferredStorage);
                _preferredStorage = null;
            }
            
            _preferredStorage = storage;
            _secondaryStorageVariants.Remove(storage.GetType());
            
#if UNITY_EDITOR
            Debugger.Log($"SaveService: Storage {storage.GetType().Name} set as preferred");
#endif
        }

        public async Task<(bool, StorageInfo)> TryLoadInfo<T>() where T : IStorageVariantService
        {
            if (_storageVariants.TryGetValue(typeof(T), out var storage) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} not registered");
#endif
                return (false, default);
            }
            
            return await TryLoadInfo(storage);
        }

        public async Task<(bool, StorageInfo)> TryLoadInfo(IStorageVariantService storage)
        {
            StorageInfo info;
            var key = nameof(StorageInfo);
            var result = await storage.Load(key);
            
            if (result.Item1 == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Meta not found in {storage.GetType().Name}");
#endif
                return (false, default);
            }
            
            var jsonBytes = result.Item2;
            
            string json = Encoding.UTF8.GetString(jsonBytes);
            
            try
            {
                info = JsonUtility.FromJson<StorageInfo>(json);
            }
            catch (Exception _)
            {
                info = default;
            }

            if (info == null)
            {
                json = Decrypt(jsonBytes);
                info = JsonUtility.FromJson<StorageInfo>(json);
            }

            return info == null ? (false, default) : (true, info);
        }

        public Task ResolveSaveByStorage<T>() where T : IStorageVariantService
        {
            if (_storageVariants.TryGetValue(typeof(T), out var storage) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} not registered");
#endif
                return Task.CompletedTask;
            }

            return ResolveSaveByStorage(storage);
        }

        public async Task ResolveSaveByStorage(IStorageVariantService storage)
        {
            foreach (var (typeStorage, storageData) in _storage)
            {
                var key = storageData.Data.GetType().Name;

                var result = await storage.Load(key);
                var jsonBytes = result.Item2;
                
                if (result.Item1 == false)
                {
                    foreach (var (_, storageVariant) in _storageVariants)
                    {
                        if (storage == storageVariant)
                            continue;

                        result = await TryLoad(key, storageVariant);
                        
                        if (result.Item1)
                        {
                            await storage.Save(key, jsonBytes);
                            break;
                        }
                    }
                    
                    continue;
                }

                foreach (var (typeStorageVariant, storageVariant) in _storageVariants)
                {
                    if (storage == storageVariant)
                        continue;
                    
                    await storageVariant.Save(key, jsonBytes);
                }

#if UNITY_EDITOR
                Debugger.Log($"SaveService: {key} resolved");
#endif
            }
            
            await SaveMeta();
        }

        public Task Clear()
        {
            foreach (var (typeStorage, storageData) in _storage)
            {
                var key = storageData.Data.GetType().Name;

                foreach (var (_, storageVariant) in _storageVariants)
                {
                    storageVariant.Remove(key);
                }
            }
            
            return Task.CompletedTask;
        }

        public async Task Save<T>() where T : IStorable
        {
            if (_storage.TryGetValue(typeof(T), out var storageData) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} not registered");
#endif
                return;
            }

            var data = (StorableData<T>)storageData;
            data.DataString = data.Data.ToStorage();
            var json = JsonUtility.ToJson(data);
            

            byte[] jsonBytes = _encrypt ? Encrypt(json) : Encoding.UTF8.GetBytes(json);
            
            var key = data.Data.GetType().Name;

            if (_preferredStorage != null)
            {
                await _preferredStorage.Save(key, jsonBytes);
            }
            
            foreach (var (typeStorage, storage) in _secondaryStorageVariants)
            {
                await storage.Save(key, jsonBytes);
            }
            
#if UNITY_EDITOR
            Debugger.Log($"SaveService: {typeof(T)} saved");
#endif
            
            await SaveMeta();
        }

        public async Task Load<T>() where T : IStorable
        {
            if (_storage.TryGetValue(typeof(T), out var storageData) == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} not registered");
#endif
                return;
            }

            var data = (StorableData<T>)storageData;
            var key = data.Data.GetType().Name;
            
            var result = await TryLoad(key, _preferredStorage);
            byte[] jsonBytes = result.Item2;
            
            if (result.Item1 == false)
            {
                bool isSuccess = false;
                
                foreach (var (typeStorage, storage) in _secondaryStorageVariants)
                {
                    result = await TryLoad(key, storage);
                    
                    if (result.Item1)
                    {
                        jsonBytes = result.Item2;
                        isSuccess = true;
                        break;
                    }
                }

                if (isSuccess == false)
                {
#if UNITY_EDITOR
                    Debugger.Log($"SaveService: Save {typeof(T)} not found");
#endif
                    
                    await Save<T>();
                    data.Created(data);
                    return;
                }
            }

            StorableData<T> dataJson;

            string json = Encoding.UTF8.GetString(jsonBytes);
            
            try
            {
                dataJson = JsonUtility.FromJson<StorableData<T>>(json);
            }
            catch (Exception _)
            {
                dataJson = null;
            }

            if (dataJson == null)
            {
                json = Decrypt(jsonBytes);
                dataJson = JsonUtility.FromJson<StorableData<T>>(json);
            }

            if (dataJson == null)
            {
                await Save<T>();
                data.Created(data);
                return;
            }

            if (data.Version != dataJson.Version)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} version mismatch {data.Version} != {dataJson.Version}");
                Debugger.Log($"SaveService: {typeof(T)} trying to migrate");
#endif

                var migratedData = Migrate<T>(dataJson.DataString, dataJson.Version, data.Version);
                if (migratedData == null)
                {
#if UNITY_EDITOR
                    Debugger.Log($"SaveService: {typeof(T)} migration failed");
#endif
                    await Save<T>();
                    return;
                }

#if UNITY_EDITOR
                Debugger.Log($"SaveService: {typeof(T)} migration success");
#endif

                dataJson.DataString = migratedData;
                dataJson.Version = data.Version;
                data.Loaded(dataJson);

                await Save<T>();
                return;
            }

            data.Loaded(dataJson);
        }

        private async Task<(bool, byte[])> TryLoad(string key, IStorageVariantService storage)
        {
            if (storage == null)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: storage not found");
#endif
                return (false, null);
            }
            
            var result = await storage.Load(key);
            
            if (result.Item1 == false)
            {
#if UNITY_EDITOR
                Debugger.Log($"SaveService: Save {key} not found");
#endif
                return (false, null);
            }
            
            return (true, result.Item2);
        }

        private async Task SaveMeta()
        {
            _storageInfo.SaveTime = _timeService.GetCurrentUtcTime();
            var key = _storageInfo.GetType().Name;
            
            var json = JsonUtility.ToJson(_storageInfo);

            byte[] jsonBytes = _encrypt ? Encrypt(json) : Encoding.UTF8.GetBytes(json);

            if (_preferredStorage != null)
            {
                await _preferredStorage.Save(key, jsonBytes);
            }

            foreach (var (typeStorage, storage) in _storageVariants)
            {
                await storage.Save(key, jsonBytes);
            }
            
#if UNITY_EDITOR
            Debugger.Log($"Meta saved time: {_storageInfo.SaveTime}");
#endif
        }
        

        private static SymmetricAlgorithm InitSymmetric(SymmetricAlgorithm algorithm, string password, int keyBitLength)
        {
            var salt = new byte[] { 1, 2, 23, 234, 37, 48, 134, 63, 248, 4 };

            const int Iterations = 234;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                if (!algorithm.ValidKeySize(keyBitLength))
                    throw new InvalidOperationException("Invalid size key");

                algorithm.Key = rfc2898DeriveBytes.GetBytes(keyBitLength / 8);
                algorithm.IV = rfc2898DeriveBytes.GetBytes(algorithm.BlockSize / 8);
                return algorithm;
            }
        }

        private static byte[] Transform(byte[] bytes, Func<ICryptoTransform> selectCryptoTransform)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    using (var cryptoStream =
                           new CryptoStream(memoryStream, selectCryptoTransform(), CryptoStreamMode.Write))
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    return memoryStream.ToArray();
                }
                catch
                {
                    return Array.Empty<byte>();
                }
            }
        }

        private byte[] Encrypt(string json)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            using (var compressStream = new MemoryStream())
            {
                using (var compressor = new GZipStream(compressStream, CompressionMode.Compress))
                {
                    compressor.Write(jsonBytes, 0, jsonBytes.Length);
                }

                jsonBytes = compressStream.ToArray();
            }

            using (var rijndael = InitSymmetric(Aes.Create(), _password, 256))
            {
                return Transform(jsonBytes, rijndael.CreateEncryptor);
            }
        }

        private string Decrypt(byte[] bytes)
        {
            using (var rijndael = InitSymmetric(Aes.Create(), _password, 256))
            {
                bytes = Transform(bytes, rijndael.CreateDecryptor);
            }
            
            using (var decompressStream = new MemoryStream(bytes))
            {
                using (var decompressor = new GZipStream(decompressStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(decompressor))
                    {
                        try
                        {
                            var json = reader.ReadToEnd();
                            return json;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}