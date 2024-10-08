using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService
{
    public class GameStatisticsService : IGameStatisticsService
    {
        private readonly IStorageService _storageService;
        private readonly GameStatisticStorage _storage;
        
        private Dictionary<Type, IStatisticRecord> _statistics = new(8);
        private List<IStatisticUpdater> _updaters = new(8);

        public GameStatisticsService(IStorageService storageService)
        {
            _storageService = storageService;
            _storage = new GameStatisticStorage();
            
            _storageService.Register<IGameStatisticsService>(new StorableData<IGameStatisticsService>(this, _storage));

        }

        public event Action<IStatisticRecord> OnStatisticUpdated;

        public void RegisterStatistic<T>(T statistic) where T : IStatisticRecord
        {
            if (TryInitializeStatistic(statistic) == false)
                return;

            _storage.Statistics.Add(statistic);
        }

        public bool TryGetStatistic<T>(out T statistic) where T : IStatisticRecord
        {
            var type = typeof(T);
            if (_statistics.TryGetValue(type, out var record) == false)
            {
                Debugger.Log($"[GameStatisticsService]: Statistic {type} not found");
                statistic = default;
                return false;
            }

            statistic = (T) record;
            return true;
        }
        
        public void RegisterUpdater(IStatisticUpdater updater)
        {
            _updaters.Add(updater);
        }

        public void EnableUpdaters()
        {
            foreach (var updater in _updaters)
            {
                updater.Enable();
            }
        }
        
        public void DisableUpdaters()
        {
            foreach (var updater in _updaters)
            {
                updater.Disable();
            }
        }

        public void StatisticUpdated(IStatisticRecord record)
        {
            OnStatisticUpdated?.Invoke(record);
            
            _storageService.Save<IGameStatisticsService>();
        }

        public void Load(IStorage data)
        {
            foreach (var statistic in _storage.Statistics)
            { 
                TryInitializeStatistic(statistic);
            }
        }

        public string ToStorage() => _storage.ToData(this);
        
        public bool TryInitializeStatistic<T>(T statistic) where T : IStatisticRecord
        {
            var type = statistic.GetType();

            if (_statistics.TryGetValue(type, out var record))
            {
                Debugger.Log($"[GameStatisticsService]: Statistic {type} already exists");
                return false;
            }
            
            _statistics.Add(type, statistic);
            return true;
        }
    }
}