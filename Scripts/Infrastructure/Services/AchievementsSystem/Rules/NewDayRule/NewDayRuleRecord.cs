using System;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.NewDayRule
{
    public class NewDayRuleRecord : AchievementRuleRecord
    {
        [JsonProperty] private long _lastUpdate;
        [JsonProperty] private int _count;
        [JsonIgnore] private NewDayRuleInfo _info;

        [JsonIgnore] public override float Progress => CalculateProgress();
        [JsonIgnore] public override string ProgressText => GetProgressString(); 
        
        [JsonIgnore] public NewDayRuleInfo Info => _info;
        
        [JsonIgnore] public long LastUpdate => _lastUpdate;

        public NewDayRuleRecord()
        {
            
        }
        
        public NewDayRuleRecord(string id, NewDayRuleInfo info) : base(id, info)
        {
            _info = info;
            _count = 0;
            _lastUpdate = 0;
        }
        
        public void Add(int count)
        {
            if (count <= 0)
            {
                throw new Exception("Count must be > 0");
            }
            
            _count += count;
        }

        public void SetLastUpdate(long lastUpdate)
        {
            _lastUpdate = lastUpdate;
        }
        
        public override void RegisterInfo(AchievementRuleInfo info)
        {
            base.RegisterInfo(info);
            
            if (info is NewDayRuleInfo == false)
            {
                throw new System.Exception("AchievementInfo is not NewDayRuleInfo");
                return;
            }
            
            _info = (NewDayRuleInfo) info;
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