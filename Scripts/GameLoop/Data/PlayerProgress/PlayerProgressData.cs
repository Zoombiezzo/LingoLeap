using System;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Data.PlayerProgress.Config;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using R3;

namespace _Client.Scripts.GameLoop.Data.PlayerProgress
{
    public class PlayerProgressData : IPlayerProgressData
    {
        private const string ConfigPath = "PlayerConfig";

        private readonly IStorageService _storageService;
        private readonly PlayerProgressStorage _storage;
        private readonly IAssetProvider _assetProvider;
        private PlayerConfig _config;
        private IDisposable _disposable;

        public ReadOnlyReactiveProperty<int> Soft => _storage.Soft;
        public ReadOnlyReactiveProperty<int> BoosterSelectChar => _storage.BoosterSelectChar;
        public ReadOnlyReactiveProperty<int> BoosterSelectWord => _storage.BoosterSelectWord;
        public ReadOnlyReactiveProperty<int> MindScore => _storage.MindScore;
        
        public PlayerProgressData(IStorageService storageService, IAssetProvider assetProvider)
        {
            _storageService = storageService;
            _assetProvider = assetProvider;
            _storage = new PlayerProgressStorage();
            _storageService.Register<IPlayerProgressData>(new StorableData<IPlayerProgressData>(this, _storage));
        }
        
        public void Load(IStorage data)
        {
            
        }

        public string ToStorage() => _storage.ToData(this);

        public async Task LoadData()
        {
            _config = await _assetProvider.Load<PlayerConfig>(ConfigPath);

            Initialize();
        }
        
        public void AddMindScore(int value)
        {
            value = Math.Max(0, value);
            _storage.MindScore.Value += value;
            _storageService.Save<IPlayerProgressData>();
        }
        
        public void ChangeSoft(int value)
        {
            _storage.Soft.Value += value;
            _storageService.Save<IPlayerProgressData>();
        }

        public void ChangeBoosterSelectChar(int value)
        {
            _storage.BoosterSelectChar.Value += value;
            _storageService.Save<IPlayerProgressData>();
        }

        public void ChangeBoosterSelectWord(int value)
        {
            _storage.BoosterSelectWord.Value += value;
            _storageService.Save<IPlayerProgressData>();
        }

        private void Initialize()
        {
            _storage.Soft.Value = _config.Soft;
            _storage.BoosterSelectChar.Value = 0;
            _storage.BoosterSelectWord.Value = 0;
            _storage.MindScore.Value = 0;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}