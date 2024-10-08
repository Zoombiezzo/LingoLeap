using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.OrRule
{
    [Serializable]
    public class OrRuleRecord : AchievementRuleRecord
    {
        [SerializeReference, JsonProperty] private List<AchievementRuleRecord> _rules;
        
        [JsonIgnore] public override IReadOnlyList<AchievementRuleRecord> Children => _rules;
        [JsonIgnore] public override float Progress => CalculateProgress();
        [JsonIgnore] public override string ProgressText => Progress.ToString("P0");
        
        [JsonIgnore] private OrRuleInfo _info;

        public OrRuleRecord()
        {
            
        }
        public OrRuleRecord(string id, List<AchievementRuleRecord> rules, OrRuleInfo info) : base(id, info)
        {
            _rules = rules;
            _info = info;
        }
        
        public override void RegisterInfo(AchievementRuleInfo info)
        {
            base.RegisterInfo(info);
            
            if (info is OrRuleInfo == false)
            {
                throw new System.Exception("AchievementInfo is not OrRuleInfo");
                return;
            }
            
            _info = (OrRuleInfo) info;
        }

        private float CalculateProgress()
        {
            float progress = 0;
            foreach (var rule in _rules)
            {
                var ruleProgress = rule.Progress;
                progress = progress > ruleProgress ? progress : ruleProgress;
            }

            return progress;
        }
    }
}