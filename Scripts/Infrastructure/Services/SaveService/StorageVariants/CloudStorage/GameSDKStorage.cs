using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AuthService;
using GameSDK.GameStorage;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.CloudStorage
{
    public class GameSDKStorage : IStorageVariantService
    {
        private readonly IAuthService _authService;

        public GameSDKStorage(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<bool> Save(string key, byte[] value)
        {
            if(_authService.SignInType != SignInType.Account)
                return false;
            
            
            if (value == null)
                return false;
            
            var base64Str = Convert.ToBase64String(value);


            var result = await Storage.Save(key, base64Str);
            
            return result == StorageStatus.Success;
        }

        public async Task<(bool, byte[])> Load(string key)
        {
            if(_authService.SignInType != SignInType.Account)
                return (false, Array.Empty<byte>());
            
            var result = await Storage.Load(key);
            
            
            if (result.Item1 == StorageStatus.Success)
            {
                if (string.IsNullOrEmpty(result.Item2))
                    return (false, Array.Empty<byte>());
                
                var value = Convert.FromBase64String(result.Item2);
                
                
                return (true, value);
            }
            
            return (false, Array.Empty<byte>());
        }

        public async void Remove(string key)
        {
            await Save(key, Array.Empty<byte>());
        }

        public void Clear()
        {
            
        }
    }
}