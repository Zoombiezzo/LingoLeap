using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService.Data
{
    public interface ILocalizationStorageData : IStorage
    {
        string CurrentLocalizationCode { get; }
        bool UserChanged { get; }
    }
}