namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public interface IPurchaseProvider
    {
        void Execute(IPurchaseData purchaseData, IPurchaseReceiver receiver);
    }
}