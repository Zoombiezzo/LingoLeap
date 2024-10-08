using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.BankService.Views
{
    public class MultipleBankItemView : BankItemView
    {
        [SerializeField] private Image _iconCurrency;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private CanvasGroup _canvasGroupHeader;
        [SerializeField] private AnimationButton _buttonBuy;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ItemView _itemPrefab;
        
        private List<ItemView> _itemViews = new(8);
        
        private IDisposable _disposable;
        private ILocalizationService _localizationService;

        public Image IconCurrency => _iconCurrency;
        public ItemView ItemPrefab => _itemPrefab;
        public TMP_Text Price => _price;
        public TMP_Text HeaderText => _headerText;
        public CanvasGroup CanvasGroupHeader => _canvasGroupHeader;
        public RectTransform Content => _content;

        public override event Action<IBankItemView> OnBuy;

        public override void Initialize(IBankItem item)
        {
            base.Initialize(item);
            
            _headerText.text = _localizationService.GetValue(_bankItem.Name);

            Subscribe();
        }

        public void RegisterLocalization(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public void AddItemView(ItemView item)
        {
            _itemViews.Add(item);
        }

        private void Subscribe()
        {
            var builder = Disposable.CreateBuilder();
            
            _buttonBuy.OnClick.AsObservable().Subscribe(OnClickBuy).AddTo(ref builder);

            Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h,
                h => _localizationService.OnLanguageChanged -= h).Subscribe(OnLanguageChanged).AddTo(ref builder);

            _disposable = builder.Build();
        }
        
        private void OnLanguageChanged(string _)
        {
            _headerText.text = _localizationService.GetValue(_bankItem.Name);
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