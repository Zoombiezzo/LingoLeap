using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.BankService.CategoryViews;
using _Client.Scripts.Infrastructure.Services.BankService.Factory.CategoryFactories;
using _Client.Scripts.Infrastructure.Services.BankService.Factory.ViewFactories;
using _Client.Scripts.Infrastructure.Services.BankService.Views;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory
{
    public class BankFactory : IBankFactory
    {
        private Dictionary<Type, IBankItemFactory> _itemsFactories;
        private Dictionary<Type, IBankCategoryFactory> _categoriesFactories;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IPurchaseService _purchaseService;
        private readonly ILocalizationService _localizationService;

        public BankFactory(ISpriteDatabaseService spriteDatabaseService, IPurchaseService purchaseService, ILocalizationService localizationService)
        {
            _spriteDatabaseService = spriteDatabaseService;
            _purchaseService = purchaseService;
            _localizationService = localizationService;

            _itemsFactories = new Dictionary<Type, IBankItemFactory>()
            {
                { typeof(HorizontalSingleBankItemView), new HorizontalSingleBankItemViewFactory(_spriteDatabaseService, _purchaseService) },
                { typeof(GridSingleBankItemView), new GridSingleBankItemViewFactory(_spriteDatabaseService, _purchaseService) },
                { typeof(MultipleBankItemView), new MultipleBankItemViewFactory(_spriteDatabaseService, _purchaseService, _localizationService) },
            };

            _categoriesFactories = new Dictionary<Type, IBankCategoryFactory>()
            {
                { typeof(VerticalCategoryView), new VerticalCategoryFactory() },
                { typeof(GridCategoryView), new GridCategoryFactory() },
            };
        }

        public IBankItemView Create(Transform parent, IBankItemView prefab, IBankItem item)
        {
            if (_itemsFactories.TryGetValue(prefab.GetType(), out var factory) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[BankFactory]:  Item factory not found for type {prefab.GetType()}");
#endif

                return null;
            }

            return factory.Create(parent, prefab, item);
        }

        public ICategoryView Create(Transform parent, ICategoryView prefab, IBankCategory category)
        {
            if (_categoriesFactories.TryGetValue(prefab.GetType(), out var factory) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[BankFactory]:  Category factory not found for type {prefab.GetType()}");
#endif

                return null;
            }
            
            return factory.Create(parent, prefab, category);
        }
    }
}