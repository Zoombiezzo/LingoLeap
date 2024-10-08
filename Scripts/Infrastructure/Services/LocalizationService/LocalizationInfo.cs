using System.Collections.Generic;
using System.Linq;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using GameSDK.Localization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    [System.Serializable]
    public class LocalizationInfo : ILocalizationInfo
    {
        [FoldoutGroup("@LocalizationName")] 
        [SerializeField] [ValueDropdown("@LanguageNames")]
        [OnValueChanged(@"OnLanguageChanged")]
        private string _languageName;
        
        [FoldoutGroup("@LocalizationName")] 
        [SerializeField] [ReadOnly]
        private string _languageCode;
        
        [FoldoutGroup("@LocalizationName")]
        [SerializeField]
        private string _languageNameTranslate;
        
        [FoldoutGroup("@LocalizationName")]
        [SerializeField] [ValueDropdown("SpritesDropdown")]
        private string _languageFlagIcon;
        
        public string LanguageName => _languageName;
        public string LanguageCode => _languageCode;
        public string LanguageNameTranslate => _languageNameTranslate;
        public string LanguageFlagIcon => _languageFlagIcon;

#if UNITY_EDITOR
        private string LocalizationName => _languageName;
        
        private IEnumerable<string> LanguageNames => LanguageProperties.Languages.Select(el => el.Name);

        private void OnLanguageChanged()
        {
            var language = LanguageProperties.Languages.FirstOrDefault(el => el.Name == _languageName).Code;
            _languageCode = language;
        }
        
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>();
            var configs = ConfigsHelper<SpritesPreset>.GetConfigs();
            foreach (var spritesPreset in configs)
            {
                foreach (var sprites in spritesPreset.SpritePresets)
                {
                    spritesIds.Add(spritesPreset.GetIdSprite(sprites.Id));
                }
            }

            return spritesIds;
        }
#endif
    }
}