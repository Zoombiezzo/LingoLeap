using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public class TutorialService : ITutorialService
    {
        private const string AssetPath = "Tutorial";

        private readonly IStorageService _storageService;
        private readonly IAssetProvider _assetProvider;
        private readonly TutorialStorageData _storage;
        private readonly Dictionary<string, TutorialConfig> _configsMap = new(16);
        private readonly Dictionary<string, ITutorialExecutor> _executors = new(16);
        private readonly List<TutorialConfig> _configs = new(16);
        
        private readonly HashSet<string> _runningExecutor = new(8);
        private readonly HashSet<string> _runningTutorial = new(8);
        
        public TutorialService(IStorageService storageService, IAssetProvider assetProvider)
        {
            _storageService = storageService;
            _assetProvider = assetProvider;
            
            _storage = new TutorialStorageData();
            _storageService.Register<ITutorialService>(new StorableData<ITutorialService>(this, _storage));
        }

        public void RegisterTutorialExecutor(ITutorialExecutor executor) => _executors.TryAdd(executor.Id, executor);

        public void UnregisterTutorialExecutor(ITutorialExecutor executor) => _executors.Remove(executor.Id);

        public void FreeExecutor(ITutorialExecutor executor) => _runningExecutor.Remove(executor.Id);

        public bool IsTutorialRunning(string key) => _runningTutorial.Contains(key);

        public bool IsAnyTutorialRunning() => _runningTutorial.Count > 0;

        public bool StartTutorial(string key)
        {
            if(_configsMap.TryGetValue(key, out var config) == false)
                return false;
            
            if(_storage.TryGetTutorial(key, out var data) == false)
                return false;

            if (data.IsCompleted)
                return false;

            if (_runningTutorial.Contains(key))
                return false;
            
            var executorId = config.TutorialExecutorId;
            
            if(_executors.TryGetValue(executorId, out var executor) == false)
                return false;

            if (_runningExecutor.Contains(executorId))
                return false;

            if (executor.Execute(config, data) == false)
                return false;
            
            _runningTutorial.Add(key);
            _runningExecutor.Add(executorId);
            
            return true;
        }

        public void ResetTutorial(string id)
        {
            if (_storage.TryGetTutorial(id, out var data))
                data.Reset();
            
            if(_runningTutorial.Contains(id) == false)
                return;
            
            if(_configsMap.TryGetValue(id, out var config) == false)
                return;
            
            if (_executors.TryGetValue(config.TutorialExecutorId, out var executor) == false)
                return;
            
            if (_runningExecutor.Contains(config.TutorialExecutorId) == false)
                return;
            
            executor.Reset();
            
            _runningExecutor.Remove(config.TutorialExecutorId);
            _runningTutorial.Remove(id);
        }

        public void ReleaseTutorial(string id) => _runningTutorial.Remove(id);

        public void Save() => _storageService.Save<ITutorialService>();

        public void Load(IStorage data)
        {
            foreach (var config in _configs)
            {
                if (_storage.ContainsTutorial(config.Id) == false)
                {
                    var tutorialData = new TutorialData(config.Id);
                    _storage.TryAddTutorial(tutorialData);
                }
            }
        }

        public string ToStorage()
        {
            var storableData = _storageService.Get<ITutorialService>();
            return storableData.Storage.ToData(this);
        }

        public async Task LoadData()
        {
            var assets = await _assetProvider.LoadAll<TutorialConfig>(AssetPath);
            _configsMap.Clear();

            foreach (var config in assets)
            {
                if (_configsMap.TryAdd(config.Id, config)) 
                    _configs.Add(config);
            }
        }
    }
}