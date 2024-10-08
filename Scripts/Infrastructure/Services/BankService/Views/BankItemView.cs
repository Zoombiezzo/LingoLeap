using System;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Views
{
    public abstract class BankItemView : MonoBehaviour, IBankItemView
    {
        public IBankItem Item => _bankItem;
        public abstract event Action<IBankItemView> OnBuy;
        protected IBankItem _bankItem;
        public virtual void Initialize(IBankItem item)
        {
            _bankItem = item;
        }
    }
}