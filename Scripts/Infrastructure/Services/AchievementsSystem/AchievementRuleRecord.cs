using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [Serializable]
    public abstract class AchievementRuleRecord
    {
        [JsonProperty] protected string _id;
        
        [JsonIgnore] public string Id => _id;
        [JsonIgnore] public string Description => _info.Description;
        [JsonIgnore] public string Title => _info.Title;
        [JsonIgnore] public bool IsComposite => _info.IsComposite;
        
        [JsonIgnore] public virtual IReadOnlyList<AchievementRuleRecord> Children => null;
        [JsonIgnore] public virtual float Progress => 0;
        [JsonIgnore] public virtual string ProgressText => "ProgressText";

        [JsonIgnore] private AchievementRuleInfo _info;
        [JsonIgnore] protected AchievementRecord _achievement;

        public AchievementRuleRecord()
        {
            
        }

        public AchievementRuleRecord(string id, AchievementRuleInfo info)
        {
            _id = id;
            _info = info;
            _info.Initialize();
        }

        public virtual void RegisterInfo(AchievementRuleInfo info)
        {
            _info = info;
            _info.Initialize();
        }
        
        public void RegisterAchievement(AchievementRecord record)
        {
            _achievement = record;
        }
    }
}