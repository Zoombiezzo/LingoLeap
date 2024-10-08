using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.RandomService;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SpinWheelService.PurchaseDataFactories;
using _Client.Scripts.Infrastructure.Services.TimeService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public class SpinWheelService : ISpinWheelService
    {
        private const string AssetPath = "SpinWheel";
        
        private readonly IAssetProvider _assetProvider;
        private readonly SpinWheelStorage _storage;
        private readonly IStorageService _storageService;
        private readonly IRewardService _rewardService;
        private readonly List<ISpinWheelItem> _currentSpinItems = new(8);
        private readonly ITimeService _timeService;
        private readonly IPurchaseService _purchaseService;
        private readonly IRandomService _randomService;

        private Dictionary<CurrencyType, IPurchaseDataFactory> _purchaseDataFactories = new(10);

        private List<SpinWheelRegionInfo> _regionConfigs = new(8);
        private SpinWheelInfo _info;

        public IReadOnlyList<ISpinWheelItem> CurrentSpinItems => _currentSpinItems;

        public event Action OnSpinUsed;
        
        public SpinWheelService(IAssetProvider assetProvider, IStorageService storageService, IRewardService rewardService,
            ITimeService timeService, IPurchaseService purchaseService, IRandomService randomService)
        {
            _assetProvider = assetProvider;
            _storageService = storageService;
            _rewardService = rewardService;
            _timeService = timeService;
            _purchaseService = purchaseService;
            _randomService = randomService;
            
            _purchaseDataFactories = new Dictionary<CurrencyType, IPurchaseDataFactory>()
            {
                [CurrencyType.Hard] = new GameCurrencyDataPurchaseFactory(),
                [CurrencyType.Free] = new GameCurrencyDataPurchaseFactory(),
                [CurrencyType.Soft] = new GameCurrencyDataPurchaseFactory(),
                [CurrencyType.Real] = new ProductDataPurchaseFactory(),
                [CurrencyType.Ads] = new AdsDataPurchaseFactory(),
            };
            
            _storage = new SpinWheelStorage();
            _storageService.Register<ISpinWheelService>(new StorableData<ISpinWheelService>(this, _storage));
        }
        
        public async Task LoadData()
        {
            var assets = await _assetProvider.LoadAll<SpinWheelInfo>(AssetPath);

            if(assets.Count == 0)
                return;
            
            _info = assets[0];
            _regionConfigs = _info.Regions;
            
            UpdateCurrentSpinItems();
        }

        public void Load(IStorage data)
        {
            UpdateCurrentSpinItems();
        }

        public string ToStorage() => _storage.ToData(this);
        
        public bool IsPossibleSpin() => _storage.CurrentSpin < _info.SpinSettings.Count;
        public int GetSpinLeft() => _info.SpinSettings.Count - _storage.CurrentSpin;
        public int GetMaxSpins() => _info.SpinSettings.Count;
        public int GetRandomIndex()
        {
            var items = _currentSpinItems;
            
            var maxChance = 0f;
            
            foreach (var item in items)
            {
                maxChance += item.Chance;
            }
            
            var randomValue = _randomService.Range(0f, maxChance);
            
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (randomValue <= item.Chance)
                {
                    return i;
                }

                randomValue -= item.Chance;
            }
            
            return 0;
        }

        public bool TryGetCurrentSpinSetting(out ISpinSetting spinSetting)
        {
            spinSetting = null;
            if (_storage.CurrentSpin >= _info.SpinSettings.Count)
            {
                return false;
            }
            
            spinSetting = _info.SpinSettings[_storage.CurrentSpin];

            return true;
        }

        private DateTime GetNextUpdateDateTime()
        {
            TimeSpan updateTime =  _info.TimeUpdateSpin.GetTime().TimeOfDay;
            var lastUpdateTime = new DateTime(_storage.LastTimeUpdateTicks);

            DateTime nextUpdateDateTime = lastUpdateTime.Date + updateTime;
            if (lastUpdateTime.TimeOfDay >= updateTime)
            {
                nextUpdateDateTime = nextUpdateDateTime.AddDays(1);
            }

            return nextUpdateDateTime;
        }
        
        public TimeSpan GetTimeLeftToUpdate()
        {
            DateTime nextUpdateDateTime = GetNextUpdateDateTime();
            DateTime currentTime = _timeService.GetCurrentUtcDateTime();

            if (currentTime > nextUpdateDateTime)
            {
                return TimeSpan.Zero;
            }

            return nextUpdateDateTime - currentTime;
        }

        public bool CanUpdate()
        {
            DateTime currentTime = _timeService.GetCurrentUtcDateTime();
            DateTime nextUpdateDateTime = GetNextUpdateDateTime();

            return currentTime >= nextUpdateDateTime;
        }

        public void UpdateSpins()
        {
            _storage.LastTimeUpdateTicks = _timeService.GetCurrentUtcDateTime().Ticks;
            _storage.CurrentSpin = 0;
            
            foreach (var region in _storage.Regions)
            {
                region.Stage = 0;
            }

            UpdateCurrentSpinItems();

            _storageService.Save<ISpinWheelService>();
        }

        public void Purchase(Action<bool> callback = null)
        {
            if (TryGetCurrentSpinSetting(out var spinSetting) == false)
            {
                callback?.Invoke(false);
                return;
            }

            if (_purchaseDataFactories.TryGetValue(spinSetting.Currency, out var factory) == false)
            {
                callback?.Invoke(false);
                return;
            }

            _purchaseService.Purchase(
                factory.Create(spinSetting),
                new PurchaseItemReceiver(this, spinSetting, result => { callback?.Invoke(result); }));
        }

        public bool TryGetReward(int regionIndex, out IRewardInfo reward)
        {
            reward = null;
            
            if (_storage.CurrentSpin >= _info.SpinSettings.Count)
                return false;

            var stage = 0;
            if (_storage.TryGetRegion(regionIndex, out var record) == false)
            {
                _storage.AddRegion(regionIndex, 0);
                _storage.TryGetRegion(regionIndex, out record);
            }
            else
            {
                stage = record.Stage;
            }

            if(_regionConfigs.Count <= regionIndex)
                return false;

            var regionConfig = _regionConfigs[regionIndex];
            var items = regionConfig.SpinWheelItems;

            ISpinWheelItem spinWheelItemInfo = stage >= items.Count ? regionConfig.BaseItem : items[stage];
            
            reward = spinWheelItemInfo.Reward;
            
            if (_rewardService.TryCollectReward(reward) == false)
                return false;

            record.Stage++;

            _storage.CurrentSpin++;
            
            UpdateCurrentSpinItems();
            
            OnSpinUsed?.Invoke();
            
            _storageService.Save<ISpinWheelService>();
            return true;
        }

        private void UpdateCurrentSpinItems()
        {
            _currentSpinItems.Clear();
            
            for(var i = 0; i < _regionConfigs.Count; i++)
            {
                var stage = 0;
                if (_storage.TryGetRegion(i, out var record))
                {
                    stage = record.Stage;
                }
                
                var config = _regionConfigs[i];
                var spinWheelItemInfo = stage >= config.SpinWheelItems.Count ? config.BaseItem : config.SpinWheelItems[stage];
                _currentSpinItems.Add(spinWheelItemInfo);
            } 
        }
    }
}