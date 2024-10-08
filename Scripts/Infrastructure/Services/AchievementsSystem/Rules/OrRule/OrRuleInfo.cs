using System;
using System.Collections.Generic;
using System.Text;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.OrRule
{
    [Serializable]
    public class OrRuleInfo : AchievementRuleInfo
    {
        [SerializeReference] private List<AchievementRuleInfo> _rules = new(4);
        public override bool IsComposite => true;
        public override IReadOnlyList<AchievementRuleInfo> Children => _rules;

        public override Type TypeRecord => typeof(OrRuleRecord);

        private static StringBuilder _stringBuilder = new(64);

        public override AchievementRuleRecord CreateRecord()
        {
            var rules = new List<AchievementRuleRecord>();
            foreach (var rule in _rules)
            {
                rules.Add(rule.CreateRecord());
            }

            return new OrRuleRecord(Id, rules, this);
        }

        public override string GetDescription(ILocalizationService localizationService)
        {
            _stringBuilder.Clear();
            
            var localized = localizationService.GetValue(_description);

            for (var index = 0; index < _rules.Count; index++)
            {
                var rule = _rules[index];
                _stringBuilder.Append(rule.GetDescription(localizationService));

                if (index < _rules.Count - 1)
                {
                    _stringBuilder.AppendFormat(" {0} ", localized);
                }
            }

            return _stringBuilder.ToString();
        }
    }
}