using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.BankService.PurchaseDataFactories
{
    public class AdsDataPurchaseFactory : IBankPurchaseDataFactory
    {
        public IPurchaseData Create(IBankItem item)
        {
            return new AdsData()
            {
                CurrencyType = item.Currency,
            };
        }
    }
}