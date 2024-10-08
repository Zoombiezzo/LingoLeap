using _Client.Scripts.Infrastructure.Services.WalletService;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class BaseCurrencyProvider : IPurchaseProvider
    {
        private readonly IWalletService _walletService;

        public BaseCurrencyProvider(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public void Execute(IPurchaseData purchaseData, IPurchaseReceiver receiver)
        {
            var hardData = purchaseData as CurrencyData;
            
            if (hardData == null)
            {
                receiver.FailPurchase(new MessageFailData("Wrong purchase data"));
                return;
            }

            var isPurchased = _walletService.TryRemoveCurrency(hardData.CurrencyType, hardData.Count, out int diff);

            if (isPurchased)
            {
                receiver.SuccessPurchase();
            }
            else
            {
                receiver.FailPurchase(new NotEnoughCurrencyFailData(diff));
            }
        }
    }
}