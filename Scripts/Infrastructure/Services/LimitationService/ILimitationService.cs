using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.LimitationService
{
    public interface ILimitationService : IService, IStorable
    {
        void RegisterUpdater(string context, LimitationUpdater updater);
        void UnregisterUpdater(string context, LimitationUpdater updater);

        void RegisterLimitation<T>(string context, string id, ILimitation limitation, ILimiter limiter, bool isSavable = true)
            where T : LimitationUpdater;

        void UnregisterLimitation<T>(string context, string id, ILimitation limitation, ILimiter limiter)
            where T : LimitationUpdater;
    }
}