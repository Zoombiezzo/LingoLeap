using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [Serializable]
    public abstract class AchievementRuleInfo
    {
        [SerializeField] protected string _id;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _description = "";
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _title = "";
        
        public virtual string Id => _id;
        public virtual string Description => _description;
        public virtual string Title => _title;

        public virtual Type TypeRecord => typeof(AchievementRuleRecord);
        
        public virtual bool IsComposite => false;
        public virtual IReadOnlyList<AchievementRuleInfo> Children => ArraySegment<AchievementRuleInfo>.Empty;

        public AchievementRuleInfo()
        {
            
        }
        
        public AchievementRuleInfo(string id, string description, string title)
        {
            _id = id;
            _description = description;
            _title = title;
        }

        public virtual void Initialize() { }
        
        public abstract AchievementRuleRecord CreateRecord();

        public virtual string GetDescription(ILocalizationService localizationService) => 
            localizationService.GetValue(_description);
    }
}