namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public interface ILocalizationAsset
    {
        string BaseLocalizationCode { get; }
        LocalizationInfo[] Localizations { get; }
    }
}