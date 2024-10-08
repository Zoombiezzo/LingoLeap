using System;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using R3;

namespace _Client.Scripts.Infrastructure.Services.MapService.Updaters
{
    public class LevelProgressUpdater : IProgressUpdater
    {
        private readonly ILevelProgressData _levelProgressData;
        private readonly IMapService _mapService;

        private IDisposable _levelCompleteSubscription;

        public LevelProgressUpdater(IMapService mapService, ILevelProgressData levelProgressData)
        {
            _levelProgressData = levelProgressData;
            _mapService = mapService;
        }

        public void Enable()
        {
            _levelCompleteSubscription = Observable.FromEvent<int>(h => _levelProgressData.OnLevelCompleted += h, h => _levelProgressData.OnLevelCompleted -= h)
                .Subscribe(OnLevelComplete);
        }
        
        public void Disable()
        {
            _levelCompleteSubscription.Dispose();
        }
        
        private void OnLevelComplete(int levelNumber)
        {
            _mapService.AddProgress(1);
        }
    }
}