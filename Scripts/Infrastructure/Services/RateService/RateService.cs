using System;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.RateService
{
    public class RateService : IRateService
    {
        private readonly IRateProvider _provider;
        private readonly IStorageService _storageService;
        private readonly RateStorageData _storage;

        public event Action OnRatedSuccess;

        public event Action OnRatedFailed;
        
        public bool Rated => _storage.Rated;
        public bool RatedShowed => _storage.RatedShowed;

        public RateService(IRateProvider provider, IStorageService storageService)
        {
            _provider = provider;
            _storageService = storageService;
            
            _provider.OnRatedSuccess += OnRatedSuccessHandler;
            _provider.OnRatedFailed += OnRatedFailedHandler;
            
            _storage = new RateStorageData();
            _storageService.Register<IRateService>(new StorableData<IRateService>(this, _storage));
        }

        private async void OnRatedSuccessHandler()
        {
            _storage.RatedShowed = true;
            _storage.Rated = true;

            await _storageService.Save<IRateService>();
            
            OnRatedSuccess?.Invoke();
        }

        private async void OnRatedFailedHandler()
        {
            _storage.RatedShowed = true;
            await _storageService.Save<IRateService>();
            
            OnRatedFailed?.Invoke();
        }

        public async void Rate()
        {
            _storage.RatedShowed = true;
            await _storageService.Save<IRateService>();
            
            _provider.Rate();
        }
        
        public event Action OnChanged;
        public void Load(IStorage data) { }
        
        public string ToStorage()
        {
            var storableData = _storageService.Get<IRateService>();
            return storableData.Storage.ToData(this);
        }
    }
}