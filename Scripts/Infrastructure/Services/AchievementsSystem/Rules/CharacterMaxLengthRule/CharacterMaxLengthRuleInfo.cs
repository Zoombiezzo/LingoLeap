using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterMaxLengthRule
{
    public class CharacterMaxLengthRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private int[] _counts;
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(CharacterMaxLengthRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new CharacterMaxLengthRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description));
    }
}