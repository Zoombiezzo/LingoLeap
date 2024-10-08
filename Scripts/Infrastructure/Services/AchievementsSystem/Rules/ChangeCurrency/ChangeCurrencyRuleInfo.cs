using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.ChangeCurrency
{
    public class ChangeCurrencyRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private CurrencyChangeType _changeType;
        [SerializeField] private int[] _counts;
        
        public CurrencyType CurrencyType => _currencyType;
        public CurrencyChangeType ChangeType => _changeType;
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(ChangeCurrencyRuleRecord);
        
        public override AchievementRuleRecord CreateRecord() => 
            new ChangeCurrencyRuleRecord(Id, this);

        public override string GetDescription(ILocalizationService localizationService) => 
            string.Format(localizationService.GetValue(_description));
    }
}