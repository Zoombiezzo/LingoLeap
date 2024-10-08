using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.BankService.PurchaseDataFactories
{
    public class GameCurrencyDataPurchaseFactory : IBankPurchaseDataFactory
    {
        public IPurchaseData Create(IBankItem item)
        {
            return new CurrencyData()
            {
                CurrencyType = item.Currency,
                Count = (int)item.Price
            };
        }
    }
}