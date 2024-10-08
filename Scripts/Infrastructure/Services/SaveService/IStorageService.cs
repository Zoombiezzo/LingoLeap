using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants;

namespace _Client.Scripts.Infrastructure.Services.SaveService
{
    public interface IStorageService : IService
    {
        StorageInfo CurrentStorageInfo { get; }
        Task Initialize();
        void Register<T>(IStorableData data) where T : IStorable;
        IStorableData Get<T>() where T : IStorable;
        void RegisterMigrate<T>(IMigration migration) where T : IStorable;
        string Migrate<T>(string data, int fromVersion, int toVersion) where T : IStorable;
        bool TryGetMigrationPath<T>(int fromVersion, int toVersion, out List<int> path) where T : IStorable;
        void RegisterStorage<T>(IStorageVariantService storage) where T : IStorageVariantService;
        void UnregisterStorage<T>() where T : IStorageVariantService;
        void SetPreferredStorage<T>() where T : IStorageVariantService;
        void SetPreferredStorage(IStorageVariantService storage);
        Task<(bool, StorageInfo)> TryLoadInfo<T>() where T : IStorageVariantService;
        Task<(bool, StorageInfo)> TryLoadInfo(IStorageVariantService storage);
        Task ResolveSaveByStorage<T>() where T : IStorageVariantService;
        Task ResolveSaveByStorage(IStorageVariantService storage);
        Task Save<T>() where T : IStorable;
        Task Load<T>() where T : IStorable;
        Task Clear();
    }
}