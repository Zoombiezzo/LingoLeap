using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.LimitationService;

namespace _Client.Scripts.GameLoop.Screens.Shop
{
    public class PurchaseLimitationUpdater : LimitationUpdater
    {
        private readonly IBankService _bankService;

        public PurchaseLimitationUpdater(IBankService bankService)
        {
            _bankService = bankService;
            
            _bankService.OnPurchased += OnPurchased;
        }

        private void OnPurchased(IBankItem bankItem)
        {
            if (bankItem.IsLimitationOnCountPurchase)
            {
                foreach (var limitationRecord in _limitation)
                {
                    if (limitationRecord.Id == bankItem.Id)
                    {
                        ((CountLimitationRecord)limitationRecord).DecreaseValue(1);
                    }
                }
            }
        }
    }
}