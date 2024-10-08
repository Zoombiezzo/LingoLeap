namespace _Client.Scripts.Infrastructure.Services.RewardsManagement
{
    public interface IReward
    {
        RewardType Type { get; }
        int Count { get; }
        string IconId { get; }
#if UNITY_EDITOR
        string TypeName { get; }
#endif
    }
}