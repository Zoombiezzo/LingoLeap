using System;
using _Client.Scripts.GameLoop.Components.Buttons;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.BankService.Views
{
    public class GridSingleBankItemView : BankItemView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconCurrency;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private AnimationButton _buttonBuy;
        
        private IDisposable _disposable;
        
        public Image Icon => _icon;
        public Image IconCurrency => _iconCurrency;
        public TMP_Text Count => _count;
        public TMP_Text Price => _price;

        public override event Action<IBankItemView> OnBuy;

        public override void Initialize(IBankItem item)
        {
            base.Initialize(item);

            Subscribe();
        }

        private void Subscribe()
        {
            var builder = Disposable.CreateBuilder();
            
            _buttonBuy.OnClick.AsObservable().Subscribe(OnClickBuy).AddTo(ref builder);

            _disposable = builder.Build();
        }
        
        private void OnClickBuy(Unit _)
        {
            OnBuy?.Invoke(this);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}