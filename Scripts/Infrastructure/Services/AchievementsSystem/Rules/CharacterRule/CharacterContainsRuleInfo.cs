using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterRule
{
    [Serializable]
    public class CharacterContainsRuleInfo : AchievementRuleInfo
    {
        [SerializeField] private LocalizationValue<string>[] _characters;
        [SerializeField] private int[] _counts;
        public int[] Counts => _counts;
        public override Type TypeRecord => typeof(CharacterContainsRuleRecord);
        
        private Dictionary<string, string> _charactersMap = new();
        
        public override AchievementRuleRecord CreateRecord() => 
            new CharacterContainsRuleRecord(Id, this);

        public string GetCharacter(string locale)
        {
            if (_charactersMap.TryGetValue(locale, out var character))
            {
                return character;
            }

            Debug.LogWarning("[CharacterContainsRuleInfo] Character not found in locale: " + locale);
            return string.Empty;
        }

        public override void Initialize()
        {
            base.Initialize();
            _charactersMap ??= new Dictionary<string, string>(_characters.Length);
            _charactersMap.Clear();
            
            foreach (var localizationValue in _characters)
            {
                _charactersMap.TryAdd(localizationValue.LanguageCode, localizationValue.Value);
            }
        }

        public override string GetDescription(ILocalizationService localizationService)
        {
            string characters = _charactersMap.GetValueOrDefault(localizationService.CurrentLanguageCode, "[CHAR]");
            var result = string.Format(localizationService.GetValue(_description), characters);
            return result;
        }
    }
}