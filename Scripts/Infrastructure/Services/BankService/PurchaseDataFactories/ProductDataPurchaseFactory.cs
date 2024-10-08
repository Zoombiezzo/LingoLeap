using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.BankService.PurchaseDataFactories
{
    public class ProductDataPurchaseFactory : IBankPurchaseDataFactory
    {
        public IPurchaseData Create(IBankItem item)
        {
            return new ProductData()
            {
                Id = item.Id,
                StoreId = item.ProductId,
                CurrencyType = item.Currency,
            };
        }
    }
}