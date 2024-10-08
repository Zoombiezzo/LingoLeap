using System;
using System.Collections;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts.Data;
using _Client.Scripts.Infrastructure.Services;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AchievementsStarsRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.AdditionalWordsRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.ChangeCurrency;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterLengthRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterMaxLengthRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.CharacterRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.LevelCompleteRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.MindScoreRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.NewDayRule;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem.Rules.SpinWheelRule;
using _Client.Scripts.Infrastructure.Services.AdditionalWordsService;
using _Client.Scripts.Infrastructure.Services.AdvertisingService;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService.Statistics.LevelCompleteStatistic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.RequirementService;
using _Client.Scripts.Infrastructure.Services.RequirementService.Requirements.LevelCompletedRequirements;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.CloudStorage;
using _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.LocalStorage;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.Services.SpinWheelService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.Services.TimeService;
using _Client.Scripts.Infrastructure.Services.TutorialService;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;
        private readonly IAddressablesService _addressablesService;
        private readonly IAssetProvider _assetProvider;
        private readonly IWordsDictionaryService _wordsDictionaryService;
        private readonly IWordsLevelsService _wordsLevelsService;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;
        private readonly IAdditionalWordsService _additionalWordsService;
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly IPlayerProgressData _playerProgressData;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IAchievementService _achievementService;
        private readonly ILevelProgressData _levelProgressData;
        private readonly INotificationService _notificationService;
        private readonly IGameStatisticsService _gameStatisticsService;
        private readonly IRequirementService _requirementService;
        private readonly IBankService _bankService;
        private readonly IPurchaseService _purchaseService;
        private readonly ISpinWheelService _spinWheelService;
        private readonly IAdvertisingService _advertisingService;
        private readonly IMapService _mapService;
        private readonly ITimeService _timeService;
        private readonly IAuthService _authService;
        private readonly IRateService _rateService;
        private readonly ITutorialService _tutorialService;

        public BootstrapState(IObjectResolver container,
            IGameStateMachine stateMachine, ISceneService sceneService,
            IAddressablesService addressablesService, IAssetProvider assetProvider,
            IWordsDictionaryService wordsDictionaryService,
            IWordsLevelsService wordsLevelsService,
            IStorageService storageService,
            ILocalizationService localizationService,
            IAdditionalWordsService additionalWordsService,
            IAdditionalWordsData additionalWordsData,
            IPlayerProgressData playerProgressData,
            ISpriteDatabaseService spriteDatabaseService,
            IAchievementService achievementService,
            ILevelProgressData levelProgressData,
            INotificationService notificationService,
            IGameStatisticsService gameStatisticsService,
            IRequirementService requirementService,
            IBankService bankService,
            IPurchaseService purchaseService,
            ISpinWheelService spinWheelService,
            IAdvertisingService advertisingService,
            IMapService mapService,
            ITimeService timeService,
            IAuthService authService,
            IRateService rateService,
            ITutorialService tutorialService)
        {
            _gameStateMachine = stateMachine;
            _sceneService = sceneService;
            _addressablesService = addressablesService;
            _assetProvider = assetProvider;
            _wordsDictionaryService = wordsDictionaryService;
            _wordsLevelsService = wordsLevelsService;
            _storageService = storageService;
            _localizationService = localizationService;
            _additionalWordsService = additionalWordsService;
            _additionalWordsData = additionalWordsData;
            _playerProgressData = playerProgressData;
            _spriteDatabaseService = spriteDatabaseService;
            _achievementService = achievementService;
            _levelProgressData = levelProgressData;
            _notificationService = notificationService;
            _gameStatisticsService = gameStatisticsService;
            _requirementService = requirementService;
            _bankService = bankService;
            _purchaseService = purchaseService;
            _spinWheelService = spinWheelService;
            _advertisingService = advertisingService;
            _mapService = mapService;
            _timeService = timeService;
            _authService = authService;
            _rateService = rateService;
            _tutorialService = tutorialService;
            
            _storageService.RegisterStorage<LocalStorage>(new LocalStorage());
            container.Resolve<IAdditionalWordsData>();
            container.Resolve<IPlayerProgressData>();
            
            RegisterAudioService();
            RegisterAchievementService();
            RegisterGameStatisticsService();
            RegisterRequirementService();
            
            Debugger.Initialize(true);
        }

        private void RegisterRequirementService()
        {
            _requirementService.RegisterRequirementSystem<LevelNeedRequirement>(
                new LevelCompletedRequirementSystem(_requirementService, _gameStatisticsService));
        }

        private void RegisterGameStatisticsService()
        {
            _gameStatisticsService.RegisterUpdater(
                new LevelCompleteStatisticUpdater(_gameStatisticsService, _levelProgressData));
        }

        private void RegisterAudioService()
        {
            AudioService.RegisterStorage(_storageService);
            AudioService.RegisterAssetProvider(_assetProvider);
            var audioDataStorage = new StorableData<AudioService>(AudioService.Instance, new AudioStorageData());
            _storageService.Register<AudioService>(audioDataStorage);
        }

        private void RegisterAchievementService()
        {
            _achievementService.RegisterUpdater(new CharacterContainsRuleUpdater(_achievementService,
                _levelProgressData, _localizationService, _additionalWordsData));
            _achievementService.RegisterUpdater(new CharacterLengthRuleUpdater(_achievementService, _levelProgressData,
                _additionalWordsData));
            _achievementService.RegisterUpdater(new MindScoreRuleUpdater(_achievementService, _playerProgressData));
            _achievementService.RegisterUpdater(new CharacterMaxLengthRuleUpdater(_achievementService,
                _levelProgressData, _wordsLevelsService, _additionalWordsData));
            _achievementService.RegisterUpdater(new ChangeCurrencyRuleUpdater(_achievementService,
                _playerProgressData));
            _achievementService.RegisterUpdater(new SpinWheelRuleUpdater(_achievementService, _spinWheelService));
            _achievementService.RegisterUpdater(new LevelCompleteRuleUpdater(_achievementService, _levelProgressData));
            _achievementService.RegisterUpdater(new AdditionalWordsRuleUpdater(_achievementService, _additionalWordsData));
            _achievementService.RegisterUpdater(new NewDayRuleUpdater(_achievementService, _gameStateMachine,
                _timeService));            
            _achievementService.RegisterUpdater(new AchievementsStarsRuleUpdater(_achievementService));
        }

        private IEnumerator UpdateAddressables(Action<float> onUpdate)
        {
            yield return _addressablesService.Initialize(onUpdate);
            yield return _addressablesService.CheckAndUpdateBundles(onUpdate);
        }
        
        private async Task LoadData(Action<float> onProgress)
        {
            Func<Task>[] tasks = {
                _sceneService.LoadData,
                AudioService.Instance.LoadData,
                _spriteDatabaseService.LoadData,
                _wordsDictionaryService.LoadData,
                _wordsLevelsService.LoadData,
                _localizationService.LoadData,
                _additionalWordsService.LoadData,
                _playerProgressData.LoadData,
                _achievementService.LoadData,
                _bankService.LoadData,
                _spinWheelService.LoadData,
                _mapService.LoadData,
                _tutorialService.LoadData
            };

            foreach (var task in tasks)
            {
                await task();
                onProgress?.Invoke(1f / tasks.Length);
            }
        }

        private async Task LoadStats(Action<float> onProgress)
        {
            await _storageService.Initialize();
            
            if (_authService.SignInType == SignInType.Account)
            {
                var currentTime = _storageService.CurrentStorageInfo;
                
                _storageService.RegisterStorage<CloudStorage>(new CloudStorage(_authService));
                var result = await _storageService.TryLoadInfo<CloudStorage>();
                
                if (result.Item1)
                {
                    var cloudStorageInfo = result.Item2;
                    var diffDate = new TimeSpan(Math.Abs(currentTime.SaveTime - cloudStorageInfo.SaveTime));
                    
                    if (diffDate.TotalSeconds > 0)
                    {
                        await _storageService.ResolveSaveByStorage<CloudStorage>();
                    }
                }
                else
                {
                    await _storageService.ResolveSaveByStorage<LocalStorage>();
                }
                
                _storageService.SetPreferredStorage<CloudStorage>();

            }
            
            Func<Task>[] tasks = {
                _storageService.Load<AudioService>,
                _storageService.Load<ILevelProgressData>,
                _storageService.Load<IAdditionalWordsData>,
                _storageService.Load<IPlayerProgressData>,
                _storageService.Load<ILocalizationService>,
                _storageService.Load<IAchievementService>,
                _storageService.Load<IGameStatisticsService>,
                _storageService.Load<IBankService>,
                _storageService.Load<ISpinWheelService>,
                _storageService.Load<IMapService>,
                _storageService.Load<IAuthService>,
                _storageService.Load<IRateService>,
                _storageService.Load<ITutorialService>
            };
            
            foreach (var task in tasks)
            {
                await task();
                onProgress?.Invoke(1f / tasks.Length);
            }
        }
        
        
        private void InitializeGameStatistics()
        {
            _gameStatisticsService.RegisterStatistic(new LevelCompleteStatisticRecord());
            _gameStatisticsService.EnableUpdaters();
        }

        public async void Enter()
        {
            WindowsService.TryGetWindow<LoadingCurtainWindow>(out var window);
            window.Show();

            await GameSDK.Core.GameApp.Initialize();
            
            await _authService.SignInAsGuest();
            
            Debug.Log($"[AuthService]: Signed in as {_authService.SignInType}");
            
            window.SetStatus("Upadating addressables...");
            
            await UpdateAddressables(progress => window.SetProgress(progress)).ToUniTask();
            
            _sceneService.Initialize();
            
            window.SetStatus("Loading data...");
            
            await _timeService.Initialize();
            
            _advertisingService.Initialize();
            _purchaseService.Initialize();
            
            await LoadData(progress => window.SetProgress(progress));

            await LoadStats(progress => window.SetProgress(progress));
            
            _achievementService.EnableUpdaters();
            InitializeGameStatistics();
            _requirementService.EnableSystems();
            
            _gameStateMachine.Enter<LoadMenuState>();
        }

        public void Exit()
        {
            
        }
    }
}