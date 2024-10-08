using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using R3;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.MindScoreRule
{
    public class MindScoreRuleUpdater : AchievementUpdater
    {
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private IDisposable _disposable;

        public MindScoreRuleUpdater(
            IAchievementService achievementService,
            IPlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
            _achievementService = achievementService;
        }
        
        public override void Enable()
        {
            _disposable = _playerProgressData.MindScore.Pairwise().Subscribe(OnValueChanged);
            OnValueChanged(_playerProgressData.MindScore.CurrentValue);
        }

        public override void Disable()
        {
            _disposable?.Dispose();
        }
        
        private void OnValueChanged((int, int) changes)
        {
            var from = changes.Item1;
            var to = changes.Item2;
            var diff = to - from;
            if (diff <= 0)
                return;
            
            OnValueChanged(_playerProgressData.MindScore.CurrentValue);
        }
        
        private void OnValueChanged(int value)
        {
            var rules = _achievementService.GetRules<MindScoreRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as MindScoreRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(value, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }

        private bool TryUpdate(int count, MindScoreRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Set(count);
            return true;
        }
    }
}