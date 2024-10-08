using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using R3;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.LevelCompleteRule
{
    public class LevelCompleteRuleUpdater : AchievementUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private IDisposable _disposable;

        public LevelCompleteRuleUpdater(
            IAchievementService achievementService,
            ILevelProgressData levelProgressData)
        {
            _levelProgressData = levelProgressData;
            _achievementService = achievementService;
        }
        
        public override void Enable()
        {
            var builder = Disposable.CreateBuilder();
            Observable.FromEvent<int>(h => _levelProgressData.OnLevelCompleted += h, h => _levelProgressData.OnLevelCompleted -= h)
                .Subscribe(OnLevelComplete).AddTo(ref builder);
            _disposable = builder.Build();
        }

        public override void Disable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnLevelComplete(int level)
        {
            OnValueChanged();
        }
        
        private void OnValueChanged()
        {
            var rules = _achievementService.GetRules<LevelCompleteRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                if (rule is not LevelCompleteRuleRecord ruleRecord)
                    continue;
                
                if(TryUpdate(1, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }

        private bool TryUpdate(int count, LevelCompleteRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Add(count);
            return true;
        }
    }
}