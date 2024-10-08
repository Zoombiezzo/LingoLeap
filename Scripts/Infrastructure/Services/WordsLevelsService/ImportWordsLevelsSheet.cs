using System;
using System.Collections.Generic;
using GameSDK.Localization;
using Sirenix.OdinInspector;

namespace _Client.Scripts.Infrastructure.Services.WordsLevelsService
{
    [Serializable]
    public struct ImportWordsLevelsSheet
    {
        public string SheetId;
        
        [ValueDropdown("GetLanguages")]
        public string Localization;

#if UNITY_EDITOR
        private List<string> GetLanguages()
        {
            var languageProperties = new List<string>() { "" };
            foreach (var language in LanguageProperties.Languages)
            {
                languageProperties.Add(language.Code);
            }

            return languageProperties;
        }
#endif
    }
}