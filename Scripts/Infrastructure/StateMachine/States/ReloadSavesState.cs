using System;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.LoadingCurtain;
using _Client.Scripts.GameLoop.Screens.PendingScreen;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.GameStatisticsService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SceneManagement;
using _Client.Scripts.Infrastructure.Services.SpinWheelService;
using _Client.Scripts.Infrastructure.Services.TutorialService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.StateMachine.States
{
    public class ReloadSavesState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneService _sceneService;
        private readonly IStorageService _storageService;

        public ReloadSavesState(IGameStateMachine gameStateMachine, ISceneService sceneService,
            IStorageService storageService)
        {
            _gameStateMachine = gameStateMachine;
            _sceneService = sceneService;
            _storageService = storageService;
        }
        
        private async Task LoadStats(Action<float> onProgress = null)
        {
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

        public async void Enter()
        {
            WindowsService.Hide<PendingWindow>();
            
            WindowsService.TryGetWindow<LoadingCurtainWindow>(out var window);
            window.Show();
            
            window.Show();
            window.SetStatus("Loading...");
            window.SetProgress(0f);

            await UniTask.WaitUntil(() => window.IsShow());
            
            await LoadStats();
            
            await _sceneService.UnloadScenesFromPreset(ScenePresetsKeys.Main,
                (sceneId, progress) =>
                {
                    window.SetProgress(progress);
                });
            
            
            GC.Collect();
            
            window.SetProgress(1f);
            
            _gameStateMachine.Enter<LoadMenuState>();
        }

        public void Exit()
        {
          
        }
    }
}