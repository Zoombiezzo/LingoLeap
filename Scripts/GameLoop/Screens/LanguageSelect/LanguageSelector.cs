using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using R3;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.LanguageSelect
{
    public class LanguageSelector : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private LanguageView _languageViewPrefab;

        private List<LanguageView> _languageViews = new(4);
        private Dictionary<string, LanguageView> _languageViewsMap = new(4);
        
        private LanguageView _selectedLanguageView;
            
        private List<IDisposable> _disposables = new();
        
        public event Action<LanguageView> OnSelected;
        
        public void CreateLanguageView(ILocalizationInfo languageInfo, string language, Sprite sprite)
        {
            var languageView = Instantiate(_languageViewPrefab, _content);
            languageView.Initialize(languageInfo, language, sprite);
            languageView.SetSelected(false);
            
            _languageViews.Add(languageView);
            _languageViewsMap.Add(languageInfo.LanguageCode, languageView);

            Observable.FromEvent<LanguageView>(h => languageView.OnSelect += h, h => languageView.OnSelect -= h)
                .Subscribe(OnSelectLanguageView).AddTo(_disposables);
        }
        
        public void SelectLanguage(string languageCode)
        {
            if(_languageViewsMap.TryGetValue(languageCode, out var languageView) == false)
                return;
            
            SelectLanguageView(languageView);
        }
        
        private void SelectLanguageView(LanguageView languageView)
        {
            if (_selectedLanguageView == languageView)
                return;
            
            _selectedLanguageView?.SetSelected(false);
            _selectedLanguageView = languageView;
            _selectedLanguageView.SetSelected(true);
            
            OnSelected?.Invoke(languageView);
        }
        
        private void OnSelectLanguageView(LanguageView languageView)
        {
            SelectLanguageView(languageView);
        }
        
        private void OnDestroy()
        {
            _disposables.ForEach(x => x.Dispose());
        }
        
        private void Clear()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            
            foreach (var languageView in _languageViews)
            {
                Destroy(languageView);
            }
            
            _languageViews.Clear();
            _languageViewsMap.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}