namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class CurrencyData : IPurchaseData
    {
        public CurrencyType CurrencyType { get; set; }
        public int Count { get; set; }
        
        
    }
}