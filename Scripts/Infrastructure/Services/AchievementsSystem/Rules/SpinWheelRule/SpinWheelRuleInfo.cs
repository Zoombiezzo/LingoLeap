using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.SpinWheelRule
{
    public class SpinWheelRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private int[] _counts;
        
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(SpinWheelRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new SpinWheelRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description));
    }
}