using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public interface ILocalizationProvider
    {
        string CurrentLanguageCode { get; }
        Task Initialize();
    }
}