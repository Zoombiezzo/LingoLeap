using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public interface IPurchaseService : IService, IStorable
    {
        public string IconId { get; }
        public string CurrencyCode { get; }
        public IReadOnlyList<PurchaseProduct> PendingProducts { get; }
        void Initialize();
        string GetPrice(string id);
        void Purchase(IPurchaseData data, IPurchaseReceiver receiver);
        Task Consume(PurchaseProduct product, IPurchaseReceiver receiver);
        void AddProduct(string id, ProductType productType);
        void AddPendingPurchase(string id, string storeId);
        void RemovePendingPurchase(string id, string storeId);
    }
}