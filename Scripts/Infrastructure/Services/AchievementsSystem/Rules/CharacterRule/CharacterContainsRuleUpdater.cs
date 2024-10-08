using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.Infrastructure.Services.LocalizationService;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterRule
{
    public class CharacterContainsRuleUpdater : AchievementUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IAchievementService _achievementService;
        private readonly ILocalizationService _localizationService;
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly List<CharacterContainsRuleRecord> _recordsToUpdate = new List<CharacterContainsRuleRecord>();

        public CharacterContainsRuleUpdater(
            IAchievementService achievementService,
            ILevelProgressData levelProgressData,
            ILocalizationService localizationService,
            IAdditionalWordsData additionalWordsData)
        {
            _levelProgressData = levelProgressData;
            _achievementService = achievementService;
            _localizationService = localizationService;
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
            var rules = _achievementService.GetRules<CharacterContainsRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as CharacterContainsRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(word, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }
        
        private bool TryUpdate(string word, CharacterContainsRuleRecord record)
        {
            if (record == null)
                return false;
            
            var character = record.Info.GetCharacter(_localizationService.CurrentLanguageCode);
            
            if(word.Contains(character) == false)
                return false;
            
            record.Add(1);
            return true;
        }
    }
}