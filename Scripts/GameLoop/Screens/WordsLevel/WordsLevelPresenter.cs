using System;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using _Client.Scripts.GameLoop.Components.WordSelector;
using _Client.Scripts.GameLoop.Components.WordViewer;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.Shop;
using _Client.Scripts.Helpers;
using _Client.Scripts.Infrastructure.Services.ScoreCalculator;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using _Client.Scripts.Infrastructure.Services.WordsLevelsService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.WordsLevel
{
    public class WordsLevelPresenter : IStartable, IDisposable
    {
        private WordsLevelWindow _wordsLevelWindow;
        private WordViewer _wordViewer;
        private WordSelector _wordSelector;
        private WordsContainer _wordsContainer;

        private readonly IWordsDictionaryService _wordsDictionaryService;
        private readonly IWordsLevelsService _wordsLevelsService;
        private readonly ILevelProgressData _levelProgressData;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly IMindScoreCalculator _mindScoreCalculator;

        public int _maxWordsPerColumn = 10;
        
        private IDisposable _disposable;
        private ILevelRecord _levelRecord;
        private Infrastructure.Services.WordsLevelsService.WordsLevel _wordsLevel;

        private Sequence _successWordSequence;
        private Sequence _failWordSequence;
        private Sequence _hideWordSequence;

        public WordsLevelPresenter(IWordsDictionaryService wordsDictionaryService,
            IWordsLevelsService wordsLevelsService,
            ILevelProgressData levelProgressData,
            IGameStateMachine gameStateMachine,
            IPlayerProgressData playerProgressData,
            IAdditionalWordsData additionalWordsData)
        {
            _wordsDictionaryService = wordsDictionaryService;
            _wordsLevelsService = wordsLevelsService;
            _levelProgressData = levelProgressData;
            _gameStateMachine = gameStateMachine;
            _playerProgressData = playerProgressData;
            _additionalWordsData = additionalWordsData;
            _mindScoreCalculator = new MindScoreCalculator(_additionalWordsData, _levelProgressData);
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _wordsLevelWindow);

            var disposableBuilder = Disposable.CreateBuilder();

            _wordSelector = _wordsLevelWindow.WordSelector;
            _wordViewer = _wordsLevelWindow.WordViewer;
            _wordsContainer = _wordsLevelWindow.WordsContainer;

            InitializeSuccessAnimation();
            InitializeFailAnimation();
            InitializeHideAnimation();

            Observable.FromEvent<WordSelector>(
                    h => _wordSelector.OnCharsChanged += h,
                    h => _wordSelector.OnCharsChanged -= h).Subscribe(OnWordsChanged)
                .AddTo(ref disposableBuilder);

            Observable.FromEvent<WordSelector>(
                    h => _wordSelector.OnWordSelected += h,
                    h => _wordSelector.OnWordSelected -= h).Subscribe(OnWordSelected)
                .AddTo(ref disposableBuilder);

            Observable.FromEvent<WordSelector>(
                h => _wordSelector.OnShuffle += h,
                h => _wordSelector.OnShuffle -= h).Subscribe(OnShuffleClick)
                .AddTo(ref disposableBuilder);

            Observable.FromEvent<string>(h => _levelProgressData.OnWordOpened += h,
                h => _levelProgressData.OnWordOpened -= h).Subscribe(OnWorldOpened).AddTo(ref disposableBuilder);
            
            _wordsLevelWindow.ButtonNextLevel.OnClickAsObservable().Subscribe(OnClickNextLevel).AddTo(ref disposableBuilder);
            _wordsLevelWindow.ButtonOpenWord.OnClickAsObservable().Subscribe(OnClickOpenWord).AddTo(ref disposableBuilder);
            _wordsLevelWindow.ButtonHome.OnClick.AsObservable().Subscribe(OnClickHome).AddTo(ref disposableBuilder);
            _wordsLevelWindow.ButtonCoins.OnClick.AsObservable().Subscribe(OnClickCoins).AddTo(ref disposableBuilder);
            _wordsLevelWindow.ButtonReset.OnClickAsObservable().Subscribe(OnClickReset).AddTo(ref disposableBuilder);
            _disposable = disposableBuilder.Build();

            InitializeLevel();
            
            _wordsLevelWindow.AdaptiveUIGroup.UpdateGroups(true);

            _wordsContainer.ShowCellsAnimation();
            _wordSelector.Show(true);
            BlockScreen(false);
        }

        private void InitializeSuccessAnimation()
        {
            _successWordSequence = DOTween.Sequence();
            _successWordSequence.SetAutoKill(false);
            _successWordSequence.Pause();
            
            _successWordSequence.AppendCallback(_wordViewer.PlaySuccessWordAnimation);
            _successWordSequence.AppendInterval(_wordViewer.DurationSuccessAnimation);
            _successWordSequence.AppendCallback(_wordViewer.Hide);
            _successWordSequence.AppendInterval(_wordViewer.DurationHideAnimation);
            _successWordSequence.AppendCallback(_wordViewer.ClearWord);
        }
        
        private void InitializeFailAnimation()
        {
            _failWordSequence = DOTween.Sequence();
            _failWordSequence.SetAutoKill(false);
            _failWordSequence.Pause();
            
            _failWordSequence.AppendCallback(_wordViewer.PlayFailWordAnimation);
            _failWordSequence.AppendInterval(_wordViewer.DurationFailAnimation);
            _failWordSequence.AppendCallback(_wordViewer.Hide);
            _failWordSequence.AppendInterval(_wordViewer.DurationHideAnimation);
            _failWordSequence.AppendCallback(_wordViewer.ClearWord);
        }
        
        private void InitializeHideAnimation()
        {
            _hideWordSequence = DOTween.Sequence();
            _hideWordSequence.SetAutoKill(false);
            _hideWordSequence.Pause();
            
            _hideWordSequence.AppendCallback(_wordViewer.Hide);
            _hideWordSequence.AppendInterval(_wordViewer.DurationHideAnimation);
            _hideWordSequence.AppendCallback(_wordViewer.ClearWord);
        }

        private void OnClickCoins(Unit _)
        {
            WindowsService.Show<ShopWindow>();
        }

        private void OnClickReset(Unit _)
        {
            _levelProgressData.ResetLevel();
            _gameStateMachine.Enter<LoadNextLevelGameState>();
        }

        private void OnWorldOpened(string word)
        {
            TryShowEffectAlmostCompletedLevel();
            TryCompleteLevel();
        }
        
        private void InitializeLevel()
        {
            _levelRecord = _levelProgressData.GetLevelRecord();

            if (_wordsLevelsService.TryGetLevel(_levelRecord.LevelNumber, out _wordsLevel) == false)
                return;
            
            _wordSelector.SetChars(_wordsLevel.Chars);
            var words = _wordsLevel.Words;
            _wordsContainer.CreateWords(words, Mathf.CeilToInt((float)words.Length / _maxWordsPerColumn));
            _wordsLevelWindow.LevelText.text = $"{_levelRecord.LevelNumber}";
            
            foreach (var openedWord in _levelRecord.OpenedWords)
            {
                _wordsContainer.ShowWord(openedWord);
            }

            foreach (var openedChar in _levelRecord.OpenedChars)
            {
                if(_levelProgressData.IsWordOpened(openedChar.Word))
                    continue;

                foreach (var index in openedChar.Indexes)
                {
                    _wordsContainer.ShowChar(openedChar.Word, index);
                }
            }

            TryShowEffectAlmostCompletedLevel();
            TryCompleteLevel();
        }

        private void OnClickNextLevel(Unit _)
        {
            CompleteLevel();
        }
        
        private void OnClickOpenWord(Unit _)
        {
            foreach (var word in _wordsLevel.Words)
            {
                if (_wordsContainer.ContainsWord(word) == false)
                    continue;

                if (_levelProgressData.IsWordOpened(word))
                    continue;

                if (TryOpenWord(word))
                    break;
            }
        }
        
        private void OnClickHome(Unit _)
        {
            _gameStateMachine.Enter<TransitionFromGameToMainMenuState>();
        }

        private void OnWordSelected(WordSelector wordSelector)
        {
            var selectedWord = wordSelector.SelectedWord;
            TryOpenWord(selectedWord);
        }

        private bool TryOpenWord(string selectedWord)
        {
            if (selectedWord.Length < _wordsContainer.MinSizeWord)
            {
                PlayHideAnimation();
                return false;
            }

            if (_wordsContainer.ContainsWord(selectedWord))
            {
                bool isOpened = false;

                if (_levelProgressData.IsWordOpened(selectedWord) == false)
                {
                    _wordsContainer.ShowWord(selectedWord, true);
                    _levelProgressData.OpenWord(selectedWord);
                    isOpened = true;
                }
                else
                {
                    _wordsContainer.PumpWord(selectedWord);
                }

                PlaySuccessAnimation();
                TryShowEffectAlmostCompletedLevel();

                return isOpened;
            }

            if (_wordsDictionaryService.Contains(selectedWord))
            {
                PlaySuccessAnimation();
                return false;
            }
            
            PlayFailAnimation();
            return false;
        }

        private void TryShowEffectAlmostCompletedLevel()
        {
            if (_levelRecord.OpenedWords.Count >= _wordsLevel.Words.Length - 1)
            {
                _wordSelector.ShowEffect();
            }
        }
        
        private void OnShuffleClick(WordSelector wordSelector)
        {
            wordSelector.OverrideChars(wordSelector.Chars.Shuffle());
        }

        private void TryCompleteLevel()
        {
            if(_levelRecord.OpenedWords.Count != _wordsLevel.Words.Length)
                return;

            CompleteLevel();
        }
        
        private void BlockScreen(bool block)
        {
            _wordsLevelWindow.BlockerLevel.interactable = block;
            _wordsLevelWindow.BlockerLevel.blocksRaycasts = block;
        }

        private void CompleteLevel()
        {
            BlockScreen(true);

            _wordsLevelWindow.CompleteAnimation.Play();
            
            _playerProgressData.AddMindScore(_mindScoreCalculator.Calculate());

            _levelProgressData.CompleteLevel();
            
            _gameStateMachine.Enter<CompletedLevelState>();
        }

        private void OnWordsChanged(WordSelector wordSelector)
        {
            StopAllAnimations();
            
            _wordViewer.Show();
            _wordViewer.SetText(wordSelector.SelectedWord);
        }
        
        private void PlaySuccessAnimation()
        {
            _successWordSequence?.Restart();
            _successWordSequence?.Play();
        }
        
        private void PlayFailAnimation()
        {
            _failWordSequence?.Restart();
            _failWordSequence?.Play();
        }
        
        private void PlayHideAnimation()
        {
            _hideWordSequence?.Restart();
            _hideWordSequence?.Play();
        }
        
        private void StopAllAnimations()
        {
            _successWordSequence?.Pause();
            _failWordSequence?.Pause();
            _hideWordSequence?.Pause();
        }
        
        private void DisposeAllAnimations()
        {
            _successWordSequence?.Kill();
            _failWordSequence?.Kill();
            _hideWordSequence?.Kill();
            
            _successWordSequence = null;
            _failWordSequence = null;
            _hideWordSequence = null;
        }

        public void Dispose()
        {
            StopAllAnimations();
            DisposeAllAnimations();
            _disposable?.Dispose();
        }
    }
}