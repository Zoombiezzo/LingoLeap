using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    [CreateAssetMenu(fileName = "LocationConfig", menuName = "Configs/Map/LocationConfig", order = 0)]
    public class LocationConfig : ScriptableObject, ILocationConfig
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _blockedColor;
        [SerializeField] private LocationPreview _previewPrefab;
        [SerializeField] private int _requiredCountLevels;
        [SerializeField] private PictureReference _pictureReference;
        [SerializeField] private RewardInfo _rewardInfo;
        
        public string Id => _id;
        public Sprite Icon => _icon;
        public Color BlockedColor => _blockedColor;
        public LocationPreview PreviewPrefab => _previewPrefab;
        public int RequiredCountLevels => _requiredCountLevels;
        public PictureReference PictureReference => _pictureReference;
        public IRewardInfo RewardInfo => _rewardInfo;
    }
}