using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using R3;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.ChangeCurrency
{
    public class ChangeCurrencyRuleUpdater : AchievementUpdater
    {
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IAchievementService _achievementService;
        private readonly List<AchievementRuleRecord> _recordsToUpdate = new(8);
        private IDisposable _disposable;

        public ChangeCurrencyRuleUpdater(
            IAchievementService achievementService,
            IPlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
            _achievementService = achievementService;
        }
        
        public override void Enable()
        {
            var builder = Disposable.CreateBuilder(); 
            _playerProgressData.Soft.Pairwise().Subscribe(OnSoftChanged);
            _playerProgressData.BoosterSelectChar.Pairwise().Subscribe(OnSelectCharChanged);
            _playerProgressData.BoosterSelectWord.Pairwise().Subscribe(OnSelectWordChanged);
            _disposable = builder.Build();
        }

        public override void Disable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        
        private void OnSelectWordChanged((int, int) changes) => 
            OnValueChanged(CurrencyType.BoosterSelectWord, changes);

        private void OnSelectCharChanged((int, int) changes) => 
            OnValueChanged(CurrencyType.BoosterSelectChar, changes);

        private void OnSoftChanged((int, int) changes) => 
            OnValueChanged(CurrencyType.Soft, changes);

        private void OnValueChanged(CurrencyType type, (int, int) changes)
        {
            var from = changes.Item1;
            var to = changes.Item2;
            var diff = to - from;
            
            if(diff == 0)
                return;
            
            var changeType = diff > 0 ? CurrencyChangeType.Increase : CurrencyChangeType.Decrease;
            
            OnValueChanged(type, changeType, Mathf.Abs(diff));
        }
        
        private void OnValueChanged(CurrencyType type, CurrencyChangeType changeType, int value)
        {
            var rules = _achievementService.GetRules<ChangeCurrencyRuleRecord>();
            if (rules == null)
                return;
            
            foreach (var rule in rules)
            {
                if (rule is not ChangeCurrencyRuleRecord ruleRecord)
                    continue;
                
                if(TryUpdate(type, changeType, value, ruleRecord) == false)
                    continue;
                
                _recordsToUpdate.Add(ruleRecord);
            }
            
            _achievementService.RulesChanged(_recordsToUpdate);
        }

        private bool TryUpdate(CurrencyType type, CurrencyChangeType changeType, int count, ChangeCurrencyRuleRecord record)
        {
            if (record == null)
                return false;
            
            var info = record.Info;

            if (info == null)
                return false;

            if (info.CurrencyType != type)
                return false;
            
            if (info.ChangeType != changeType)
                return false;
            
            record.Add(count);
            return true;
        }
    }
}