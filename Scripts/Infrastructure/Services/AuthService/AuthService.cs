using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProvider _authProvider;
        private readonly IStorageService _storageService;
        private readonly AuthStorageData _storage;

        public AuthService(IAuthProvider authProvider, IStorageService storage)
        {
            _authProvider = authProvider;
            _storageService = storage;

            _authProvider.OnSignInTypeChanged += OnSignInTypeChangedHandler;

            _storage = new AuthStorageData();
            _storageService.Register<IAuthService>(new StorableData<IAuthService>(this, _storage));
        }

        public SignInType SignInType => _authProvider.SignInType;

        public bool IsCanceled => _storage.IsCanceled;

        public event Action<SignInType> OnSignInTypeChanged;

        public Task SignInAsGuest()
        {
            return _authProvider.SignInAsGuest();
        }

        public Task SignIn()
        {
            return _authProvider.SignIn();
        }

        public void CancelSignIn(bool isCanceled)
        {
            _storage.IsCanceled = isCanceled;
            _storageService.Save<IAuthService>();
        }

        public event Action OnChanged;

        public void Load(IStorage data) { }
        
        public string ToStorage()
        {
            var storableData = _storageService.Get<IAuthService>();
            return storableData.Storage.ToData(this);
        }

        private void OnSignInTypeChangedHandler(SignInType signInType)
        {
            OnSignInTypeChanged?.Invoke(signInType);
        }
    }
}