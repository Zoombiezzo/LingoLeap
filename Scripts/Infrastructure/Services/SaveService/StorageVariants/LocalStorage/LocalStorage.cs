using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.SystemIO;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.LocalStorage
{
    public class LocalStorage : IStorageVariantService
    {
        private IStorageVariantService _localStorage;

        public LocalStorage()
        {
#if UNITY_EDITOR
            _localStorage = new SystemIOStorage();
#elif (UNITY_WEBGL || PLATFORM_WEBGL) && !UNITY_EDITOR
            _localStorage = new LocalStorageWebGL();
#endif
        }
        
        public Task<bool> Save(string key, byte[] value) => _localStorage.Save(key, value);

        public Task<(bool, byte[])> Load(string key) => _localStorage.Load(key);

        public void Remove(string key)
        {
            _localStorage.Remove(key);
        }

        public void Clear()
        {
            _localStorage.Clear();
        }
    }
}