using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public interface ITutorialService : IConfigData, IStorable
    {
        void RegisterTutorialExecutor(ITutorialExecutor executor);
        void UnregisterTutorialExecutor(ITutorialExecutor executor);
        void FreeExecutor(ITutorialExecutor executor);
        bool IsTutorialRunning(string key);
        bool IsAnyTutorialRunning();
        bool StartTutorial(string key);
        void ResetTutorial(string id);
        void ReleaseTutorial(string id);
        void Save();
    }
}