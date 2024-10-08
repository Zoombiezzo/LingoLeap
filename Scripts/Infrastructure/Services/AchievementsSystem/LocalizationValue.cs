using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using GameSDK.Localization;
#endif

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [Serializable]
    public class LocalizationValue<T> where T : class
    {
        [SerializeField] [ValueDropdown("LanguageCodes")]
        private string _languageCode;
        
        [SerializeField] private T _value;
        
        public string LanguageCode => _languageCode;   
        public T Value => _value;
        
#if UNITY_EDITOR
        private IEnumerable<string> LanguageCodes => LanguageProperties.Languages.Select(el => el.Code);
#endif
    }
}