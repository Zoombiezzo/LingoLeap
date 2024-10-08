using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Screens.PendingScreen;
using _Client.Scripts.Helpers;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Shop
{
    public class ShopPresenter : IStartable, IDisposable
    {
        private readonly IBankService _bankService;
        private readonly IPurchaseService _purchaseService;
        private readonly IRewardService _rewardService;

        private ShopWindow _window;
        private IDisposable _disposable;

        public ShopPresenter(IBankService bankService, IPurchaseService purchaseService, IRewardService rewardService)
        {
            _bankService = bankService;
            _purchaseService = purchaseService;
            _rewardService = rewardService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            
            CreateBank();
            
            var builder = Disposable.CreateBuilder();

            var container = _window.Container;

            Observable.FromEvent(h => _window.OnBeforeShow += h, h => _window.OnBeforeShow -= h).Subscribe(OnBeforeShow)
                .AddTo(ref builder);
            Observable.FromEvent<IBankItemView>(h => container.OnBuyItem += h, h => container.OnBuyItem -= h)
                .Subscribe(OnBuyItemHandler).AddTo(ref builder);
            
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnCloseClick).AddTo(ref builder);
            
            _disposable = builder.Build();

            RestorePurchases();
        }
        
        private void OnBeforeShow(Unit _)
        {
            var scrollRect = _window.Container.ScrollRect;
            var targetCategory = _window.TargetCategoryId;
            scrollRect.normalizedPosition = new Vector2(0, 1f);

            if (targetCategory == null)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1f);
                return;
            }

            if (_window.Container.TryGetCategoryView(targetCategory, out var targetCategoryView) == false)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1f);
                return;
            }
            
            scrollRect.ScrollToElement(targetCategoryView.RectTransform, new Vector2(0f, 0f));

            _window.SetTargetCategoryId(string.Empty);
        }
        
        private void OnCloseClick(Unit _)
        {
            _window.Hide();
        }
        
        private async void RestorePurchases()
        {
            var pendingProducts = new List<PurchaseProduct>(_purchaseService.PendingProducts);
            
            foreach (var pendingProduct in pendingProducts)
            {
                await _bankService.Consume(pendingProduct);
            }
        }
        
        private void OnBuyItemHandler(IBankItemView view)
        {
            WindowsService.Show<PendingWindow>();
            
            _bankService.Purchase(view.Item.Id, OnPurchaseHandler);
        }

        private void OnPurchaseHandler(BankItem bankItem, bool isSuccess)
        {
            WindowsService.Hide<PendingWindow>();

            if (isSuccess == false)
                return;
            
            _rewardService.ShowScreenReward(bankItem.Reward);
        }

        private void CreateBank()
        {
            var container = _window.Container;

            container.Create(BankCategoryType.Main, _bankService.GetBankConfig("main"));
        }

        public void Dispose()
        {
            _window.Container.Dispose();
            _disposable?.Dispose();
        }
    }
}