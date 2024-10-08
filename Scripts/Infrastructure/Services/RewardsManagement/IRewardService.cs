using System;
using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    public interface IRewardService : IService
    {
        event Action<IReward> OnShowScreenReward;
        event Action<IRewardInfo> OnShowScreenRewardInfo;
        event Action<IReadOnlyList<IRewardInfo>> OnShowScreenRewardInfoList;
        bool MultiplierRewardAvailable { get; }
        int MultiplierRewardValue { get; }
        void SetAvailableMultipleReward(int multiplier);
        void DisableMultipleReward();
        bool TryCollectReward(IRewardInfo rewardInfo);
        bool TryCollectReward(IReward reward);
        void ShowScreenReward(IReward reward);
        void ShowScreenReward(IRewardInfo reward);
        void ShowScreenReward(IReadOnlyList<IRewardInfo> rewards);
    }
}