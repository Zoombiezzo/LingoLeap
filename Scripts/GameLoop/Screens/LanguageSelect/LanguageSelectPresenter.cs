using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.LanguageSelect
{
    public class LanguageSelectPresenter : IStartable, IDisposable
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISpriteDatabaseService _spriteDatabaseService;

        private LanguageSelectWindow _window;

        private IDisposable _disposable;
        
        public LanguageSelectPresenter(ILocalizationService localizationService, ISpriteDatabaseService spriteDatabaseService)
        {
            _localizationService = localizationService;
            _spriteDatabaseService = spriteDatabaseService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            
            CreateLocalizationButtons();
            
            var disposableBuilder = Disposable.CreateBuilder();

            Observable.FromEvent<LanguageView>(h => _window.LanguageSelector.OnSelected += h,
                    h => _window.LanguageSelector.OnSelected -= h)
                .Subscribe(OnLanguageSelected).AddTo(ref disposableBuilder);
            
            _window.ClosePanel.OnClickAsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnClickClose).AddTo(ref disposableBuilder);

            _disposable = disposableBuilder.Build();
        }

        public void Dispose()
        {
            _window.LanguageSelector.Dispose();
            _disposable?.Dispose();
        }
        
        private void OnClickClose(Unit _)
        {
            _window.Hide();
        }
        
        private void OnLanguageSelected(LanguageView languageView)
        {
            _localizationService.SetCurrentLanguage(languageView.LanguageInfo.LanguageCode);
        }

        private void CreateLocalizationButtons()
        {
            var languageSelector = _window.LanguageSelector;
            
            foreach (var localization in _localizationService.Localizations)
            {
                languageSelector.CreateLanguageView(localization,
                    _localizationService.GetValue(localization.LanguageNameTranslate),
                    _spriteDatabaseService.GetSprite(localization.LanguageFlagIcon));
            }
            
            languageSelector.SelectLanguage(_localizationService.CurrentLanguageCode);
        }
    }
}