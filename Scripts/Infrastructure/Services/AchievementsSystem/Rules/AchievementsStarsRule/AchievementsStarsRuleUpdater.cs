using System;
using System.Collections.Generic;
using R3;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AchievementsStarsRule
{
    public class AchievementsStarsRuleUpdater : AchievementUpdater
    {
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private IDisposable _disposable;

        public AchievementsStarsRuleUpdater(
            IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }
        
        public override void Enable()
        {
            var builder = Disposable.CreateBuilder();
            Observable.FromEvent<IAchievementRecord>(h => _achievementService.OnAchievementStageChanged += h, h => _achievementService.OnAchievementStageChanged -= h)
                .Subscribe(OnAchievementStageChanged).AddTo(ref builder);
            _disposable = builder.Build();

            OnValueChanged(_achievementService.CurrentStages);
        }

        public override void Disable()
        {
            _disposable?.Dispose();
        }
        
        private void OnAchievementStageChanged(IAchievementRecord record)
        {
            OnValueChanged(_achievementService.CurrentStages);
        }
        
        private void OnValueChanged(int value)
        {
            var rules = _achievementService.GetRules<AchievementsStarsRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as AchievementsStarsRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(value, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }

        private bool TryUpdate(int count, AchievementsStarsRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Set(count);
            return true;
        }
    }
}