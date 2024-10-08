using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public interface ILocationConfig
    {
        string Id { get; }
        Sprite Icon { get; }
        Color BlockedColor { get; }
        LocationPreview PreviewPrefab { get; }
        int RequiredCountLevels { get; }
        PictureReference PictureReference { get; }
        IRewardInfo RewardInfo { get; }
    }
}