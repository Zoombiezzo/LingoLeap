using _Client.Scripts.Infrastructure.Services.AdvertisingService;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class AdsCurrencyProvider : IPurchaseProvider
    {
        private readonly IAdvertisingService _advertisingService;

        private IPurchaseReceiver _receiver;
        private readonly AdsReceiver _advertisingReceiver;

        public AdsCurrencyProvider(IAdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
            _advertisingReceiver = new AdsReceiver(this);
        }

        public void Execute(IPurchaseData purchaseData, IPurchaseReceiver receiver)
        {
            if (purchaseData is AdsData adsData == false)
            {
                receiver.FailPurchase(new MessageFailData("Wrong purchase data"));
                return;
            }

            _receiver = receiver;
            _advertisingService.ShowRewarded(_advertisingReceiver);
        }
        
        
        private void OnSuccessShowed()
        {
            _receiver?.SuccessPurchase();
        }
        
        private void OnFailShowed()
        {
            _receiver?.FailPurchase(new MessageFailData("Purchase failed"));
        }
        
        private class AdsReceiver : IAdvertisingReceiver
        {
            private readonly AdsCurrencyProvider _provider;

            public AdsReceiver(AdsCurrencyProvider provider)
            {
                _provider = provider;
            }

            public void AdsShowed()
            {
                
            }

            public void SuccessShowed()
            {
                _provider.OnSuccessShowed();
            }

            public void FailShowed()
            {
                _provider.OnFailShowed();
            }
        }
    }
}