using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.SaveService;
using TMPro;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public interface ILocalizationService : IConfigData, IStorable
    {
        string CurrentLanguageCode { get; }
        bool UserChanged { get; }
        event Action<string> OnLanguageChanged;
        IReadOnlyList<LocalizationInfo> Localizations { get; }
        bool SetCurrentLanguage(string languageCode, bool save = true);
        LocalizationInfo GetCurrentLocalization();
        string GetValue(string key);
        void RegisterTMPText(string id, TMP_Text text);
        void UnregisterTMPText(string id, TMP_Text text);
    }
}