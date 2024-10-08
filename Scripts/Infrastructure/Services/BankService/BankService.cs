using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.BankService.PurchaseDataFactories;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public class BankService : IBankService
    {
        private const string BankPath = "BankCategory";
        private const string ItemsPath = "BankItem";
        private Dictionary<string, BankConfig> _configs;
        private Dictionary<string, BankItem> _items;
        private Dictionary<string, BankItem> _itemsByStoreId;
        private Dictionary<string, PurchasedItem> _purchasedItemsDict = new(10);
        private Dictionary<CurrencyType, IBankPurchaseDataFactory> _purchaseDataFactories = new(10);

        private readonly IAssetProvider _assetProvider;
        private readonly IPurchaseService _purchaseService;

        private readonly IStorageService _storageService;
        private readonly BankStorageData _storage;
        private readonly IRewardService _rewardService;

        public event Action<IBankItem> OnPurchased;

        public BankService(IPurchaseService purchaseService,
            IAssetProvider assetProvider, IStorageService storageServiceService,
            IRewardService rewardService)
        {
            _purchaseService = purchaseService;
            _assetProvider = assetProvider;
            _storageService = storageServiceService;
            _rewardService = rewardService;

            _purchaseDataFactories = new Dictionary<CurrencyType, IBankPurchaseDataFactory>()
            {
                [CurrencyType.Hard] = new GameCurrencyDataPurchaseFactory(),
                [CurrencyType.Soft] = new GameCurrencyDataPurchaseFactory(),
                [CurrencyType.Real] = new ProductDataPurchaseFactory(),
                [CurrencyType.Ads] = new AdsDataPurchaseFactory(),
            };

            _storage = new BankStorageData();
            _storageService.Register<IBankService>(new StorableData<IBankService>(this, _storage));
        }

        public BankConfig GetBankConfig(string id)
        {
            if (_configs.TryGetValue(id, out var config))
            {
                return config;
            }

            return null;
        }
        
        public BankItem GetItemConfig(string id)
        {
            if (_items.TryGetValue(id, out var config))
            {
                return config;
            }

            return null;
        }

        public void Purchase(string id, Action<BankItem, bool> callback = null)
        {
            var config = GetItemConfig(id);

            if (config == null)
            {
                callback?.Invoke(null, false);
                return;
            }

            if (_purchaseDataFactories.TryGetValue(config.Currency, out var factory) == false)
            {
                callback?.Invoke(config, false);
                return;
            }
            
            _purchaseService.Purchase(
                factory.Create(config),
                new PurchaseItemReceiver(this, config, result =>
                {
                    callback?.Invoke(config, result);
                    
                    if (result)
                    {
                        OnPurchased?.Invoke(config);
                    }
                }));
        }

        public async Task Consume(PurchaseProduct purchaseProduct, Action<BankItem, bool> callback = null)
        {
            BankItem config = null;

            if (string.IsNullOrEmpty(purchaseProduct.id) == false)
            {
                var id = purchaseProduct.id;
                config = GetItemConfig(id);
            }
            else
            {
                var id = purchaseProduct.storeId;
                config = GetItemConfigByStoreId(id);
            }

            if (config == null)
            {
                callback?.Invoke(config, false);
                return;
            }

            
            await _purchaseService.Consume(
                purchaseProduct,
                new PurchaseItemReceiver(this, config, result =>
                {
                    callback?.Invoke(config, result);

                    if (result)
                    {
                        OnPurchased?.Invoke(config);
                    }
                }));
        }

        internal async void CollectItem(IBankItem item)
        {
            if (_purchasedItemsDict.TryGetValue(item.Id, out var purchasedItem))
            {
                purchasedItem.Count += 1;
            }
            else
            {
                purchasedItem = new PurchasedItem() { Id = item.Id, Count = 1 };
                _storage.PurchasedItems.Add(purchasedItem);
                _purchasedItemsDict.Add(item.Id, purchasedItem);
            }
            
            Collect(item);
            
            await _storageService.Save<IBankService>();
        }

        private void Collect(IBankItem item)
        {
            _rewardService.TryCollectReward(item.Reward);
        }
        
        private BankItem GetItemConfigByStoreId(string id) => _itemsByStoreId.GetValueOrDefault(id);

        public void Load(IStorage data)
        {
            _purchasedItemsDict = _storage.PurchasedItems.ToDictionary(el => el.Id, el => el);
        }

        public string ToStorage()
        {
            var storableData = _storageService.Get<IBankService>();
            return storableData.Storage.ToData(this);
        }

        public async Task LoadData()
        {
            var configs = await _assetProvider.LoadAll<BankConfig>(BankPath);
            _configs = new Dictionary<string, BankConfig>(configs.Count);
            
            foreach (var config in configs)
            {
                _configs.TryAdd(config.Id, config);
            }
            
            var items = await _assetProvider.LoadAll<BankItem>(ItemsPath);
            _items = new Dictionary<string, BankItem>(items.Count);
            _itemsByStoreId = new Dictionary<string, BankItem>(items.Count);
            
            foreach (var item in items)
            {
                _items.TryAdd(item.Id, item);
                
                if (item.Currency == CurrencyType.Real)
                {
                    _purchaseService.AddProduct(item.ProductId, ProductType.Consumable);
                    _itemsByStoreId.TryAdd(item.ProductId, item);
                }
            }
        }
    }
}