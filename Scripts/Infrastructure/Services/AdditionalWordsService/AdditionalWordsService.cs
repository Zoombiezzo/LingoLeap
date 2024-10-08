using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;

namespace _Client.Scripts.Infrastructure.Services.AdditionalWordsService
{
    public class AdditionalWordsService : IAdditionalWordsService
    {
        private const string ConfigPath = "AdditionalWords";
        private readonly IAssetProvider _assetProvider;
        private AdditionalWordsConfig _config;
        
        private Dictionary<int, IAdditionalWordsLevelInfo> _levelInfo = new(16);
        private IAdditionalWordsLevelInfo _baseLevelInfo;

        public AdditionalWordsService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public async Task LoadData()
        {
            _config = await _assetProvider.Load<AdditionalWordsConfig>(ConfigPath);

            _levelInfo.Clear();
            
            for (var index = 0; index < _config.Levels.Count; index++)
            {
                var level = _config.Levels[index];
                _levelInfo.TryAdd(index, level);
            }

            _baseLevelInfo = _config.BaseLevel;
        }

        public bool TryGetLevelInfo(int level, out IAdditionalWordsLevelInfo levelInfo)
        {
            if(_levelInfo.TryGetValue(level, out levelInfo))
                return true;

            if (_baseLevelInfo == null)
                return false;
            
            levelInfo = _baseLevelInfo;
            return true;
        }
    }
}