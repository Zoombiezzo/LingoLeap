using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AchievementsStarsRule
{
    public class AchievementsStarsRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private int[] _counts;
        
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(AchievementsStarsRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new AchievementsStarsRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description));
    }
}