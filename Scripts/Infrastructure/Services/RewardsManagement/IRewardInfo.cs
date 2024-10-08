using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    public interface IRewardInfo
    {
        List<IReward> Rewards { get; }
    }
}