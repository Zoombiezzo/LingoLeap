using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar.PurchaseDataFactories
{
    public class GameCurrencyDataPurchaseFactory : IPurchaseDataFactory
    {
        public IPurchaseData Create(CurrencyType currencyType, int price) =>
            new CurrencyData()
            {
                CurrencyType = currencyType,
                Count = (int)price
            };
    }
}