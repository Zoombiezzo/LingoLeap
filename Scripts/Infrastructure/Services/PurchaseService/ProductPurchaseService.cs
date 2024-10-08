using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameSDK.Purchases;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class GameSDKProductPurchaseService : IProductPurchaseService
    {
        private string _icon;
        private string _currencyCode;
        
        private Dictionary<string, Product> _products = new();
        
        private Dictionary<string, List<ProductPurchase>> _purchasedProducts = new();
        private List<string> _purchasedProductsIds = new();
        
        public GameSDKProductPurchaseService()
        {
            _icon = "currency:currency_yan";
        }

        public string IconId => _icon;
        public string CurrencyCode => _currencyCode;
        public bool IsInitialized { get; private set; }
        public IReadOnlyList<string> PurchasedProductIds => _purchasedProductsIds;

        public async void Initialize()
        {
            IsInitialized = false;
            
            await Purchases.Initialize().AsUniTask();
            
            var (success, catalog) = await Purchases.GetCatalog().AsUniTask();
            if (success)
            {
                _products = catalog.ToDictionary(el => el.Id, el => el);
                
                if (catalog.Length > 0)
                {
                    _currencyCode = catalog[0].PriceCurrencyCode;
                }
            }
            
            var products = await Purchases.GetPurchases().AsUniTask();
            
            _purchasedProducts = new Dictionary<string, List<ProductPurchase>>(products.Length);
            
            foreach (var product in products)
            {
                if (_purchasedProducts.TryGetValue(product.Id, out var list) == false)
                {
                    list = new List<ProductPurchase>(5);
                    _purchasedProducts.Add(product.Id, list);
                }
                
                list.Add(product);
                _purchasedProductsIds.Add(product.Id);
            }

            IsInitialized = true;
        }

        public void AddProduct(string id, ProductType productType)
        {
            Purchases.AddProduct(id, productType switch
            {
                ProductType.Consumable => GameSDK.Purchases.ProductType.Consumable,
                ProductType.NonConsumables => GameSDK.Purchases.ProductType.NonConsumables,
                _ => GameSDK.Purchases.ProductType.None
            });
        }

        public string GetPrice(string id)
        {
            if (_products.TryGetValue(id, out var product))
            {
                return product.Price;
            }

            return "";
        }

        public async void Purchase(string id, Action<string, bool> callback = null)
        {
            
#if UNITY_EDITOR
            await Task.Delay(1000);
            
            //return;
#endif

            var (success, product) = await Purchases.Purchase(id);
            
            if(success)
            {
                await Purchases.Consume(product);
            }
            
            callback?.Invoke(id, success);
        }

        public async Task Consume(string id, Action<string, bool> callback = null)
        {
            if (_purchasedProducts.TryGetValue(id, out var list) == false)
            {
                callback?.Invoke(id, false);
                return;
            }

            if (list.Count == 0)
            {
                callback?.Invoke(id, false);
                return;
            }
            
            var item = list[0];
            list.RemoveAt(0);


            if (item.Type != GameSDK.Purchases.ProductType.Consumable)
            {
                callback?.Invoke(id, false);
                return;
            }

            await item.Consume();


            if (item.IsConsumed == false)
            {
                callback?.Invoke(id, false);
                return;
            }
            
            if (list.Count == 0)
            {
                _purchasedProducts.Remove(id);
                _purchasedProductsIds.Remove(id);
            }

            callback?.Invoke(id, true);
        }

        public void Dispose()
        {
            
        }
    }
}