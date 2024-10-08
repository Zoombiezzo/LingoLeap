using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.RewardsManagement.Types.Currency;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Reward.RewardViewFactory
{
    public class CurrencyRewardViewFactory : IRewardViewFactory
    {
        private readonly RewardView _prefab;
        private readonly ISpriteDatabaseService _spriteDatabaseService;

        public CurrencyRewardViewFactory(RewardView prefab, ISpriteDatabaseService spriteDatabaseService)
        {
            _prefab = prefab;
            _spriteDatabaseService = spriteDatabaseService;
        }
        
        public RewardView Create(IReward reward, RectTransform parent)
        {
            if (reward is CurrencyReward currencyReward == false)
                return null;

            var view = Object.Instantiate(_prefab, parent);
            var sprite = _spriteDatabaseService.GetCurrencySprite(currencyReward.CurrencyType);
            view.Initialize(sprite, currencyReward.Count);
            return view;
        }
    }
}