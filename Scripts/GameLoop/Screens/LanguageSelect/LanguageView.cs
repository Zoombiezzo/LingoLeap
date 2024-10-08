using System;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.LanguageSelect
{
    public class LanguageView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TMP_Text _languageText;
        [SerializeField] private AnimationButton _animationButton;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unselectedSprite;

        private ILocalizationInfo _languageInfo;
        private IDisposable _disposable;

        public event Action<LanguageView> OnSelect;
        
        public ILocalizationInfo LanguageInfo => _languageInfo;

        public void Initialize(ILocalizationInfo languageInfo, string language, Sprite sprite)
        {
            _languageInfo = languageInfo;
            _languageText.text = language;
            _icon.sprite = sprite;
        }
        
        public void SetSelected(bool selected)
        {
            _buttonImage.sprite = selected ? _selectedSprite : _unselectedSprite;
        }

        private void OnEnable()
        {
            _disposable = _animationButton.OnClick.AsObservable()
                .Subscribe(OnClickLanguage);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void OnClickLanguage(Unit _)
        {
            OnSelect?.Invoke(this);
        }
    }
}