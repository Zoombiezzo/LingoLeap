using System;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.WalletService;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public class PurchaseItemReceiver : IPurchaseReceiver
    {
        private BankItem _item;
        private readonly Action<bool> _callback;
        private readonly IWalletService _walletService;
        private readonly BankService _bankService;

        public PurchaseItemReceiver(BankService bankService, BankItem item, Action<bool> callback = null)
        {
            _bankService = bankService;
            _item = item;
            _callback = callback;
        }
        
        public void SuccessPurchase()
        {
            _bankService.CollectItem(_item);
            _callback?.Invoke(true);
        }

        public void FailPurchase(IFailPurchaseData data)
        {
            _callback?.Invoke(false);
        }
    }
}