using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.MapService.Factories;
using _Client.Scripts.Infrastructure.Services.MapService.Updaters;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public class MapService : IMapService
    {
        private const string AssetPath = "MapConfig";

        private readonly IAssetProvider _assetProvider;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;

        private readonly MapStorage _storage;
        private readonly LocationPictureFactory _locationPictureFactory;
        private readonly ILocationPreviewFactory _previewFactory;
        private readonly IRewardService _rewardService;
        private readonly IAddressablesService _addressablesService;

        private List<MapConfig> _mapConfigs = new(2);
        private List<ILocationConfig> _locations = new(16);
        private readonly Dictionary<string, MapConfig> _mapConfigMap = new(2);
        private readonly Dictionary<string, ILocationConfig> _mapLocationConfigMap = new(2);
        private LocationConfig _baseLocationConfig;
        
        private List<IProgressUpdater> _progressUpdaters;

        public IReadOnlyList<IMapConfig> MapConfigs => _mapConfigs;
        public string CurrentSelectedLocationId => _storage.CurrentSelectedLocationId;
        public int CurrentIndex => _storage.CurrentOpenedLocationIndex;
        public int ProgressCounter => _storage.ProgressCounter;

        public event Action<string> LocationSelected;
        public event Action<int> LocationOpened;
        public event Action<int> ProgressChanged;
        public event Action<ILocationConfig> OnShowScreenRewardLocation;

        public MapService(IAssetProvider assetProvider, IAddressablesService addressablesService, IStorageService storageService,
            ILocalizationService localizationService, ILevelProgressData levelProgressData, IRewardService rewardService)
        {
            _assetProvider = assetProvider;
            _storageService = storageService;
            _addressablesService = addressablesService;
            _localizationService = localizationService;
            _rewardService = rewardService;

            _progressUpdaters = new List<IProgressUpdater>()
            {
                new LevelProgressUpdater(this, levelProgressData),
            };
            
            _locationPictureFactory = new LocationPictureFactory(_addressablesService);
            _previewFactory = new LocationPreviewFactory(this, _localizationService);
            
            _storage = new MapStorage();
            _storageService.Register<IMapService>(new StorableData<IMapService>(this, _storage));
        }
        
        public async Task LoadData()
        {
            var mapConfigs = await _assetProvider.LoadAll<MapConfig>(AssetPath);

            if(mapConfigs.Count == 0)
                return;
            
            foreach (var mapConfig in mapConfigs)
            {
                if(_mapConfigMap.ContainsKey(mapConfig.Id))
                    continue;
                
                _mapConfigMap.TryAdd(mapConfig.Id, mapConfig);

                foreach (var locationsCategory in mapConfig.Categories)
                {
                    foreach (var location in locationsCategory.Locations)
                    {
                        _mapLocationConfigMap.TryAdd(location.Id, location);
                        _locations.Add(location);
                    }
                }

                var baseLocation = mapConfig.Base;
                _baseLocationConfig = baseLocation;
                _mapLocationConfigMap.TryAdd(baseLocation.Id, baseLocation);
                _mapConfigs.Add(mapConfig);
            }
            
            _storage.CurrentSelectedLocationId = _baseLocationConfig.Id;
            _storage.CurrentOpenedLocationIndex = 0;
            _storage.ProgressCounter = 0;
        }

        public void ShowRewardWindow(ILocationConfig config) => OnShowScreenRewardLocation?.Invoke(config);

        public bool TryGetCurrentLocationConfig(out ILocationConfig config)
        {
            var currentIndex = _storage.CurrentOpenedLocationIndex;
            if (currentIndex >= _locations.Count)
            {
                config = null;
                return false;
            }

            config = _locations[_storage.CurrentOpenedLocationIndex];

            return true;
        }

        public bool TryGetLocationByIndexConfig(int index, out ILocationConfig config)
        {
            if (index >= _locations.Count)
            {
                config = null;
                return false;
            }

            config = _locations[index];

            return true;
        }

        public bool TryGetLocationConfig(string id, out ILocationConfig config) => _mapLocationConfigMap.TryGetValue(id, out config);

        public bool IsLocationAvailableToSelect(string locationId) => true;

        public async Task<LocationPicture> CreatePicture(Transform parent, ILocationConfig config)
        {
            var picture = await _locationPictureFactory.Create(parent, config);
            return picture;
        }

        public Task<LocationPreview> CreatePreview(Transform parent, ILocationConfig config)
        {
            var preview = _previewFactory.Create(parent,config.PreviewPrefab, config);
            return Task.FromResult(preview);
        }

        public Task<LocationCategoryPreview> CreateCategoryPreview(Transform parent, ILocationsCategory category)
        {
            var preview = _previewFactory.Create(parent, category.CategoryPrefab, category);
            return Task.FromResult(preview);
        }

        public void Release(ILocationConfig config) => _locationPictureFactory.Release(config);
        public bool TrySelectLocation(string location)
        {
            if (CurrentSelectedLocationId == location)
                return false;
            
            if (_mapLocationConfigMap.TryGetValue(location, out var locationConfig))
            {
                _storage.CurrentSelectedLocationId = locationConfig.Id;
                
                _storageService.Save<IMapService>();
                LocationSelected?.Invoke(locationConfig.Id);
                return true;
            }
            
            return false;
        }

        public void AddProgress(int count)
        {
            _storage.ProgressCounter += count;
            
            _storageService.Save<IMapService>();
            ProgressChanged?.Invoke(ProgressCounter);
        }

        public bool TryCollectReward(out IReadOnlyList<IRewardInfo> rewards)
        {
            rewards = null;
            
            var progress = _storage.ProgressCounter;
            var currentIndex = _storage.CurrentOpenedLocationIndex;
            
            if(TryGetLocationByIndexConfig(currentIndex, out var locationConfig) == false)
                return false;
            
            if (progress < locationConfig.RequiredCountLevels)
                return false;
            
            var rewardsList = new List<IRewardInfo>(2);

            while (progress >= locationConfig.RequiredCountLevels)
            {
                progress -= locationConfig.RequiredCountLevels;
                currentIndex++;
                
                var rewardInfo = locationConfig.RewardInfo;
                
                if (rewardInfo != null)
                {
                    rewardsList.Add(locationConfig.RewardInfo);
                }
                
                if(TryGetLocationByIndexConfig(currentIndex, out locationConfig) == false)
                    break;
            }

            foreach (var rewardInfo in rewardsList)
            {
                _rewardService.TryCollectReward(rewardInfo);
            }
            
            var locationChanged = _storage.CurrentOpenedLocationIndex < currentIndex;
            _storage.CurrentOpenedLocationIndex = currentIndex;
            _storage.ProgressCounter = progress;
            
            rewards = rewardsList;
            
            _storageService.Save<IMapService>();
            
            ProgressChanged?.Invoke(ProgressCounter);
            
            if (locationChanged)
            {
                LocationOpened?.Invoke(CurrentIndex);
            }
            
            return true;
        }

        public void Load(IStorage data)
        {
            if (_mapLocationConfigMap.ContainsKey(_storage.CurrentSelectedLocationId) == false)
            {
                _storage.CurrentSelectedLocationId = _baseLocationConfig.Id;
            }

            TryCollectReward(out _);
            EnableUpdaters();
        }

        public string ToStorage() => _storage.ToData(this);

        private void EnableUpdaters()
        {
            foreach (var updater in _progressUpdaters)
            {
                updater.Enable();
            }
        }
        
        private void DisableUpdaters()
        {
            foreach (var updater in _progressUpdaters)
            {
                updater.Disable();
            }
        }

    }
}