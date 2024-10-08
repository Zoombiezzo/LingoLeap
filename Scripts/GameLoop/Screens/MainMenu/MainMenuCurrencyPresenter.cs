using System;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.MainMenu
{
    public class MainMenuCurrencyPresenter : IStartable, IDisposable
    {
        private readonly IPlayerProgressData _playerProgressData;
        private MainMenuWindow _window;
        private IDisposable _disposable;

        public MainMenuCurrencyPresenter(IPlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);

            _window.SoftCounterField.SetValue(_playerProgressData.Soft.CurrentValue);
            _window.MindScoreCounterField.SetValue(_playerProgressData.MindScore.CurrentValue);

            var disposableBuilder = Disposable.CreateBuilder();

            _playerProgressData.Soft.Subscribe(OnSoftChanged).AddTo(ref disposableBuilder);
            _playerProgressData.MindScore.Subscribe(OnMindScoreChanged).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }
        
        private void OnMindScoreChanged(int value)
        {
            _window.MindScoreCounterField.SetValue(value, true);
        }
        
        private void OnSoftChanged(int value)
        {
            _window.SoftCounterField.SetValue(value, true);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}