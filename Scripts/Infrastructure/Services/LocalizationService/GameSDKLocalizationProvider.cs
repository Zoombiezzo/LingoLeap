using System.Threading.Tasks;
using GameSDK.Core;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public class GameSDKLocalizationProvider : ILocalizationProvider
    {
        private string _currentLanguageCode;

        public string CurrentLanguageCode => _currentLanguageCode;
        
        public async Task Initialize()
        {
            if (GameApp.IsInitialized == false)
            {
                await GameApp.Initialize();
            }

            _currentLanguageCode = "ru"; //GameApp.Lang;
        }
    }
}