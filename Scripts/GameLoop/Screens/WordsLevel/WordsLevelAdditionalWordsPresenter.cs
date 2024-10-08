using System;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using _Client.Scripts.GameLoop.Components.WordSelector;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Screens.AdditionalWords;
using _Client.Scripts.Infrastructure.Services.AdditionalWordsService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.WordsLevel
{
    public class WordsLevelAdditionalWordsPresenter : IStartable, IDisposable
    {
        private WordsLevelWindow _wordsLevelWindow;
        private WordSelector _wordSelector;

        private readonly IWordsDictionaryService _wordsDictionaryService;
        private readonly IWordsLevelsService _wordsLevelsService;
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly IAdditionalWordsService _additionalWordsService;
        private readonly ILevelProgressData _levelProgressData;
        private readonly IRewardService _rewardService;

        private IDisposable _disposable;
        private Infrastructure.Services.WordsLevelsService.WordsLevel _wordsLevel;
        private WordsContainer _wordsContainer;
        private AdditionalWordProgressView _additionalWordsProgressbar;

        private List<RewardInfo> _rewardCache = new(2);

        public WordsLevelAdditionalWordsPresenter(
            IWordsDictionaryService wordsDictionaryService,
            IWordsLevelsService wordsLevelsService,
            ILevelProgressData levelProgressData,
            IAdditionalWordsData additionalWordsData,
            IAdditionalWordsService additionalWordsService,
            IRewardService rewardService)
        {
            _wordsDictionaryService = wordsDictionaryService;
            _wordsLevelsService = wordsLevelsService;
            _additionalWordsData = additionalWordsData;
            _additionalWordsService = additionalWordsService;
            _levelProgressData = levelProgressData;
            _rewardService = rewardService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _wordsLevelWindow);
            
            _wordsContainer = _wordsLevelWindow.WordsContainer;
            _additionalWordsProgressbar = _wordsLevelWindow.AdditionalWordProgress;

            var disposableBuilder = Disposable.CreateBuilder();

            _wordSelector = _wordsLevelWindow.WordSelector;

            Observable.FromEvent<WordSelector>(
                    h => _wordSelector.OnWordSelected += h,
                    h => _wordSelector.OnWordSelected -= h).Subscribe(OnWordSelected)
                .AddTo(ref disposableBuilder);

            Observable.FromEvent<int>(h => _additionalWordsData.OnWordsChanged += h,
                h => _additionalWordsData.OnWordsChanged -= h).Subscribe(OnWordOpened).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _levelProgressData.OnCurrentLevelChanged += h,
                h => _levelProgressData.OnCurrentLevelChanged -= h).Subscribe(OnCurrentLevelChanged).AddTo(ref disposableBuilder);
            
            _wordsLevelWindow.AdditionalWordProgress.OnClick.AsObservable().Subscribe(OnClickAdditionalWords).AddTo(ref disposableBuilder);
            _disposable = disposableBuilder.Build();

            InitializeLevel();
        }
        
        private void OnWordOpened(int count)
        {
            PlayAnimation();
        }
        
        private void OnCurrentLevelChanged(Unit _)
        {
            _additionalWordsData.ClearWords();
        }
        
        private void UpdateProgressBar()
        {
            var currentLevel = _additionalWordsData.GetCurrentProgressLevel();
            _additionalWordsService.TryGetLevelInfo(currentLevel, out var levelInfo);
            _additionalWordsProgressbar.SetProgress(_additionalWordsData.GetCurrentProgressWords(),
                levelInfo.RequiredWordsCount, true);
        }
        
        private void OnClickAdditionalWords(Unit _)
        {
            WindowsService.Show<AdditionalWordsWindow>();
        }
        
        private void InitializeLevel()
        {
            var currentLevel = _levelProgressData.GetCurrentLevel();

            if (_wordsLevelsService.TryGetLevel(currentLevel, out _wordsLevel) == false)
                return;

            _additionalWordsData.InitializeLevel();
            _additionalWordsProgressbar.SetProgress(0f);
            
            UpdateProgressBar();

            TrAddRewards();
        }

        private void OnWordSelected(WordSelector wordSelector)
        {
            var selectedWord = wordSelector.SelectedWord;

            if (selectedWord.Length < _wordsContainer.MinSizeWord)
                return;

            if (_wordsLevel.IsWordContains(selectedWord))
                return;

            if (_additionalWordsData.IsWordOpened(selectedWord))
                return;

            if (_wordsDictionaryService.Contains(selectedWord) == false)
                return;

            _additionalWordsData.OpenWord(selectedWord);

            TrAddRewards();
        }
        
        private void TrAddRewards()
        {
            var currentLevel = _additionalWordsData.GetCurrentProgressLevel();
            var currentCountWords = _additionalWordsData.GetCurrentProgressWords();
            _additionalWordsService.TryGetLevelInfo(currentLevel, out var levelInfo);
            var diffCount = currentCountWords - levelInfo.RequiredWordsCount;

            if (diffCount >= 0)
            {
                _rewardCache.Clear();
             
                while (diffCount >= 0)
                {
                    var reward = levelInfo.Reward;
                    if (reward != null)
                    {
                        _rewardCache.Add(levelInfo.Reward);
                    }

                    _additionalWordsData.SetCurrentLevel(++currentLevel, diffCount);
                    _additionalWordsService.TryGetLevelInfo(currentLevel, out levelInfo);
                    diffCount -= levelInfo.RequiredWordsCount;
                }
                
                AddReward(_rewardCache);
            }
        }

        private void PlayAnimation()
        {
            var sequence = DOTween.Sequence()
                .AppendCallback(() => _wordsLevelWindow.AdditionalWordAnimator.PlayAnimation(
                    _wordsLevelWindow.WordViewer.Container,
                    _wordsLevelWindow.AdditionalWordProgress.RectTransform))
                .AppendInterval(_wordsLevelWindow.AdditionalWordAnimator.Duration)
                .AppendCallback(_wordsLevelWindow.AdditionalWordProgress.PlayPumpAnimation)
                .AppendCallback(UpdateProgressBar);

            sequence.Play();
        }

        private void AddReward(List<RewardInfo> rewardInfos)
        {
            foreach (var rewardInfo in rewardInfos)
            {
                _rewardService.TryCollectReward(rewardInfo);
            }
            
            _rewardService.SetAvailableMultipleReward(2);
            _rewardService.ShowScreenReward(rewardInfos);
        }

        private void StopAllAnimations()
        {
          
        }

        public void Dispose()
        {
            StopAllAnimations();
            
            _disposable?.Dispose();
        }
    }
}