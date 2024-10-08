using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Reward.RewardViewFactory
{
    public interface IRewardViewFactory
    {
        RewardView Create(IReward reward, RectTransform parent);
    }
}