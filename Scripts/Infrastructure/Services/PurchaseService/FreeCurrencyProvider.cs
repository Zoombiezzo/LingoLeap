namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class FreeCurrencyProvider : IPurchaseProvider
    {
        public void Execute(IPurchaseData purchaseData, IPurchaseReceiver receiver)
        {
            receiver.SuccessPurchase();
        }
    }
}