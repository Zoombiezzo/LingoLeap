using System;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using R3;

namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService.Statistics.LevelCompleteStatistic
{
    public class LevelCompleteStatisticUpdater : StatisticUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IGameStatisticsService _statisticsService;

        private IDisposable _levelCompleteSubscription;

        public LevelCompleteStatisticUpdater(IGameStatisticsService statisticsService, ILevelProgressData levelProgressData)
        {
            _statisticsService = statisticsService;
            _levelProgressData = levelProgressData;
        }
        
        public override void Enable()
        {
            _levelCompleteSubscription = Observable.FromEvent<int>(h => _levelProgressData.OnLevelCompleted += h, h => _levelProgressData.OnLevelCompleted -= h)
                .Subscribe(OnLevelComplete);
        }
        
        private void OnLevelComplete(int levelNumber)
        {
            if(_statisticsService.TryGetStatistic<LevelCompleteStatisticRecord>(out var levelCompleteStatistic) == false)
                return;
            
            levelCompleteStatistic.Add(1);
            
            _statisticsService.StatisticUpdated(levelCompleteStatistic);
        }

        public override void Disable()
        {
            _levelCompleteSubscription?.Dispose();
        }
    }
}