using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AdditionalWordsRule
{
    public class AdditionalWordsRuleUpdater : AchievementUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private readonly IAdditionalWordsData _additionalWordsData;

        public AdditionalWordsRuleUpdater(
            IAchievementService achievementService,
            IAdditionalWordsData additionalWordsData)
        {
            _achievementService = achievementService;
            _additionalWordsData = additionalWordsData;
        }
        
        public override void Enable()
        {
            _additionalWordsData.OnWordOpened += OnWordOpened;
        }

        public override void Disable()
        {
            _additionalWordsData.OnWordOpened -= OnWordOpened;
        }

        private void OnWordOpened(string word)
        {
            _recordsToUpdate.Clear();
            var rules = _achievementService.GetRules<AdditionalWordsRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as AdditionalWordsRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }
        
        private bool TryUpdate(AdditionalWordsRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Add(1);
            return true;
        }
    }
}