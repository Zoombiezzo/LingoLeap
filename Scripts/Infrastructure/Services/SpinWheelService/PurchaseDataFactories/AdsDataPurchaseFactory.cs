using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService.PurchaseDataFactories
{
    public class AdsDataPurchaseFactory : IPurchaseDataFactory
    {
        public IPurchaseData Create(ISpinSetting item)
        {
            return new AdsData()
            {
                CurrencyType = item.Currency,
            };
        }
    }
}