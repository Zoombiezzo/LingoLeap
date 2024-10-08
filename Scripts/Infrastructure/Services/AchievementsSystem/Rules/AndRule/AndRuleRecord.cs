using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AndRule
{
    [Serializable]
    public class AndRuleRecord : AchievementRuleRecord
    {
        [SerializeReference, JsonProperty] private readonly List<AchievementRuleRecord> _rules;
        
        [JsonIgnore] public override IReadOnlyList<AchievementRuleRecord> Children => _rules;
        [JsonIgnore] public override float Progress => CalculateProgress();
        [JsonIgnore] public override string ProgressText => Progress.ToString("P0");
        
        [JsonIgnore] private AndRuleInfo _info;

        public AndRuleRecord()
        {
            
        }
        
        public AndRuleRecord(string id, List<AchievementRuleRecord> rules, AndRuleInfo info) : base(id, info)
        {
            _rules = rules;
            _info = info;
        }
        
        public override void RegisterInfo(AchievementRuleInfo info)
        {
            base.RegisterInfo(info);
            
            if (info is AndRuleInfo == false)
            {
                throw new System.Exception("AchievementInfo is not AndRuleInfo");
                return;
            }
            
            _info = (AndRuleInfo) info;
        }

        private float CalculateProgress()
        {
            float progress = 0;
            foreach (var rule in _rules)
            {
                progress += rule.Progress;
            }

            return progress / _rules.Count;
        }
    }
}