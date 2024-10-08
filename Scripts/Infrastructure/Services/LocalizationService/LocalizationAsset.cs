using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    [CreateAssetMenu(menuName = "Localization/Localization Asset", fileName = "LocalizationAsset", order = 0)]
    public class LocalizationAsset : ScriptableObject , ILocalizationAsset
    {
        [FoldoutGroup("Основное")] [ValueDropdown("LanguageNames")]
        [SerializeField]
        private string _baseLocalizationCode;
        
        [FoldoutGroup("Локализации")]
        [SerializeField]
        private LocalizationInfo[] _localizations;
        
        public string BaseLocalizationCode => _baseLocalizationCode;
        public LocalizationInfo[] Localizations => _localizations;
        
#if UNITY_EDITOR
        private IEnumerable<string> LanguageNames => _localizations.Select(el => el.LanguageCode);
#endif
    }
}