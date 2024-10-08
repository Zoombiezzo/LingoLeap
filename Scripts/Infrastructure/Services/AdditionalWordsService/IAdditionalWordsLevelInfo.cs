using _Client.Scripts.Infrastructure.Services.RewardsManagement;

namespace _Client.Scripts.Infrastructure.Services.AdditionalWordsService
{
    public interface IAdditionalWordsLevelInfo
    {
        int RequiredWordsCount { get; }
        RewardInfo Reward { get; }
    }
}