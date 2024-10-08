using System;
using System.Threading;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Achievements
{
    public class AchievementPresenter : IAsyncStartable, IDisposable
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IAchievementService _achievementService;
        private readonly INotificationService _notificationService;
        private readonly IRewardService _rewardService;

        private AchievementsWindow _window;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private AchievementContainer _achievementContainer;

        public AchievementPresenter(IAchievementService achievementService, ILocalizationService localizationService,
            ISpriteDatabaseService spriteDatabaseService, INotificationService notificationService, IRewardService rewardService)
        {
            _achievementService = achievementService;
            _localizationService = localizationService;
            _spriteDatabaseService = spriteDatabaseService;
            _notificationService = notificationService;
            _rewardService = rewardService;
        }
        
        public async UniTask StartAsync(CancellationToken cancellation)
        {
            WindowsService.TryGetWindow(out _window);
            _achievementContainer = _window.AchievementContainer;

            _notificationService.RegisterNotificationView(_window.NotificationView);
            
            _window.ClosePanel.OnClickAsObservable().Subscribe(OnClickClose).AddTo(_disposable);
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnClickClose).AddTo(_disposable);

            Observable.FromEvent(h => _window.OnBeforeShow += h, h => _window.OnBeforeShow -= h).Subscribe(OnBeforeShow)
                .AddTo(_disposable);

            Observable.FromEvent<AchievementView>(h => _achievementContainer.OnTryCollectReward += h,
                    h => _achievementContainer.OnTryCollectReward -= h).Subscribe(OnTryCollectReward)
                .AddTo(_disposable);

            await CreateAchievements();
        }

        public void Dispose()
        {
            _window.AchievementContainer.Dispose();
            _disposable?.Dispose();
        }
        
        private void OnClickClose(Unit _)
        {
            _window.Hide();
        }
        
        private void OnBeforeShow(Unit _)
        {
            _achievementContainer.SortAchievements();
            UpdateProgress();
        }
        
        private void OnTryCollectReward(AchievementView achievementView)
        {
            if(_achievementService.TryGetReward(achievementView.Record, out var reward) == false)
                return;
            
            _rewardService.SetAvailableMultipleReward(2);
            _rewardService.ShowScreenReward(reward);
            
            _achievementContainer.SortAchievements();
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            _window.CurrentStarsText.text = _achievementService.CurrentStages.ToString();
            _window.MaxStarsText.text = _achievementService.MaxAllStages.ToString();
        }

        private async Task CreateAchievements()
        {
            foreach (var achievement in _achievementService.Achievements)
            {
                _achievementContainer.CreateAchievementView(achievement);
                await UniTask.NextFrame();
            }
        }
    }
}