using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.Infrastructure.Services.TimeService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using R3;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.NewDayRule
{
    public class NewDayRuleUpdater : AchievementUpdater
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ITimeService _timeService;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private readonly IAdditionalWordsData _additionalWordsData;
        private IDisposable _disposable;

        public NewDayRuleUpdater(
            IAchievementService achievementService,
            IGameStateMachine stateMachine,
            ITimeService timeService)
        {
            _achievementService = achievementService;
            _stateMachine = stateMachine;
            _timeService = timeService;
        }
        
        public override void Enable()
        {
            var builder = Disposable.CreateBuilder();
            Observable.FromEvent<IState>(h => _stateMachine.OnStateEnter += h, h => _stateMachine.OnStateEnter -= h)
                .Subscribe(OnStateEnter).AddTo(ref builder);
            _disposable = builder.Build();
        }

        public override void Disable()
        {
            _disposable?.Dispose();
        }
        
        private void OnStateEnter(IState state)
        {
            if (state is MenuState or GameState)
            {
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            _recordsToUpdate.Clear();
            var rules = _achievementService.GetRules<NewDayRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                var ruleRecord = rule as NewDayRuleRecord;
                if (ruleRecord == null)
                    continue;
                
                if(TryUpdate(ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }
        
        private bool TryUpdate(NewDayRuleRecord record)
        {
            if (record == null)
                return false;
            
            var lastDay = new DateTime(record.LastUpdate).Date;
            var now = _timeService.GetCurrentUtcDateTime().Date;
            
            if (now <= lastDay)
                return false;
            
            record.SetLastUpdate(now.Ticks);
            
            record.Add(1);
            return true;
        }
    }
}