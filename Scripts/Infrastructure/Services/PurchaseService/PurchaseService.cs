using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AdvertisingService;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.WalletService;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class PurchaseService : IPurchaseService
    {
        private readonly Dictionary<CurrencyType, IPurchaseProvider> _providers;
        private readonly IWalletService _walletService;
        private readonly IProductPurchaseService _productPurchaseService;
        private readonly IAdvertisingService _advertisingService;
        private readonly IStorageService _storageService;

        private List<PurchaseProduct> _pendingProducts = new List<PurchaseProduct>(10);
        private Dictionary<string, List<PurchaseProduct>> _pendingProductsDictionary = new (10);
        public IReadOnlyList<PurchaseProduct> PendingProducts => _pendingProducts;

        public PurchaseService(IStorageService storageService, IWalletService walletService, IProductPurchaseService productPurchaseService, IAdvertisingService advertisingService)
        {
            _storageService = storageService;
            _walletService = walletService;
            _productPurchaseService = productPurchaseService;
            _advertisingService = advertisingService;
            
            _providers = new Dictionary<CurrencyType, IPurchaseProvider>
            {
                { CurrencyType.Hard, new BaseCurrencyProvider(_walletService) },
                { CurrencyType.Soft, new BaseCurrencyProvider(_walletService) },
                { CurrencyType.Free, new FreeCurrencyProvider() },
                { CurrencyType.Real, new ProductCurrencyProvider(this, _productPurchaseService) },
                { CurrencyType.Ads, new AdsCurrencyProvider(_advertisingService) },
                { CurrencyType.BoosterSelectChar, new BaseCurrencyProvider(_walletService)},
                { CurrencyType.BoosterSelectWord, new BaseCurrencyProvider(_walletService)}
            };
            
            _storageService.Register<IPurchaseService>(new StorableData<IPurchaseService>(this, new PurchaseStorageData()));
        }

        public string IconId => _productPurchaseService.IconId;
        public string CurrencyCode => _productPurchaseService.CurrencyCode;

        public void Initialize()
        {
            _productPurchaseService.Initialize();
        }

        public string GetPrice(string id) => _productPurchaseService.GetPrice(id);

        public void Purchase(IPurchaseData data, IPurchaseReceiver receiver)
        {
            if (_providers.TryGetValue(data.CurrencyType, out var provider) == false)
            {
                receiver.FailPurchase(new MessageFailData("No providers for this currency type"));
                return;
            }

            provider.Execute(data, receiver);
        }

        public async Task Consume(PurchaseProduct product, IPurchaseReceiver receiver)
        {
            if (_pendingProductsDictionary.TryGetValue(product.storeId, out var list) == false)
            {
                receiver.FailPurchase(new MessageFailData("No pending purchases"));
                return;
            }
            
            
            await _productPurchaseService.Consume(product.storeId, OnCallback);

            void OnCallback(string id, bool success)
            {
                if (success == false)
                {
                    receiver.FailPurchase(new MessageFailData("No pending purchases"));
                    return;
                }
                
                list.Remove(product);
                _pendingProducts.Remove(product);

                _storageService.Save<IPurchaseService>();
                
                receiver.SuccessPurchase();
            }
        }

        public void AddProduct(string id, ProductType productType)
        {
            _productPurchaseService.AddProduct(id, productType);
        }

        public async void AddPendingPurchase(string id, string storeId)
        {
            AddPendingPurchaseInternal(id, storeId);
            await _storageService.Save<IPurchaseService>();
        }

        public async void RemovePendingPurchase(string id, string storeId)
        {
            RemovePendingPurchaseInternal(id, storeId);
            await _storageService.Save<IPurchaseService>();
        }

        public event Action OnChanged;
        public void Load(IStorage data)
        {
            var purchaseStorageData = (PurchaseStorageData)data; 
            _pendingProducts.Clear();
            _pendingProducts.AddRange(purchaseStorageData.PendingProducts);
            
            _pendingProductsDictionary.Clear();
            foreach (var pendingProduct in _pendingProducts)
            {
                if (_pendingProductsDictionary.TryGetValue(pendingProduct.storeId, out var list) == false)
                {
                    list = new List<PurchaseProduct>(2);
                    _pendingProductsDictionary.Add(pendingProduct.storeId, list);
                }

                list.Add(pendingProduct);
            }

            CollectPendingProductsFromService();

            OnChanged?.Invoke();
        }

        public string ToStorage()
        {
            var storableData = _storageService.Get<IPurchaseService>();
            return storableData.Storage.ToData(this);
        }

        private void AddPendingPurchaseInternal(string id, string storeId)
        {
            var purchaseProduct = new PurchaseProduct()
            {
                id = id,
                storeId = storeId
            };

            _pendingProducts.Add(purchaseProduct);
           
            if (_pendingProductsDictionary.TryGetValue(storeId, out var list) == false)
            {
                list = new List<PurchaseProduct>(2);
                _pendingProductsDictionary.Add(storeId, list);
            }
           
            list.Add(purchaseProduct);
        }
        
        private void RemovePendingPurchaseInternal(string id, string storeId)
        {
            if (_pendingProductsDictionary.TryGetValue(storeId, out var list) == false)
                return;

            PurchaseProduct currentProduct = null;
            
            foreach (var product in list)
            {
                if (product.id == id)
                {
                    currentProduct = product;
                    break;
                }
            }

            if (currentProduct == null) return;
            
            _pendingProducts.Remove(currentProduct);
            list.Remove(currentProduct);
        }
        
        private void CollectPendingProductsFromService()
        {
            var pendingProducts = _productPurchaseService.PurchasedProductIds;
            

            Dictionary<string, int> counts = new Dictionary<string, int>();
            
            foreach (var pendingProduct in pendingProducts)
            {
                if (counts.TryGetValue(pendingProduct, out var count) == false)
                {
                    counts.Add(pendingProduct, 1);
                }
                else
                {
                    counts[pendingProduct] = count + 1;
                }
            }
            
            List<PurchaseProduct> productsToRemove = new List<PurchaseProduct>(8);

            foreach (var itemCounts in counts)
            {
                if (_pendingProductsDictionary.TryGetValue(itemCounts.Key, out var list) == false)
                {

                    for (int i = 0; i < itemCounts.Value; i++)
                    {
                        AddPendingPurchaseInternal(string.Empty, itemCounts.Key);
                    }


                    continue;
                }
                

                if (list.Count > itemCounts.Value)
                {
                    productsToRemove.AddRange(list.GetRange(itemCounts.Value, list.Count - itemCounts.Value));
                    list.RemoveRange(itemCounts.Value, list.Count - itemCounts.Value);
                    

                    continue;
                }

                if (list.Count < itemCounts.Value)
                {
                    var diff = itemCounts.Value - list.Count;

                    for (int i = 0; i < diff; i++)
                    {
                        AddPendingPurchaseInternal(string.Empty, itemCounts.Key);
                    }


                    continue;
                }
            }

            foreach (var pendingProduct in _pendingProductsDictionary)
            {
                if (counts.TryGetValue(pendingProduct.Key, out var count))
                    continue;
                
                productsToRemove.AddRange(pendingProduct.Value);
                pendingProduct.Value.Clear();
            }
            
            
            foreach (var product in productsToRemove)
            {
                _pendingProducts.Remove(product);
            }
            

            
            _storageService.Save<IPurchaseService>();
        }
    }
}