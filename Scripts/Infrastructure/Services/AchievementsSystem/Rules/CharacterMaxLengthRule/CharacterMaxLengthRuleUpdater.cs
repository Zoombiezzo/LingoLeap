using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterMaxLengthRule
{
    public class CharacterMaxLengthRuleUpdater : AchievementUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IAchievementService _achievementService;
        private readonly IWordsLevelsService _wordsLevelsService;
        private readonly IAdditionalWordsData _additionalWordsData;

        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);

        public CharacterMaxLengthRuleUpdater(
            IAchievementService achievementService,
            ILevelProgressData levelProgressData,
            IWordsLevelsService wordsLevelsService,
            IAdditionalWordsData additionalWordsData)
        {
            _levelProgressData = levelProgressData;
            _achievementService = achievementService;
            _wordsLevelsService = wordsLevelsService;
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
            var levelRecord = _levelProgressData.GetLevelRecord();

            if (_wordsLevelsService.TryGetLevel(levelRecord.LevelNumber, out var wordsLevel) == false) 
                return;
            
            if(word.Length != wordsLevel.Chars.Length)
                return;
            
            _recordsToUpdate.Clear();
            var rules = _achievementService.GetRules<CharacterMaxLengthRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                if (rule is not CharacterMaxLengthRuleRecord ruleRecord)
                    continue;
                
                if(TryUpdate(ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }
        
        private bool TryUpdate(CharacterMaxLengthRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Add(1);
            return true;
        }
    }
}