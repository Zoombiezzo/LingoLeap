using System;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    public interface IRewardProvider
    {
        bool IsPossibleGetReward(IReward rewardInfo, out string errorMessage);
        bool GetReward(IReward rewardInfo, out string errorMessage);
    }
}