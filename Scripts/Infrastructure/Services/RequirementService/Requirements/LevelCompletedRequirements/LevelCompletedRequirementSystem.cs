using System;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService.Statistics.LevelCompleteStatistic;
using R3;

namespace _Client.Scripts.Infrastructure.Services.RequirementService.Requirements.LevelCompletedRequirements
{
    public class LevelCompletedRequirementSystem : IRequirementSystem
    {
        private readonly IGameStatisticsService _gameStatisticsService;
        private readonly IRequirementService _requirementService;

        private IDisposable _disposable;

        public LevelCompletedRequirementSystem(IRequirementService requirementService, IGameStatisticsService gameStatisticsService)
        {
            _requirementService = requirementService;
            _gameStatisticsService = gameStatisticsService;
        }

        public void Enable()
        {
            _disposable = Observable.FromEvent<IStatisticRecord>(h => _gameStatisticsService.OnStatisticUpdated += h,
                    h => _gameStatisticsService.OnStatisticUpdated -= h)
                .Subscribe(OnStatisticUpdated);
        }

        public void Disable()
        {
            _disposable?.Dispose();
        }
        
        private void OnStatisticUpdated(IStatisticRecord record)
        {
            if (record is LevelCompleteStatisticRecord)
            {
                _requirementService.CheckRequirementsByType<LevelNeedRequirement>();
            }
        }

        public bool Check(IRequirement requirement)
        {
            var requirementLevel = requirement as LevelNeedRequirement;

            if (_gameStatisticsService.TryGetStatistic<LevelCompleteStatisticRecord>(
                    out var levelCompleteStatisticRecord) == false)
                return false;
            
            return requirementLevel != null && levelCompleteStatisticRecord.Count >= requirementLevel.Level;
        }
    }
}