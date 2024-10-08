using System.Globalization;
using _Client.Scripts.Infrastructure.Services.BankService.Views;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.BankService.Factory.ViewFactories
{
    public class GridSingleBankItemViewFactory : IBankItemFactory
    {
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IPurchaseService _purchaseService;

        public GridSingleBankItemViewFactory(ISpriteDatabaseService spriteDatabaseService, IPurchaseService purchaseService)
        {
            _spriteDatabaseService = spriteDatabaseService;
            _purchaseService = purchaseService;
        }
        
        public IBankItemView Create(Transform parent, IBankItemView prefab, IBankItem item)
        {
            if (prefab is not BankItemView bankItemViewPrefab)
                return null;
            
            var view = Object.Instantiate(bankItemViewPrefab, parent);
            
            view.Initialize(item);
            
            if (view is GridSingleBankItemView targetView == false)
                return view;
            
            targetView.Icon.sprite = _spriteDatabaseService.GetSprite(item.SpriteId);
            
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
            if (rewards.Count == 0)
                return targetView;
            
            var reward = rewards[0];
            targetView.Count.text = reward.Count.ToString();

            return view;
        }
    }
}