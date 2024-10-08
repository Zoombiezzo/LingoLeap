using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [CreateAssetMenu(fileName = "AchievementInfo", menuName = "Achievements/AchievementInfo", order = 0)]
    [System.Serializable]
    public class AchievementInfo : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _description;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _title;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _completedProgressText;
        [SerializeField] [ValueDropdown("SpritesDropdown")]
        private string _iconId;
        [SerializeField] private Color _color;
        [SerializeField] private int _maxStage;
        [SerializeField] private float _notificationStep;
        [SerializeField] private List<RewardInfo> _rewards;
        [SerializeReference] private AchievementRuleInfo _rule;
        
        public string Id => _id;
        public string Description => _description;
        public string Title => _title;
        public string CompletedProgressText => _completedProgressText;
        public string IconId => _iconId;
        public Color Color => _color;
        public int MaxStage => _maxStage;
        public float NotificationStep => _notificationStep;
        public IReadOnlyList<IRewardInfo> Rewards => _rewards;
        public AchievementRuleInfo Rule => _rule;

        public AchievementRecord CreateRecord()
        {
            return new AchievementRecord(_id, this);
        }

        public string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description), _rule.GetDescription(localizationService));

#if UNITY_EDITOR
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>();
            var configs = AssetHelper<SpritesPreset>.GetAsset("Assets");
            foreach (var spritesPreset in configs)
            {
                foreach (var sprites in spritesPreset.SpritePresets)
                {
                    spritesIds.Add(spritesPreset.GetIdSprite(sprites.Id));
                }
            }

            return spritesIds;
        }
#endif
    }
}