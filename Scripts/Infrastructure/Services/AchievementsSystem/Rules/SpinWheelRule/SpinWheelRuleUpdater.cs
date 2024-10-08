using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SpinWheelService;
using R3;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.SpinWheelRule
{
    public class SpinWheelRuleUpdater : AchievementUpdater
    {
        private readonly ISpinWheelService _spinWheelService;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private IDisposable _disposable;

        public SpinWheelRuleUpdater(
            IAchievementService achievementService,
            ISpinWheelService spinWheelService)
        {
            _spinWheelService = spinWheelService;
            _achievementService = achievementService;
        }
        
        public override void Enable()
        {
            var builder = Disposable.CreateBuilder();
            Observable.FromEvent(h => _spinWheelService.OnSpinUsed += h, h => _spinWheelService.OnSpinUsed -= h)
                .Subscribe(OnSpinUsed).AddTo(ref builder);
            _disposable = builder.Build();
        }

        public override void Disable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnSpinUsed(Unit _)
        {
            OnValueChanged();
        }
        
        private void OnValueChanged()
        {
            var rules = _achievementService.GetRules<SpinWheelRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                if (rule is not SpinWheelRuleRecord ruleRecord)
                    continue;
                
                if(TryUpdate(1, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }

        private bool TryUpdate(int count, SpinWheelRuleRecord record)
        {
            if (record == null)
                return false;
            
            record.Add(count);
            return true;
        }
    }
}