using System;
using _Client.Scripts.GameLoop.Screens.LanguageSelect;
using _Client.Scripts.GameLoop.Screens.SignIn;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.Services.AuthService;
using _Client.Scripts.Infrastructure.Services.RateService;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using UnityEngine;
using VContainer.Unity;
using AudioType = _Client.Scripts.Infrastructure.AudioSystem.Scripts.AudioType;

namespace _Client.Scripts.GameLoop.Screens.Settings
{
    public class SettingsPresenter : IStartable, IDisposable
    {
        private SettingsWindow _settingsWindow;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IStorageService _storageService;
        private readonly IAuthService _authService;
        private readonly IRateService _rateService;

        private IDisposable _disposable;

        public SettingsPresenter(IStorageService storageService, IAuthService authService, IRateService rateService)
        {
            _storageService = storageService;
            _authService = authService;
            _rateService = rateService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _settingsWindow);

            var disposableBuilder = Disposable.CreateBuilder();
            
            _settingsWindow.ClosePanel.OnClickAsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _settingsWindow.CloseButton.OnClick.AsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _settingsWindow.SignInButton.OnClick.AsObservable().Subscribe(OnSignIn).AddTo(ref disposableBuilder);
            _settingsWindow.LanguageButton.OnClick.AsObservable().Subscribe(OnClickLanguageButton).AddTo(ref disposableBuilder);
            _settingsWindow.RateButton.OnClick.AsObservable().Subscribe(OnRateClick).AddTo(ref disposableBuilder);

            Observable.FromEvent<bool>(h => _settingsWindow.ToggleMusic.OnValueChanged += h,
                    h => _settingsWindow.ToggleMusic.OnValueChanged -= h).Subscribe(OnMusicValueChanged)
                .AddTo(ref disposableBuilder);
            
            Observable.FromEvent<bool>(h => _settingsWindow.ToggleSound.OnValueChanged += h,
                    h => _settingsWindow.ToggleSound.OnValueChanged -= h).Subscribe(OnSoundValueChanged)
                .AddTo(ref disposableBuilder);
            
            Observable.FromEvent<SignInType>(h => _authService.OnSignInTypeChanged += h, h => _authService.OnSignInTypeChanged -= h)
                .Subscribe(OnSignInTypeChanged).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _rateService.OnRatedSuccess += h, h => _rateService.OnRatedSuccess -= h)
                .Subscribe(OnRateSuccess).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
            
            _settingsWindow.ToggleMusic.SetValue(Mathf.Approximately(AudioService.GetVolume(AudioType.Music), 1f), false);
            _settingsWindow.ToggleSound.SetValue(Mathf.Approximately(AudioService.GetVolume(AudioType.Sound), 1f), false);
            _settingsWindow.VersionText.text = Application.version;
            
            UpdateAuth();
            UpdateRate();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
        
        private void OnMusicValueChanged(bool value)
        {
            AudioService.SetVolume(AudioType.Music, value ? 1f : 0f);
        }
        
        private void OnSoundValueChanged(bool value)
        {
            AudioService.SetVolume(AudioType.Sound, value ? 1f : 0f);
        }
        
        private void OnClickClose(Unit _)
        {
            _storageService.Save<AudioService>();
            
            _settingsWindow.Hide();
        }
        
        private void OnClickLanguageButton(Unit _)
        {
            WindowsService.Show<LanguageSelectWindow>();
        }
        
        private void OnSignIn(Unit _)
        {
            WindowsService.Show<SignInWindow>();
        }
        
        private void OnSignInTypeChanged(SignInType signInType)
        {
            UpdateAuth();
        }
        
        private void UpdateAuth()
        {
            _settingsWindow.SignInButtonGO.gameObject.SetActive(_authService.SignInType != SignInType.Account);
        }

        private void OnRateSuccess(Unit _)
        {
            UpdateRate();
        }
        
        private void UpdateRate()
        {
            _settingsWindow.RateButtonGO.gameObject.SetActive(_rateService.Rated == false);
        }
        
        private void OnRateClick(Unit _)
        {
            _rateService.Rate();
        }
    }
}