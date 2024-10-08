using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService.PurchaseDataFactories
{
    public class ProductDataPurchaseFactory : IPurchaseDataFactory
    {
        public IPurchaseData Create(ISpinSetting item)
        {
            return new ProductData()
            {
                Id = "spin",
                StoreId = "",
                CurrencyType = item.Currency,
            };
        }
    }
}