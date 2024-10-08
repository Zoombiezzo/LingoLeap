using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public interface IProductPurchaseService: IService, IDisposable
    {
        public string IconId { get; }
        public string CurrencyCode { get; }
        public bool IsInitialized { get; }
        public IReadOnlyList<string> PurchasedProductIds { get; }
        void Initialize();
        void AddProduct(string id, ProductType productType);
        string GetPrice(string id);
        void Purchase(string id, Action<string, bool> callback = null);
        Task Consume(string id, Action<string, bool> callback = null);
    }
}