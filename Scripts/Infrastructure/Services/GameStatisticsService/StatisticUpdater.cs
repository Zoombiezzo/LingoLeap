namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService
{
    public abstract class StatisticUpdater : IStatisticUpdater
    {
        public abstract void Enable();
        public abstract void Disable();
    }
}