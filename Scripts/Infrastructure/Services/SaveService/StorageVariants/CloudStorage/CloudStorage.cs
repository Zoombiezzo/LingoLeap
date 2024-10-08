using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.SystemIO;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.CloudStorage
{
    public class CloudStorage : IStorageVariantService
    {
        private readonly IAuthService _authService;

        private IStorageVariantService _storageVariantService;

        public CloudStorage(IAuthService authService)
        {
            _authService = authService;

#if UNITY_EDITOR
            _storageVariantService = new SystemIOStorage();
#elif (UNITY_WEBGL || PLATFORM_WEBGL) && !UNITY_EDITOR
            _storageVariantService = new GameSDKStorage(_authService);
#endif
        }

        public Task<bool> Save(string key, byte[] value)
        {
            if (_storageVariantService == null)
                return Task.FromResult(false);
            
            return _storageVariantService.Save(key, value);
        }

        public Task<(bool, byte[])> Load(string key)
        {
            if (_storageVariantService == null)
                return Task.FromResult<(bool, byte[])>((false, null));
            
            return _storageVariantService.Load(key);
        }

        public void Remove(string key)
        {
            if (_storageVariantService == null)
                return;
            
            _storageVariantService.Remove(key);
        }

        public void Clear()
        {
            if (_storageVariantService == null)
                return;
            
            _storageVariantService.Clear();
        }
    }
}