using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public interface IPurchaseDataFactory
    {
        IPurchaseData Create(CurrencyType currencyType, int price);
    }
}