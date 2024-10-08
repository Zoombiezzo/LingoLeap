using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterLengthRule
{
    public class CharacterLengthRuleUpdater : AchievementUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IAchievementService _achievementService;
        private readonly List<CharacterLengthRuleRecord> _recordsToUpdate = new(8);
        private readonly IAdditionalWordsData _additionalWordsData;

        public CharacterLengthRuleUpdater(
            IAchievementService achievementService,
            ILevelProgressData levelProgressData,
            IAdditionalWordsData additionalWordsData)
        {
            _levelProgressData = levelProgressData;
            _achievementService = achievementService;
            _additionalWordsData = additionalWordsData;
        }
        
        public override void Enable()
        {
            _levelProgressData.OnWordOpened += OnWordOpened;
            _additionalWordsData.OnWordOpened += OnWordOpened;
        }

        public override void Disable()
        {
            _levelProgressData.OnWordOpened -= OnWordOpened;
            _additionalWordsData.OnWordOpened -= OnWordOpened;
        }

        private void OnWordOpened(string word)
        {
            _recordsToUpdate.Clear();
            var rules = _achievementService.GetRules<CharacterLengthRuleRecord>();
            if (rules == null)
                return;
            
            var length = word.Length;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as CharacterLengthRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(length, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }
        
        private bool TryUpdate(int length, CharacterLengthRuleRecord record)
        {
            if (record == null)
                return false;
            
            if(record.Info.Length != length)
                return false;
            
            record.Add(1);
            return true;
        }
    }
}