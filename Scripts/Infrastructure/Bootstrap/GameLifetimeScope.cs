using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.Infrastructure.RandomService;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.AdditionalWordsService;
using _Client.Scripts.Infrastructure.Services.AdvertisingService;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.BankService.Factory;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService;
using _Client.Scripts.Infrastructure.Services.LimitationService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.RequirementService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.Services.SpinWheelService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.Services.TimeService;
using _Client.Scripts.Infrastructure.Services.TutorialService;
using _Client.Scripts.Infrastructure.Services.WalletService;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IStateFactory, ContainerStateFactory>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            builder.Register<IAdvertisingProvider, GameSDKAdvertisingProvider>(Lifetime.Singleton);
            builder.Register<IAdvertisingService, AdvertisingService>(Lifetime.Singleton);
            builder.Register<IAddressablesService, AddressablesService>(Lifetime.Singleton);
            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Singleton);
            builder.Register<ISceneService, SceneService>(Lifetime.Singleton);
            builder.Register<IWordsDictionaryService, WordsDictionaryService>(Lifetime.Singleton);
            builder.Register<IWordsLevelsService, WordsLevelsService>(Lifetime.Singleton);
            builder.Register<ITimeService, GameSDKTimeService>(Lifetime.Singleton);
            builder.Register<IAuthProvider, GameSDKAuthProvider>(Lifetime.Singleton);
            builder.Register<IAuthService, AuthService>(Lifetime.Singleton);
            builder.Register<IRateProvider, GameSDKRateProvider>(Lifetime.Singleton);
            builder.Register<IRateService, RateService>(Lifetime.Singleton);
            builder.Register<IStorageService, StorageService>(Lifetime.Singleton)
                .WithParameter(0).WithParameter("lingoleap").WithParameter(false);
            builder.Register<ILocalizationService, LocalizationService>(Lifetime.Singleton)
                .WithParameter<ILocalizationProvider>(new GameSDKLocalizationProvider());
            builder.Register<IAdditionalWordsService, AdditionalWordsService>(Lifetime.Singleton);
            builder.Register<ISpriteDatabaseService, SpriteDatabaseService>(Lifetime.Singleton);
            builder.Register<IAchievementService, AchievementService>(Lifetime.Singleton);
            builder.Register<IGameStatisticsService, GameStatisticsService>(Lifetime.Singleton);
            builder.Register<IRequirementService, RequirementService>(Lifetime.Singleton);
            builder.Register<ILimitationService, LimitationService>(Lifetime.Singleton);
            builder.Register<IProductPurchaseService, GameSDKProductPurchaseService>(Lifetime.Singleton);
            builder.Register<IPurchaseService, PurchaseService>(Lifetime.Singleton);
            builder.Register<IBankService, BankService>(Lifetime.Singleton);
            builder.Register<IBankFactory, BankFactory>(Lifetime.Singleton);
            builder.Register<IRandomService, RandomService.RandomService>(Lifetime.Singleton);
            
            builder.Register<ILevelProgressData, LevelProgressData>(Lifetime.Singleton);
            builder.Register<IAdditionalWordsData, AdditionalWordsData>(Lifetime.Singleton);
            builder.Register<IPlayerProgressData, PlayerProgressData>(Lifetime.Singleton);
            
            builder.Register<IWalletService, WalletService>(Lifetime.Singleton);
            builder.Register<IRewardService, RewardService>(Lifetime.Singleton);
            
            builder.Register<INotificationService, NotificationService>(Lifetime.Singleton);
            builder.Register<ISpinWheelService, SpinWheelService>(Lifetime.Singleton);
            builder.Register<IMapService, MapService>(Lifetime.Singleton);

            builder.Register<ITutorialService, TutorialService>(Lifetime.Singleton);
            
            builder.Register<IState, BootstrapState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, LoadMenuState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, LoadNextLevelGameState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, MenuState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, LoadGameState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, GameState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, TransitionFromGameToMainMenuState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, TransitionFromCompletedToMainMenuState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, CompletedLevelState>(Lifetime.Transient).AsSelf();
            builder.Register<IState, ReloadSavesState>(Lifetime.Transient).AsSelf();
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
