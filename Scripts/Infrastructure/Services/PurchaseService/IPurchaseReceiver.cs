namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public interface IPurchaseReceiver
    {
        void SuccessPurchase();
        void FailPurchase(IFailPurchaseData data);
    }
}