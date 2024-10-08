using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService.PurchaseDataFactories
{
    public class GameCurrencyDataPurchaseFactory : IPurchaseDataFactory
    {
        public IPurchaseData Create(ISpinSetting item)
        {
            return new CurrencyData()
            {
                CurrencyType = item.Currency,
                Count = (int)item.Price
            };
        }
    }
}