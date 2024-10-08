using System.Collections.Generic;
using System.Globalization;
using _Client.Scripts.Infrastructure.Services.BankService.Views;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.RewardsManagement.Types.Currency;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory.ViewFactories
{
    public class MultipleBankItemViewFactory : IBankItemFactory
    {
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IPurchaseService _purchaseService;
        private readonly ILocalizationService _localizationService;
        private readonly Dictionary<RewardType, IItemViewFactory> _rewardsFactory;

        public MultipleBankItemViewFactory(ISpriteDatabaseService spriteDatabaseService,
            IPurchaseService purchaseService, ILocalizationService localizationService)
        {
            _spriteDatabaseService = spriteDatabaseService;
            _purchaseService = purchaseService;
            _localizationService = localizationService;

            _rewardsFactory = new Dictionary<RewardType, IItemViewFactory>()
            {
                { RewardType.Currency, new CurrencyRewardViewFactory(_spriteDatabaseService) }
            };
        }

        public IBankItemView Create(Transform parent, IBankItemView prefab, IBankItem item)
        {
            if (prefab is not BankItemView bankItemViewPrefab)
                return null;
            
            var view = Object.Instantiate(bankItemViewPrefab, parent);
            
            if (view is MultipleBankItemView targetView == false)
                return view;

            targetView.RegisterLocalization(_localizationService);
            view.Initialize(item);

            targetView.IconCurrency.gameObject.SetActive(item.Currency != CurrencyType.Real);
            targetView.Price.gameObject.SetActive(item.Currency != CurrencyType.Ads);
            targetView.Price.text = item.Price.ToString(CultureInfo.InvariantCulture);
            
            if (item.Currency == CurrencyType.Real)
            {
                var price = _purchaseService.GetPrice(item.ProductId);
                
                if (string.IsNullOrEmpty(_purchaseService.IconId) == false)
                {
                    targetView.IconCurrency.sprite = _spriteDatabaseService.GetSprite(_purchaseService.IconId);
                    targetView.Price.text = string.IsNullOrEmpty(price) ? targetView.Price.text : price;
                }
                else
                {
                    targetView.Price.text =
                        $"{(string.IsNullOrEmpty(price) ? targetView.Price.text : price)} {_purchaseService.CurrencyCode}";
                }
            }
            else
            {
                targetView.IconCurrency.sprite = _spriteDatabaseService.GetCurrencySprite(item.Currency);
            }
            
            var rewards = item.Reward.Rewards;

            foreach (var reward in rewards)
            {
                if (_rewardsFactory.TryGetValue(reward.Type, out var factory) == false)
                    continue;

                var itemView = factory.Create(reward, targetView.ItemPrefab, targetView.Content);

                if (itemView == null)
                    continue;
                
                targetView.AddItemView(itemView);
            }

            targetView.CanvasGroupHeader.alpha = string.IsNullOrEmpty(item.Name) ? 0f : 1f;

            return view;
        }
        
        private class CurrencyRewardViewFactory : IItemViewFactory
        {
            private readonly ISpriteDatabaseService _spriteDatabaseService;

            public CurrencyRewardViewFactory(ISpriteDatabaseService spriteDatabaseService)
            {
                _spriteDatabaseService = spriteDatabaseService;
            }

            public ItemView Create(IReward reward, ItemView prefab, RectTransform parent)
            {
                if (reward is CurrencyReward currencyReward == false)
                    return null;

                var view = Object.Instantiate(prefab, parent);
                var sprite = string.IsNullOrEmpty(currencyReward.IconId)
                    ? _spriteDatabaseService.GetCurrencySprite(currencyReward.CurrencyType)
                    : _spriteDatabaseService.GetSprite(currencyReward.IconId);
                view.Image.sprite = sprite;
                view.Text.text = currencyReward.Count.ToString();
                return view;
            }
        }
        
        private interface IItemViewFactory
        {
            ItemView Create(IReward reward, ItemView prefab, RectTransform parent);
        }
        
    }
}