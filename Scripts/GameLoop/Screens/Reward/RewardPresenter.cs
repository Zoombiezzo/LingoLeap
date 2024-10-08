using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Screens.PendingScreen;
using _Client.Scripts.GameLoop.Screens.Reward.RewardViewFactory;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Reward
{
    public class RewardPresenter : IStartable, IDisposable
    {
        private RewardWindow _window;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ILevelProgressData _levelProgressData;
        private readonly ILocalizationService _localizationService;
        private readonly IRewardService _rewardService;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IPurchaseService _purchaseService;

        private Dictionary<RewardType, IRewardViewFactory> _rewardsFactory = new();
        
        private List<RewardView> _rewardViews = new(8);

        private AdsData _adsData;
        
        private IDisposable _disposable;
        
        private Sequence _showSequence;
        private Sequence _hideSequence;

        private Action _collectMultipleRewardOnce;
        
        private bool _isBlocked;

        public RewardPresenter(IGameStateMachine gameStateMachine, IRewardService rewardService, ISpriteDatabaseService spriteDatabaseService,
            IPurchaseService purchaseService)
        {
            _gameStateMachine = gameStateMachine;
            _rewardService = rewardService;
            _spriteDatabaseService = spriteDatabaseService;
            _purchaseService = purchaseService;
            _adsData = new AdsData()
            {
                CurrencyType = CurrencyType.Ads
            };
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            
            _rewardsFactory.Add(RewardType.Currency, new CurrencyRewardViewFactory(_window.RewardPrefab, _spriteDatabaseService));

            var disposableBuilder = Disposable.CreateBuilder();
            
            _window.CloseButton.OnClickAsObservable().Subscribe(OnCloseClick).AddTo(ref disposableBuilder);
            _window.MultiplierButton.OnClick.AsObservable().Subscribe(OnMultiplierClick).AddTo(ref disposableBuilder);
            Observable.FromEvent<IReward>(h => _rewardService.OnShowScreenReward += h,
                    h => _rewardService.OnShowScreenReward -= h)
                .Subscribe(OnShowReward).AddTo(ref disposableBuilder);
            
            Observable.FromEvent<IRewardInfo>(h => _rewardService.OnShowScreenRewardInfo += h,
                    h => _rewardService.OnShowScreenRewardInfo -= h)
                .Subscribe(OnShowRewardInfo).AddTo(ref disposableBuilder);
            
            Observable.FromEvent<IReadOnlyList<IRewardInfo>>(h => _rewardService.OnShowScreenRewardInfoList += h,
                    h => _rewardService.OnShowScreenRewardInfoList -= h)
                .Subscribe(OnShowRewardInfoList).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
        
        private void OnShowReward(IReward reward)
        {
            _collectMultipleRewardOnce = CollectMultipleReward;
            
            ClearRewards();
            
            CreateReward(reward);
            
            ShowScreen();
            
            return;

            void CollectMultipleReward()
            {
                _rewardService.TryCollectReward(reward);
            }
        }
        
        
        
        private void OnShowRewardInfo(IRewardInfo rewardInfo)
        {
            _collectMultipleRewardOnce = CollectMultipleReward;

            ClearRewards();
            
            foreach (var reward in rewardInfo.Rewards)
            {
                CreateReward(reward);
            }
            
            ShowScreen();
            
            void CollectMultipleReward()
            {
                _rewardService.TryCollectReward(rewardInfo);
            }
        }
        
        private void OnShowRewardInfoList(IReadOnlyList<IRewardInfo> rewardsInfo)
        {
            _collectMultipleRewardOnce = CollectMultipleReward;

            ClearRewards();

            foreach (var rewardInfo in rewardsInfo)
            {
                foreach (var reward in rewardInfo.Rewards)
                {
                    CreateReward(reward);
                }   
            }
            
            ShowScreen();
            
            void CollectMultipleReward()
            {
                for (var index = 0; index < rewardsInfo.Count; index++)
                {
                    var rewardInfo = rewardsInfo[index];
                    _rewardService.TryCollectReward(rewardInfo);
                }
            }
        }
        
        private void ClearRewards()
        {
            foreach (var rewardView in _rewardViews)
            {
                UnityEngine.Object.Destroy(rewardView.gameObject);
            }
            
            _rewardViews.Clear();
        }
        
        private void CreateReward(IReward reward)
        {
            if (_rewardsFactory.TryGetValue(reward.Type, out var factory) == false)
                return;

            var view = factory.Create(reward, _window.RewardsContainer);
            
            if(view == null)
                return;
            
            view.Hide();
            _rewardViews.Add(view);
        }

        private async void ShowScreen()
        {
            _isBlocked = true;

            _window.MultiplierButton.Hide(false);
            _window.AnimationElement.Hide();
            _window.Show();

            await UniTask.WaitUntil(() => _window.IsShow());
            
            _showSequence?.Kill();
            _hideSequence?.Kill();
            _showSequence = DOTween.Sequence();
            
            var delay = _window.Duration;

            foreach (var rewardView in _rewardViews)
            {
                _showSequence.AppendCallback(() => rewardView.Show(true));
                _showSequence.AppendInterval(delay);
            }
            
            if (_rewardService.MultiplierRewardAvailable)
            {
                _window.MultiplierText.text = _rewardService.MultiplierRewardValue.ToString();
                _showSequence.AppendCallback(() => _window.MultiplierButton.Show());
                _showSequence.AppendInterval(0.5f);
            }

            _showSequence.AppendCallback(() => _window.AnimationElement.Show(true));
            _showSequence.AppendInterval(_window.AnimationElement.DurationShow);

            await _showSequence.Play().AsyncWaitForCompletion().AsUniTask();
            
            _isBlocked = false;
        }
        
        private async void HideScreen()
        {
            _collectMultipleRewardOnce = null;
            _rewardService.DisableMultipleReward();
            _isBlocked = true;
            
            _showSequence?.Kill();
            _hideSequence?.Kill();
            _hideSequence = DOTween.Sequence();
            
            _hideSequence.AppendCallback(() => _window.AnimationElement.Hide(true));
            _hideSequence.AppendInterval(_window.AnimationElement.DurationHide);
            
            var delay = _window.Duration;

            foreach (var rewardView in _rewardViews)
            {
                _hideSequence.AppendCallback(() => rewardView.Hide(true));
                _hideSequence.AppendInterval(delay);
            }

            _hideSequence.AppendCallback(() => _window.Hide());
            
            await _hideSequence.Play().AsyncWaitForCompletion().AsUniTask();
            
            await UniTask.WaitUntil(() => _window.IsShow() == false);
            
            _isBlocked = false;
        }
        
        private void OnMultiplierClick(Unit _)
        {
            if(_isBlocked)
                return;
            
            if(_rewardService.MultiplierRewardAvailable == false)
                return;
            
            if(_collectMultipleRewardOnce == null)
                return;
            
            var counter = _rewardService.MultiplierRewardValue - 1;
            
            if(counter < 0)
                return;

            WindowsService.Show<PendingWindow>();

            _purchaseService.Purchase(
                _adsData,
                new PurchaseItemReceiver(PurchaseMultiplierHandler));
            
            return;

            void PurchaseMultiplierHandler(bool isSuccess)
            {
                WindowsService.Hide<PendingWindow>();

                if (isSuccess == false)
                    return;
                
                for (int i = 0; i < counter; i++)
                    _collectMultipleRewardOnce.Invoke();

                HideScreen();
            }
        }
        
        private void OnCloseClick(Unit _)
        {
            if(_isBlocked)
                return;
            
            HideScreen();
        }
    }
}