using System;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService
{
    public interface IGameStatisticsService : IStorable
    {
        event Action<IStatisticRecord> OnStatisticUpdated;
        void RegisterStatistic<T>(T statistic) where T : IStatisticRecord;
        bool TryGetStatistic<T>(out T statistic) where T : IStatisticRecord;
        void RegisterUpdater(IStatisticUpdater updater);
        void EnableUpdaters();
        void DisableUpdaters();
        void StatisticUpdated(IStatisticRecord record);
    }
}