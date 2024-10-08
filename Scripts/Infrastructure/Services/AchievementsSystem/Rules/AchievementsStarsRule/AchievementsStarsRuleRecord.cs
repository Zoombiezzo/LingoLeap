using System;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AchievementsStarsRule
{
    public class AchievementsStarsRuleRecord : AchievementRuleRecord
    {
        [JsonProperty] private int _count;
        [JsonIgnore] private AchievementsStarsRuleInfo _info;

        [JsonIgnore] public override float Progress => CalculateProgress();
        [JsonIgnore] public override string ProgressText => GetProgressString(); 
        
        [JsonIgnore] public AchievementsStarsRuleInfo Info => _info;

        public AchievementsStarsRuleRecord()
        {
            
        }
        
        public AchievementsStarsRuleRecord(string id, AchievementsStarsRuleInfo info) : base(id, info)
        {
            _info = info;
            _count = 0;
        }
        
        public void Set(int count)
        {
            if (count < 0)
            {
                throw new Exception("Count must be >= 0");
            }
            
            _count = count;
        }
        
        public override void RegisterInfo(AchievementRuleInfo info)
        {
            base.RegisterInfo(info);
            
            if (info is AchievementsStarsRuleInfo == false)
            {
                throw new System.Exception("AchievementInfo is not AchievementsStarsRuleInfo");
                return;
            }
            
            _info = (AchievementsStarsRuleInfo) info;
        }

        private int GetNeedCounts()
        {
            var stage = _achievement.Stage;
            var counts = _info.Counts;
            stage = Mathf.Min(stage, counts.Length - 1);
            return counts[stage];
        }

        private float CalculateProgress() => (float)_count / GetNeedCounts();

        private string GetProgressString()
        {
            var needCounts = GetNeedCounts();
            var count = _count;
            
            return $"{count}/{needCounts}";
        }
    }
}