using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.RewardsManagement.Types.Currency;
using _Client.Scripts.Infrastructure.Services.WalletService;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    public class RewardService : IRewardService
    {
        private readonly Dictionary<RewardType, IRewardProvider> _providers;
        private readonly IWalletService _walletService;
        public event Action<IReward> OnShowScreenReward;
        public event Action<IRewardInfo> OnShowScreenRewardInfo;
        public event Action<IReadOnlyList<IRewardInfo>> OnShowScreenRewardInfoList;
        
        public bool MultiplierRewardAvailable => _multiplierRewardEnabled;
        public int MultiplierRewardValue => _multiplierRewardValue;
        
        private bool _multiplierRewardEnabled;
        private int _multiplierRewardValue;

        public RewardService(IWalletService walletService)
        {
            _walletService = walletService;
            _providers = new Dictionary<RewardType, IRewardProvider>()
            {
                { RewardType.Currency, new CurrencyRewardProvider(_walletService) },
            };
        }

        public void SetAvailableMultipleReward(int multiplier)
        {
            _multiplierRewardEnabled = true;
            _multiplierRewardValue = multiplier;
        }

        public void DisableMultipleReward()
        {
            _multiplierRewardEnabled = false;
            _multiplierRewardValue = 1;
        }

        public bool TryCollectReward(IRewardInfo rewardInfo)
        {
            foreach (var reward in rewardInfo.Rewards)
            {
                if (_providers.TryGetValue(reward.Type, out var provider) == false)
                    return false;
                
                if (provider.IsPossibleGetReward(reward, out var error) == false)
                    return false;
            }
            
            foreach (var reward in rewardInfo.Rewards)
            {
                if (_providers.TryGetValue(reward.Type, out var provider) == false)
                    return false;
                
                if (provider.GetReward(reward, out var error) == false)
                    return false;
            }
            
            return true;
        }
        
        public bool TryCollectReward(IReward reward)
        {
            if (_providers.TryGetValue(reward.Type, out var provider) == false)
                return false;
                
            if (provider.IsPossibleGetReward(reward, out var error) == false)
                return false;
            
            return provider.GetReward(reward, out error);
        }

        public void ShowScreenReward(IReward reward)
        {
            OnShowScreenReward?.Invoke(reward);
        }

        public void ShowScreenReward(IRewardInfo reward)
        {
            OnShowScreenRewardInfo?.Invoke(reward);
        }

        public void ShowScreenReward(IReadOnlyList<IRewardInfo> rewards)
        {
            OnShowScreenRewardInfoList?.Invoke(rewards);
        }
    }
}