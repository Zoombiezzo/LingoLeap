using System;
using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.GameLoop.Screens.Reward
{
    public class PurchaseItemReceiver : IPurchaseReceiver
    {
        private readonly Action<bool> _callback;

        public PurchaseItemReceiver(Action<bool> callback = null)
        {
            _callback = callback;
        }
        
        public void SuccessPurchase()
        {
            _callback?.Invoke(true);
        }

        public void FailPurchase(IFailPurchaseData data)
        {
            _callback?.Invoke(false);
        }
    }
}