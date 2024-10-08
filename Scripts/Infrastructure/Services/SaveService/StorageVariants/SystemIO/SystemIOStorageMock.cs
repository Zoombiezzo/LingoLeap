using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.SystemIO
{
    public class SystemIOStorageMock : IStorageVariantService
    {
        private string DataPath => _dataPath;

        private string _dataPath;
        
        public SystemIOStorageMock()
        {
            _dataPath = Path.Combine(Application.persistentDataPath, "mock");
        }
        
        public async Task<bool> Save(string key, byte[] value)
        {
            if (Directory.Exists(_dataPath) == false)
            {
                Directory.CreateDirectory(_dataPath);
            }
            
            await File.WriteAllBytesAsync(GetPath(key),value);
            
            return true;
        }

        public async Task<(bool, byte[])> Load(string key)
        {
            if (HasKey(key) == false) return (false, null);
            
            var value = await File.ReadAllBytesAsync(GetPath(key));
            

            return (true, value);
        }

        public void Remove(string key)
        {
            File.Delete(GetPath(key));
        }

        public void Clear()
        {
            
        }

        private string GetPath(string key)
        {
            return Path.Combine(DataPath, $"{key}.data");
        }
        
        private bool HasKey(string key)
        {
            return File.Exists(GetPath(key));
        }
    }
}