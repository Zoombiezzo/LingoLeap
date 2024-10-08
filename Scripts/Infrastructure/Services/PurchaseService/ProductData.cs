namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class ProductData : IPurchaseData
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public CurrencyType CurrencyType { get; set; }
    }
}