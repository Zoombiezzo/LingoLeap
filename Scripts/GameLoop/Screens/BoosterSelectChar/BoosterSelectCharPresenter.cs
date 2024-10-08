using System;
using System.Collections;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Boosters;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.BoosterSelectChar.PurchaseDataFactories;
using _Client.Scripts.GameLoop.Screens.Shop;
using _Client.Scripts.GameLoop.Screens.WordsLevel;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar
{
    public class BoosterSelectCharPresenter :  IStartable, IDisposable
    {
        private WordsLevelWindow _wordsLevelWindow;
        private BoosterSelectCharWindow _boosterSelectCharWindow;
        private BoosterCharsContainer _boosterCharsContainer;
        private BoosterCharAnimator _boosterCharAnimator;
        
        private IDisposable _disposable;
        private readonly ILevelProgressData _levelProgressData;
        private readonly IPlayerProgressData _playerProgressData;
        private readonly IPurchaseService _purchaseService;
        
        private readonly HashSet<BoosterCharView> _selectedBoosterCharViewSet = new(4);
        
        private readonly Dictionary<CurrencyType, IPurchaseDataFactory> _purchaseDataFactories;
        private readonly PurchaseItemReceiver _purchaseItemReceiver;
        
        private Coroutine _coroutine;

        private bool _isUsed = false;
        private Sequence _sequence;

        public BoosterSelectCharPresenter(IPlayerProgressData playerProgressData, IPurchaseService purchaseService, ILevelProgressData levelProgressData)
        {
            _playerProgressData = playerProgressData;
            _levelProgressData = levelProgressData;
            _purchaseService = purchaseService;
            
            _purchaseDataFactories = new Dictionary<CurrencyType, IPurchaseDataFactory>()
            {
                {CurrencyType.BoosterSelectChar, new GameCurrencyDataPurchaseFactory()},
            };
            
            _purchaseItemReceiver = new PurchaseItemReceiver(PurchaseResult);
        }

        public void Start()
        {
            WindowsService.TryGetWindow(out _wordsLevelWindow);
            WindowsService.TryGetWindow(out _boosterSelectCharWindow);
            _boosterCharsContainer = _boosterSelectCharWindow.BoosterCharsContainer;
            _boosterCharAnimator = _wordsLevelWindow.BoosterCharAnimator;
            
            var disposableBuilder = Disposable.CreateBuilder();

            _boosterSelectCharWindow.HideButton.OnClick.AsObservable().Subscribe(OnHideClick)
                .AddTo(ref disposableBuilder);
            
            _boosterSelectCharWindow.OpenButton.OnClick.AsObservable().Subscribe(OnOpenClick)
                .AddTo(ref disposableBuilder);
            
            var button = _wordsLevelWindow.BoosterSelectCharButton;
            Observable.FromEvent(h => button.OnClick += h, h => button.OnClick -= h).Subscribe(OnBoosterSelectCharClick)
                .AddTo(ref disposableBuilder);

            Observable.FromEvent(h => _boosterSelectCharWindow.OnBeforeShow += h, h => _boosterSelectCharWindow.OnBeforeShow -= h)
                .Subscribe(OnBeforeShowScreen).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _boosterSelectCharWindow.OnHide += h, h => _boosterSelectCharWindow.OnHide -= h)
                .Subscribe(OnHideScreen).AddTo(ref disposableBuilder);

            Observable.FromEvent<BoosterCharView>(h => _boosterCharsContainer.OnClickedChar += h,
                h => _boosterCharsContainer.OnClickedChar -= h).Subscribe(OnClickChar).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }
        
        private void OnBoosterSelectCharClick(Unit _)
        {
            if (_playerProgressData.BoosterSelectChar.CurrentValue <= 0)
            {
                WindowsService.TryGetWindow<ShopWindow>(out var shopWindow);
                shopWindow.SetTargetCategoryId("booster_select_char_currency_category");
                WindowsService.Show<ShopWindow>();
                return;
            }
            
            _boosterSelectCharWindow.Show();
        }
        
        private void OnHideScreen(Unit _)
        {
            _selectedBoosterCharViewSet.Clear();
            _boosterSelectCharWindow.OpenToggleElement.Set(false);
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
                _purchaseDataFactories[CurrencyType.BoosterSelectChar].Create(CurrencyType.BoosterSelectChar,
                    _selectedBoosterCharViewSet.Count), _purchaseItemReceiver);
        }
        
        
        private void PurchaseResult(bool result)
        {
            if (result)
            {
                OpenChars();
            }
        }

        private void OpenChars()
        {
            foreach (var boosterCharView in _selectedBoosterCharViewSet)
            {
                if(_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                    continue;
            
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
            
            var wordsContainer = _wordsLevelWindow.WordsContainer;
            
            foreach (var boosterCharView in _selectedBoosterCharViewSet)
            {
                if(_boosterCharsContainer.TryGetWordAndChar(boosterCharView, out var word, out var indexChar) == false)
                    return;
                
                if(_boosterCharsContainer.TryGetCharView(boosterCharView, out var charView) == false)
                    return;
                
                wordsContainer.SetCharState(word, indexChar, CharViewState.Shown);

                var sequenceChar = DOTween.Sequence()
                    .Append(_boosterCharAnimator.PlayAnimation(charView.RectTransform, out var duration));
                    
                var sequenceBoosterChar = DOTween.Sequence()
                    .AppendInterval(duration)
                    .AppendCallback(() => charView.ShowChar());
                
                sequenceChar.Join(sequenceBoosterChar);

                _sequence.Join(sequenceChar);
            }
        }

        private void OnClickChar(BoosterCharView boosterCharView)
        {
            if (boosterCharView.State == State.Selected)
            {
                if (_selectedBoosterCharViewSet.Contains(boosterCharView))
                {
                    _selectedBoosterCharViewSet.Remove(boosterCharView);
                }
                
                boosterCharView.SetState(State.None);
                boosterCharView.DeselectAnimation();
                UpdateButtonOpenState();
                return;
            }
            
            if(_selectedBoosterCharViewSet.Count >= _playerProgressData.BoosterSelectChar.CurrentValue)
                return;
                
            boosterCharView.SetState(State.Selected);
            boosterCharView.SelectAnimation();
            _selectedBoosterCharViewSet.Add(boosterCharView);
            
            UpdateButtonOpenState();
        }

        private void OnBeforeShowScreen(Unit _)
        {
            ShowPanel();
        }

        private void UpdateButtonOpenState()
        {
            if (_selectedBoosterCharViewSet.Count > 0)
            {
                _boosterSelectCharWindow.CountObjectButton.SetActive(true);
                _boosterSelectCharWindow.CountText.text = _selectedBoosterCharViewSet.Count.ToString();
                _boosterSelectCharWindow.OpenToggleElement.Set(true, true);
                _boosterSelectCharWindow.ParticleImageButtonPlay.Play();
                return;
            }

            _boosterSelectCharWindow.OpenToggleElement.Set(false, true);
            _boosterSelectCharWindow.CountObjectButton.SetActive(false);
            _boosterSelectCharWindow.ParticleImageButtonPlay.Stop();
            _boosterSelectCharWindow.ParticleImageButtonPlay.Clear();
        }

        private void ShowPanel()
        {
            _isUsed = false;

            _selectedBoosterCharViewSet.Clear();

            var words = _wordsLevelWindow.WordsContainer.GetWords();
            _boosterCharsContainer.Initialize(words);
            _wordsLevelWindow.WordsSelectPanelUiElement.Hide();
            UpdateButtonOpenState();
            
            _boosterSelectCharWindow.CounterBoosterField.SetValue(_playerProgressData.BoosterSelectChar.CurrentValue);

            if (_coroutine != null)
            {
                _boosterSelectCharWindow.StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            _coroutine = _boosterSelectCharWindow.StartCoroutine(UpdatePositions());
        }

        private void HidePanel()
        {
            _boosterSelectCharWindow.Hide();
            _wordsLevelWindow.WordsSelectPanelUiElement.Show();
            
            if (_coroutine != null)
            {
                _boosterSelectCharWindow.StopCoroutine(_coroutine);
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