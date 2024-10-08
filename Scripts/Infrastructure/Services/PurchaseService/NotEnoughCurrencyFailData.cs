namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class NotEnoughCurrencyFailData : IFailPurchaseData
    {
        public FailPurchaseType FailType => FailPurchaseType.NotEnoughCurrency;
        public int Count { get; }
 
        public NotEnoughCurrencyFailData(int count)
        {
            Count = count;
        }
    }
}