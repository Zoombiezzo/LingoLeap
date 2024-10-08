using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.MindScoreRule
{
    public class MindScoreRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private int[] _counts;
        
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(MindScoreRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new MindScoreRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description));
    }
}