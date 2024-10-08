using System;
using System.Collections;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Boosters;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.BoosterSelectWord.PurchaseDataFactories;
using _Client.Scripts.GameLoop.Screens.Shop;
using _Client.Scripts.GameLoop.Screens.WordsLevel;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.WalletService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public class BoosterSelectWordPresenter :  IStartable, IDisposable
    {
        private WordsLevelWindow _wordsLevelWindow;
        private BoosterSelectWordWindow _boosterSelectWordWindow;
        private BoosterCharsContainer _boosterCharsContainer;
        
        private IDisposable _disposable;
        private readonly ILevelProgressData _levelProgressData;
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IPurchaseService _purchaseService;
        
        private readonly HashSet<BoosterCharView> _selectedBoosterCharViewSet = new(4);
        private readonly Dictionary<string, Sequence> _playedSequences = new(4);
        private readonly List<BoosterCharView> _cachedListAnimation = new(8);
        
        private readonly Dictionary<CurrencyType, IPurchaseDataFactory> _purchaseDataFactories;
        
        private Coroutine _coroutine;
        private BoosterCharView _selectedBoosterCharView;
        private BoosterWordAnimator _boosterWordAnimator;
        private bool _isUsed = false;

        private readonly PurchaseItemReceiver _purchaseItemReceiver;

        private readonly List<RectTransform> _cachedListTargets = new List<RectTransform>(8);

        private Sequence _sequence;

        public BoosterSelectWordPresenter(IPlayerProgressData playerProgressData, IPurchaseService purchaseService, ILevelProgressData levelProgressData)
        {
            _playerProgressData = playerProgressData;
            _levelProgressData = levelProgressData;
            _purchaseService = purchaseService;
            
            _purchaseDataFactories = new Dictionary<CurrencyType, IPurchaseDataFactory>
            {
                { CurrencyType.BoosterSelectWord, new GameCurrencyDataPurchaseFactory() }
            };
            
            _purchaseItemReceiver = new PurchaseItemReceiver(PurchaseResult);
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _wordsLevelWindow);
            WindowsService.TryGetWindow(out _boosterSelectWordWindow);
            _boosterCharsContainer = _boosterSelectWordWindow.BoosterCharsContainer;
            _boosterWordAnimator = _wordsLevelWindow.BoosterWordAnimator;
            
            var disposableBuilder = Disposable.CreateBuilder();

            _boosterSelectWordWindow.HideButton.OnClick.AsObservable().Subscribe(OnHideClick)
                .AddTo(ref disposableBuilder);
            
            _boosterSelectWordWindow.OpenButton.OnClick.AsObservable().Subscribe(OnOpenClick)
                .AddTo(ref disposableBuilder);
            
            var button = _wordsLevelWindow.BoosterSelectWordButton;
            Observable.FromEvent(h => button.OnClick += h, h => button.OnClick -= h).Subscribe(OnBoosterSelectWordClick)
                .AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _boosterSelectWordWindow.OnBeforeShow += h, h => _boosterSelectWordWindow.OnBeforeShow -= h)
                .Subscribe(OnBeforeShowScreen).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _boosterSelectWordWindow.OnHide += h, h => _boosterSelectWordWindow.OnHide -= h)
                .Subscribe(OnHideScreen).AddTo(ref disposableBuilder);

            Observable.FromEvent<BoosterCharView>(h => _boosterCharsContainer.OnClickedChar += h,
                h => _boosterCharsContainer.OnClickedChar -= h).Subscribe(OnClickChar).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }

        private void OnBoosterSelectWordClick(Unit _)
        {
            if (_playerProgressData.BoosterSelectWord.CurrentValue <= 0)
            {
                WindowsService.TryGetWindow<ShopWindow>(out var shopWindow);
                shopWindow.SetTargetCategoryId("booster_select_word_currency_category");
                WindowsService.Show<ShopWindow>();
                return;
            }
            
            _boosterSelectWordWindow.Show();
        }
        
        private void OnHideScreen(Unit _)
        {
            _selectedBoosterCharViewSet.Clear();
            _boosterCharsContainer.ReleaseAll();
        }
        
        private void OnHideClick(Unit _)
        {
            HidePanel();
        }

        private void OnOpenClick(Unit _)
        {
            if(_isUsed)
                return;
            
            if(_selectedBoosterCharViewSet.Count == 0)
                return;
            
            _purchaseService.Purchase(
                _purchaseDataFactories[CurrencyType.BoosterSelectWord].Create(CurrencyType.BoosterSelectWord, 1),
                _purchaseItemReceiver);
        }

        private void PurchaseResult(bool success)
        {
            if (success)
            {
                OpenWord();
            }
        }

        private void OpenWord()
        {
            foreach (var boosterCharView in _selectedBoosterCharViewSet)
            {
                if(_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                    return;
            
                _levelProgressData.OpenCharIndex(word,indexChar);
            }
            
            PlayAnimation();
            HidePanel();
            
            _isUsed = true;
        }

        private void PlayAnimation()
        {
            if(_sequence != null && _sequence.IsActive())
                _sequence.Kill();
            
            _sequence = DOTween.Sequence();
            _sequence.Pause();

            var index = 0;

            _cachedListTargets.Clear();
            var wordsContainer = _wordsLevelWindow.WordsContainer;
            
            foreach (var boosterCharView in _selectedBoosterCharViewSet)
            {
                if (_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                    return;

                if (_boosterCharsContainer.TryGetCharView(boosterCharView, out var charView) == false)
                    return;
                
                wordsContainer.SetCharState(word, indexChar, CharViewState.Shown);
                
                var charRectTransform = charView.RectTransform;
                var delay = _boosterWordAnimator.Duration * ((float)index / _selectedBoosterCharViewSet.Count);

                var sequenceChar = DOTween.Sequence()
                    .AppendInterval(delay)
                    .Append(_boosterWordAnimator.PlayAnimation(charRectTransform, out var duration));

                var sequenceBoosterChar = DOTween.Sequence()
                    .AppendInterval(duration)
                    .AppendCallback(() => charView.ShowChar());

                sequenceChar.Join(sequenceBoosterChar);

                _sequence.Join(sequenceChar);

                index++;

                _cachedListTargets.Add(charRectTransform);
            }

            var durationOnCell = _boosterWordAnimator.Duration / _cachedListTargets.Count * 0.75f;
            _sequence.Join(
                _boosterWordAnimator.PlayPenAnimation(_cachedListTargets, durationOnCell / 2f, durationOnCell / 2f));

            _sequence.Play();
        }

        private void OnClickChar(BoosterCharView boosterCharView)
        {
            if (boosterCharView.State == State.Selected)
            {
                DeselectWord(boosterCharView);
                UpdateButtonOpenState();
                return;
            }

            if (_selectedBoosterCharView != null)
            {
                DeselectWord(_selectedBoosterCharView);
            }
                
            SelectWord(boosterCharView);
            UpdateButtonOpenState();
        }

        private void DeselectWord(BoosterCharView boosterCharView)
        {
            _cachedListAnimation.Clear();
            
            if(_boosterCharsContainer.GetWordByChar(boosterCharView, out var boosterCharViews) == false)
                return;
            
            if(_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                return;

            foreach (var charView in boosterCharViews)
            {
                charView.SetState(State.None);
                _cachedListAnimation.Add(charView);
            }
            
            _selectedBoosterCharViewSet.Clear();
            _selectedBoosterCharView = null;
            
            DeselectAnimation(word, _cachedListAnimation.IndexOf(boosterCharView), _cachedListAnimation);
        }

        private void DeselectAnimation(string word, int startIndex, IReadOnlyList<BoosterCharView> boosterCharViews)
        {
            if (_playedSequences.TryGetValue(word, out var sequence))
            {
                if (sequence.IsPlaying())
                {
                    sequence.Kill();
                }
                _playedSequences.Remove(word);
            }
            
            sequence = DOTween.Sequence();

            int left = startIndex - 1;
            int right = startIndex + 1;
            
            float delay = _boosterSelectWordWindow.MaxDelay / word.Length;

            sequence.Append(boosterCharViews[startIndex].DeselectAnimation());

            int maxCount = boosterCharViews.Count;
            int index = 0;
            
            while (left >= 0 || right < maxCount)
            {
                var sequenceEdgeWithDelay = DOTween.Sequence();
                sequenceEdgeWithDelay.AppendInterval(index * delay);
                
                var sequenceEdge = DOTween.Sequence();
                
                if (left >= 0)
                {
                    sequenceEdge.Join(boosterCharViews[left].DeselectAnimation());
                    left--;
                }

                if (right < maxCount)
                {
                    sequenceEdge.Join(boosterCharViews[right].DeselectAnimation());
                    right++;
                }

                sequenceEdgeWithDelay.Append(sequenceEdge);
                sequence.Join(sequenceEdgeWithDelay);
                
                index++;
            }
            
            _playedSequences.Add(word, sequence);
        }

        private void SelectWord(BoosterCharView boosterCharView)
        {
            _cachedListAnimation.Clear();
            
            if(_boosterCharsContainer.GetWordByChar(boosterCharView, out var boosterCharViews) == false)
                return;
            
            if(_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                return;
            
            _selectedBoosterCharViewSet.Clear();
            
            foreach (var charView in boosterCharViews)
            {
                charView.SetState(State.Selected);
                _selectedBoosterCharViewSet.Add(charView);
                _cachedListAnimation.Add(charView);
            }

            _selectedBoosterCharView = boosterCharView;
            
            SelectAnimation(word, _cachedListAnimation.IndexOf(boosterCharView), _cachedListAnimation);
        }
        
        private void SelectAnimation(string word, int startIndex, IReadOnlyList<BoosterCharView> boosterCharViews)
        {
            if (_playedSequences.TryGetValue(word, out var sequence))
            {
                if (sequence.IsPlaying())
                {
                    sequence.Kill();
                }
                _playedSequences.Remove(word);
            }
            
            sequence = DOTween.Sequence();

            int left = startIndex - 1;
            int right = startIndex + 1;
            
            float delay = _boosterSelectWordWindow.MaxDelay / word.Length;

            sequence.Append(boosterCharViews[startIndex].SelectAnimation());

            int maxCount = boosterCharViews.Count;
            int index = 0;
            
            while (left >= 0 || right < maxCount)
            {
                var sequenceEdgeWithDelay = DOTween.Sequence();
                sequenceEdgeWithDelay.AppendInterval(index * delay);
                
                var sequenceEdge = DOTween.Sequence();
                
                if (left >= 0)
                {
                    sequenceEdge.Join(boosterCharViews[left].SelectAnimation());
                    left--;
                }

                if (right < maxCount)
                {
                    sequenceEdge.Join(boosterCharViews[right].SelectAnimation());
                    right++;
                }

                sequenceEdgeWithDelay.Append(sequenceEdge);
                sequence.Join(sequenceEdgeWithDelay);
                
                index++;
            }
            
            _playedSequences.Add(word, sequence);
        }

        private void OnBeforeShowScreen(Unit _)
        {
            ShowPanel();
        }

        private void UpdateButtonOpenState()
        {
            if (_selectedBoosterCharViewSet.Count > 0)
            {
                _boosterSelectWordWindow.OpenToggleElement.Set(true, true);
                _boosterSelectWordWindow.ParticleImageButtonPlay.Play();
                return;
            }

            _boosterSelectWordWindow.OpenToggleElement.Set(false, true);
            _boosterSelectWordWindow.ParticleImageButtonPlay.Stop();
            _boosterSelectWordWindow.ParticleImageButtonPlay.Clear();
        }

        private void ShowPanel()
        {
            _isUsed = false;
            _selectedBoosterCharViewSet.Clear();
            _selectedBoosterCharView = null;

            UpdateButtonOpenState();
            
            _boosterSelectWordWindow.BoosterCounterField.SetValue(_playerProgressData.BoosterSelectWord.CurrentValue);
            
            var words = _wordsLevelWindow.WordsContainer.GetWords();
            _boosterCharsContainer.Initialize(words);
            _wordsLevelWindow.WordsSelectPanelUiElement.Hide();

            if (_coroutine != null)
            {
                _boosterSelectWordWindow.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            _coroutine = _boosterSelectWordWindow.StartCoroutine(UpdatePositions());
        }

        private void HidePanel()
        {
            _boosterSelectWordWindow.Hide();
            _wordsLevelWindow.WordsSelectPanelUiElement.Show();
            
            if (_coroutine != null)
            {
                _boosterSelectWordWindow.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator UpdatePositions()
        {
            while (true)
            {
                _boosterCharsContainer.UpdatePositions();
                yield return null;
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}