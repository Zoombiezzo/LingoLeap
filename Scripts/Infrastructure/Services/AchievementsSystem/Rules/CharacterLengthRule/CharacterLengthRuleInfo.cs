using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterLengthRule
{
    public class CharacterLengthRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private int _length;
        [SerializeField] private int[] _counts;
        
        public int Length => _length;
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(CharacterLengthRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new CharacterLengthRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description), _length);
    }
}