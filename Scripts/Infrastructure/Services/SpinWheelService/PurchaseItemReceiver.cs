using System;
using _Client.Scripts.Infrastructure.Services.PurchaseService;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public class PurchaseItemReceiver : IPurchaseReceiver
    {
        private readonly ISpinSetting _item;
        private readonly Action<bool> _callback;
        private readonly SpinWheelService _spinWheelService;

        public PurchaseItemReceiver(SpinWheelService spinWheelService, ISpinSetting item, Action<bool> callback = null)
        {
            _spinWheelService = spinWheelService;
            _item = item;
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