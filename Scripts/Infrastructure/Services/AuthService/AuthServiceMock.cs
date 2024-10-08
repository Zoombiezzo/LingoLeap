using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    public class AuthServiceMock : IAuthService
    {
        private readonly IStorageService _storage;
        
        private bool _isCanceled;
        private SignInType _signInType = SignInType.Guest;

        public AuthServiceMock(IStorageService storage)
        {
            _storage = storage;
        }

        public SignInType SignInType => _signInType;

        public bool IsCanceled => _isCanceled;

        public event Action<SignInType> OnSignInTypeChanged;

        public Task SignInAsGuest()
        {
            _signInType = SignInType.Guest;
            OnSignInTypeChanged?.Invoke(_signInType);
            
            return Task.CompletedTask;
        }

        public Task SignIn()
        {
            _signInType = SignInType.Account;
            OnSignInTypeChanged?.Invoke(_signInType);
            
            return Task.CompletedTask;
        }


        public void CancelSignIn(bool isCanceled)
        {
            _isCanceled = isCanceled;
        }

        public event Action OnChanged;

        public void Load(IStorage data)
        {
            var storageData = (AuthStorageData)data;
            _isCanceled = storageData.IsCanceled;
        }

        public string ToStorage()
        {
            var storableData = _storage.Get<IAuthService>();
            return storableData.Storage.ToData(this);
        }
    }
}