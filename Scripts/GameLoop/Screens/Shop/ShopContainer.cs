using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.BankService.Factory;
using _Client.Scripts.Infrastructure.Services.LimitationService;
using _Client.Scripts.Infrastructure.Services.RequirementService;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Client.Scripts.GameLoop.Screens.Shop
{
    public class ShopContainer : MonoBehaviour, IDisposable
    {
        private const string LimitationShopId = "Shop";

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _container;
        
        private readonly Dictionary<BankCategoryType, BankConfig> _configs = new(16);
        private readonly Dictionary<string, IBankItemView> _items = new(16);
        private readonly Dictionary<string, ICategoryView> _categories = new(16);
        private readonly Dictionary<string, ILimitation> _limitations = new(16);
        private readonly List<IBankItemView> _itemViews = new(16);
        private IBankFactory _bankFactory;
        private IRequirementService _requirementService;
        private ILimitationService _limitationService;
        private IBankService _bankService;

        private PurchaseLimitationUpdater _purchaseLimitationUpdater; 
        
        private List<IDisposable> _disposables = new List<IDisposable>(16);
        
        public event Action<IBankItemView> OnBuyItem;
        public ScrollRect ScrollRect => _scrollRect;

        [Inject]
        public void Construct(IBankService bankService, IBankFactory bankFactory, IRequirementService requirementService, ILimitationService limitationService)
        {
            _bankService = bankService;
            _bankFactory = bankFactory;
            _requirementService = requirementService;
            _limitationService = limitationService;

            _purchaseLimitationUpdater = new PurchaseLimitationUpdater(_bankService);
            _limitationService.RegisterUpdater(LimitationShopId, _purchaseLimitationUpdater);
        }
        
        public void Create(BankCategoryType type, BankConfig config)
        {
            if(config == null)
                return;
            
            if(_configs.TryAdd(type, config) == false)
            {
                return;
            }

            if(config.Categories == null)
                return;
            
            foreach (var category in config.Categories)
            {
                CreateCategory(_container, category);
            }
        }

        public bool TryGetCategoryView(string categoryId, out ICategoryView categoryView) => _categories.TryGetValue(categoryId, out categoryView);

        private void CreateCategory(Transform root, IBankCategory category)
        {
            var categoryView = _bankFactory.Create(root, category.View, category);
            
            _categories.TryAdd(category.Id, categoryView);

            if (category.Items != null)
            {
                var categoryItems = category.Items;
                
                foreach (var item in categoryItems)
                {
                    var itemView = _bankFactory.Create(categoryView.Content, item.View, item);

                    if (itemView == null)
                        continue;

                    _items.TryAdd(item.Id, itemView);
                    _itemViews.Add(itemView);

                    SubscribeItem(itemView);
                    RegisterRequirements(itemView, item);
                    RegisterLimiter(itemView, item);
                }
            }
            
            if (category.Categories != null)
            {
                foreach (var subCategory in category.Categories)
                {
                    CreateCategory(categoryView.Content, subCategory);
                }
            }
        }

        private void SubscribeItem(IBankItemView bankItemView)
        {
            var builder = Disposable.CreateBuilder();

            Observable.FromEvent<IBankItemView>(h => bankItemView.OnBuy += h, h => bankItemView.OnBuy -= h)
                .Subscribe(OnBuyItemHandler).AddTo(ref builder);
            
            _disposables.Add(builder.Build());
        }
        
        private void OnBuyItemHandler(IBankItemView bankItemView)
        {
            OnBuyItem?.Invoke(bankItemView);
        }
        
        private void RegisterRequirements(IBankItemView bankItemView, IBankItem item)
        {
            if (bankItemView is IRequirable requirementView == false)
                return;
            
            if (item.IsRequirementLevel)
                _requirementService.Register(requirementView, item.LevelNeedRequirement);
        }
        
        private void UnregisterRequirements(IBankItemView item)
        {
            if (item is IRequirable requirementView) 
                _requirementService.Unregister(requirementView);
        }

        private void RegisterLimiter(IBankItemView bankItemView, IBankItem item)
        {
            if(bankItemView is ILimiter limiter == false)
                return;

            if (item.IsLimitationOnCountPurchase)
            {
                if (_limitations.TryGetValue(item.Id, out var limitation) == false)
                {
                    limitation = new CountLimitation(item.CountPurchaseLimit);
                    _limitations.Add(item.Id, limitation);
                }
                            
                _limitationService.RegisterLimitation<PurchaseLimitationUpdater>(LimitationShopId, item.Id, limitation, limiter);
            }
        }
        
        private void UnregisterLimiter(IBankItemView item)
        {
            if (item is not ILimiter limiter)
                return;

            var bankItem = item.Item;

            if (bankItem.IsLimitationOnCountPurchase)
            {
                if (_limitations.TryGetValue(bankItem.Id, out var limitation))
                {
                    _limitationService.UnregisterLimitation<PurchaseLimitationUpdater>(LimitationShopId, bankItem.Id,
                        limitation, limiter);
                }
            }
        }
        
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }

            foreach (var item in _itemViews)
            {
                UnregisterRequirements(item);
                UnregisterLimiter(item);
            }
            
            if (_purchaseLimitationUpdater != null)
            {
                _limitationService.UnregisterUpdater(LimitationShopId, _purchaseLimitationUpdater);
            }
        }
    }
}