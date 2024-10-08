using System;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.Map;
using _Client.Scripts.GameLoop.Screens.Reward;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.CompletedLevelWindow
{
    public class CompletedLevelPresenter : IStartable, IDisposable
    {
        private CompletedLevelWindow _window;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ILevelProgressData _levelProgressData;
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IMapService _mapService;
        private readonly IRewardService _rewardService;
        
        private IDisposable _disposable;
        private bool _blocked = false;

        public CompletedLevelPresenter(IGameStateMachine gameStateMachine, ILevelProgressData levelProgressData,
            IPlayerProgressData playerProgressData, IMapService mapService, IRewardService rewardService)
        {
            _gameStateMachine = gameStateMachine;
            _levelProgressData = levelProgressData;
            _playerProgressData = playerProgressData;
            _mapService = mapService;
            _rewardService = rewardService;
        }

        public void Start()
        {
            WindowsService.TryGetWindow(out _window);

            var disposableBuilder = Disposable.CreateBuilder();
            
            _window.ButtonHome.OnClick.AsObservable().Where(IsNotBlocked).Subscribe(OnClickHome).AddTo(ref disposableBuilder);
            _window.ButtonNextLevel.OnClick.AsObservable().Where(IsNotBlocked).Subscribe(OnPlayNextLevelClick).AddTo(ref disposableBuilder);
            Observable.FromEvent(h => _window.OnBeforeShow += h, h => _window.OnBeforeShow -= h)
                .Subscribe(OnBeforeShow).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _window.OnShow += h, h => _window.OnShow -= h)
                .Subscribe(OnShow).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
            
            _window.SoftCounterField.SetValue(_playerProgressData.Soft.CurrentValue);
            _window.MindScoreCounterField.SetValue(_playerProgressData.MindScore.CurrentValue);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
        
        private void OnBeforeShow(Unit _)
        {
            _blocked = true;
            Initialize();
        }

        private bool IsNotBlocked(Unit _) => _blocked == false;

        private async void OnShow(Unit _)
        {
            _blocked = true;
            
            _window.MindScoreCounterField.SetValue(_playerProgressData.MindScore.CurrentValue, true);

            var mapProgress = _window.MapProgressbar;
            if (_mapService.TryGetCurrentLocationConfig(out var locationConfig))
            {
                mapProgress.Show();
                var currentProgress = _mapService.ProgressCounter;
                var needCount = locationConfig.RequiredCountLevels;
                
                mapProgress.SetProgress(Mathf.Min(currentProgress, needCount), needCount, true);
                
                if (_mapService.TryCollectReward(out var rewards))
                {
                    await UniTask.Delay(500);

                    WindowsService.TryGetWindow(out MapRewardWindow mapReward);
                    _mapService.ShowRewardWindow(locationConfig);
                    
                    await UniTask.WaitUntil(() => mapReward.IsShow());
                    await UniTask.WaitWhile(() => mapReward.IsShow());
                    
                    _rewardService.SetAvailableMultipleReward(2);
                    _rewardService.ShowScreenReward(rewards);
                    WindowsService.TryGetWindow(out RewardWindow window);
                    await UniTask.Delay(100);
                    await UniTask.WaitUntil(() => window.IsShow());
                    await UniTask.WaitWhile(() => window.IsShow());
                }
            }
            else
            {
                mapProgress.Hide();
            }
            
            _blocked = false;
        }

        private void OnPlayNextLevelClick(Unit _)
        {
            _gameStateMachine.Enter<LoadNextLevelGameState>();
        }
        
        private void OnClickHome(Unit _)
        {
            _gameStateMachine.Enter<TransitionFromCompletedToMainMenuState>();
        }

        private void Initialize()
        {
            _window.SoftCounterField.SetValue(_playerProgressData.Soft.CurrentValue);

            var progress = _levelProgressData.GetCurrentLevel();
            
            _window.CompletedLevelText.text = (progress-1).ToString();
            _window.NextLevelText.text = progress.ToString();
            
            var mapProgress = _window.MapProgressbar;
            if (_mapService.TryGetCurrentLocationConfig(out var locationConfig))
            {
                mapProgress.Show();
                var currentProgress = _mapService.ProgressCounter;
                var needCount = locationConfig.RequiredCountLevels;
                
                mapProgress.SetProgress(Mathf.Min(Mathf.Max(currentProgress - 1, 0), needCount), needCount);
            }
            else
            {
                mapProgress.Hide();
            }
        }
    }
}