using System;
using _Client.Scripts.GameLoop.Data.AdditionalWordsProgress;
using _Client.Scripts.Infrastructure.Services.AdditionalWordsService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordsPresenter : IStartable, IDisposable
    {
        private AdditionalWordsWindow _additionalWordsWindow;
        private IDisposable _disposable;
        private readonly IAdditionalWordsData _additionalWordsData;
        private readonly IAdditionalWordsService _additionalWordsService;

        public AdditionalWordsPresenter(IAdditionalWordsData additionalWordsData, IAdditionalWordsService additionalWordsService)
        {
            _additionalWordsData = additionalWordsData;
            _additionalWordsService = additionalWordsService;
        }
        public void Start()
        {
            WindowsService.TryGetWindow(out _additionalWordsWindow);
            
            var disposableBuilder = Disposable.CreateBuilder();
            
            _additionalWordsWindow.ButtonExit.OnClick.AsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _additionalWordsWindow.PanelExit.OnClickAsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            Observable.FromEvent(h => _additionalWordsWindow.OnBeforeShow += h, h => _additionalWordsWindow.OnBeforeShow -= h)
                .Subscribe(AdditionalWindowShowed).AddTo(ref disposableBuilder);
            
            Observable.FromEvent<int>(h => _additionalWordsData.OnWordsChanged += h, h => _additionalWordsData.OnWordsChanged -= h)
                .Subscribe(AdditionalWordsChanged).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }
        
        private void AdditionalWordsChanged(int count)
        {
            if(_additionalWordsWindow.IsShow() == false)
                return;
        }
        
        private void AdditionalWindowShowed(Unit _)
        {
            _additionalWordsWindow.AdditionalWordsContainer.AddWords(_additionalWordsData.GetLevelRecord().OpenedWords);
            _additionalWordsService.TryGetLevelInfo(_additionalWordsData.GetCurrentProgressLevel(), out var levelInfo);
            _additionalWordsWindow.Progressbar.SetProgress(_additionalWordsData.GetCurrentProgressWords(), levelInfo.RequiredWordsCount, true);
        }

        private void OnClickClose(Unit _)
        {
            _additionalWordsWindow.Hide();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}