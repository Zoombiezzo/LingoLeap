using System;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.WordsLevel
{
    public class WordsLevelCurrencyPresenter : IStartable, IDisposable
    {
        private readonly IPlayerProgressData _playerProgressData;
        private WordsLevelWindow _wordsLevelWindow;
        private IDisposable _disposable;

        public WordsLevelCurrencyPresenter(IPlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _wordsLevelWindow);

            _wordsLevelWindow.CoinsCounter.SetValue(_playerProgressData.Soft.CurrentValue);
            _wordsLevelWindow.BoosterSelectCharButton.SetValue(_playerProgressData.BoosterSelectChar.CurrentValue);
            _wordsLevelWindow.BoosterSelectWordButton.SetValue(_playerProgressData.BoosterSelectWord.CurrentValue);

            var disposableBuilder = Disposable.CreateBuilder();

            _playerProgressData.Soft.Subscribe(OnSoftChanged).AddTo(ref disposableBuilder);
            _playerProgressData.BoosterSelectChar.Subscribe(OnBoosterSelectCharChanged).AddTo(ref disposableBuilder);
            _playerProgressData.BoosterSelectWord.Subscribe(OnBoosterSelectWordChanged).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }
        
        private void OnSoftChanged(int value)
        {
            _wordsLevelWindow.CoinsCounter.SetValue(value, true);
        }
        
        private void OnBoosterSelectCharChanged(int value)
        {
            _wordsLevelWindow.BoosterSelectCharButton.SetValue(value, true);
        }
        
        private void OnBoosterSelectWordChanged(int value)
        {
            _wordsLevelWindow.BoosterSelectWordButton.SetValue(value, true);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}