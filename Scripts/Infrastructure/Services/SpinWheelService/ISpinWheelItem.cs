using _Client.Scripts.Infrastructure.Services.RewardsManagement;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    public interface ISpinWheelItem
    {
        string SpriteId { get; }
        string Text { get; }
        IRewardInfo Reward { get; }
        float Chance { get; }
    }
}