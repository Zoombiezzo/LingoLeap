namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class ProductCurrencyProvider : IPurchaseProvider
    {
        private readonly IProductPurchaseService _productPurchaseService;
        private readonly IPurchaseService _purchaseService;

        public ProductCurrencyProvider(IPurchaseService purchaseService, IProductPurchaseService productPurchaseService)
        {
            _purchaseService = purchaseService;
            _productPurchaseService = productPurchaseService;
        }

        public void Execute(IPurchaseData purchaseData, IPurchaseReceiver receiver)
        {
            if (purchaseData is ProductData productData == false)
            {
                receiver.FailPurchase(new MessageFailData("Wrong purchase data"));
                return;
            }

            if (_productPurchaseService.IsInitialized == false)
            {
                receiver.FailPurchase(new MessageFailData("Not initialized"));
                return;
            }

            _purchaseService.AddPendingPurchase(productData.Id, productData.StoreId);
            
            _productPurchaseService.Purchase(productData.StoreId, (id, success) =>
            {
                _purchaseService.RemovePendingPurchase(productData.Id, productData.StoreId);

                if (success)
                {
                    receiver.SuccessPurchase();
                    return;
                }

                receiver.FailPurchase(new MessageFailData("Purchase failed"));

            });
        }
    }
}