namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public interface ILocalizationInfo
    {
        string LanguageName { get; }
        string LanguageCode { get; }
        string LanguageNameTranslate { get; }
        string LanguageFlagIcon { get; }
    }
}