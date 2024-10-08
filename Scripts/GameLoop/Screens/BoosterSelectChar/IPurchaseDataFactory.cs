using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar
{
    public interface IPurchaseDataFactory
    {
        IPurchaseData Create(CurrencyType currencyType, int price);
    }
}