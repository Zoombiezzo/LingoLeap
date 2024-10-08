using System;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using AssetKits.ParticleImage;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public abstract class LocationPreview : MonoBehaviour
    {
        [SerializeField] protected Canvas _canvas;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected ParticleImage _progressParticle;
        [SerializeField] protected UIProgressbar _progressbar;
        [SerializeField] protected Image _buttonSelectedImage;
        [SerializeField] protected Image _buttonSelectImage;
        [SerializeField] protected TMP_Text _buttonSelectText;
        [SerializeField] protected CanvasGroup _buttonCanvasGroup;
        [SerializeField] protected Image _lockImage;
        [SerializeField] protected CanvasGroup _lockCanvasGroup;
        [SerializeField] protected AnimationButton _buttonSelect;
        
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _selectedLocalizationKey;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        protected string _selectLocalizationKey;
        
        protected ILocalizationService _localizationService;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private IDisposable _localizationDisposable;
        
        protected string _id;
        protected bool _isSelected;
        public event Action<LocationPreview> OnSelectClick;
        public string Id => _id;
        public RectTransform RectTransform => _rectTransform;
        
        public void SetId(string id)
        {
            _id = id;
        }

        public void SetBlockedColor(Color itemBlockedColor)
        {
            _lockImage.color = itemBlockedColor;
        }

        public void RegisterLocalization(ILocalizationService localizationService)
        {
            _localizationDisposable?.Dispose();
            
            _localizationService = localizationService;

            _localizationDisposable = Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h,
                h => _localizationService.OnLanguageChanged -= h).Subscribe(OnLanguageChanged);
        }
        
        public void ShowProgressbar(bool show)
        {
            if (show)
            {
                _progressbar.Show();
                _progressParticle.Play();
            }
            else
            {
                _progressbar.Hide();
                _progressParticle.Stop();
                _progressParticle.Clear();
            }
        }
        
        public void SetProgress(int count, int maxCount, bool animate = false)
        {
            _progressbar.SetProgress(count, maxCount, animate);
        }

        public void SetOpenedState()
        {
            ShowButtonSelect(true);
            ShowProgressbar(false);
            ShowClosedPanel(false);
        }

        public void SetProgressState()
        {
            ShowButtonSelect(false);
            ShowProgressbar(true);
            ShowClosedPanel(false);
        }

        public void SetClosedState()
        {
            ShowProgressbar(false);
            ShowButtonSelect(false);
            ShowClosedPanel(true);
        }

        public void ShowButtonSelect(bool show)
        {
            _buttonCanvasGroup.alpha = show ? 1f : 0f;
            _buttonCanvasGroup.interactable = show;
            _buttonCanvasGroup.blocksRaycasts = show;
        }
        
        public virtual void ShowClosedPanel(bool show)
        {
            _lockCanvasGroup.alpha = show ? 1f : 0f;
            _lockCanvasGroup.interactable = show;
            _lockCanvasGroup.blocksRaycasts = show;
        }
        
        public void SetButtonStateSelected(bool selected)
        {
            _buttonSelectedImage.enabled = selected;
            _buttonSelectImage.enabled = !selected;

            _isSelected = selected;
            UpdateText();
        }

        public void SetVisible(bool visible) => _canvasGroup.alpha = visible ? 1f : 0f;

        private void OnLanguageChanged(string _)
        {
            UpdateText();
        }
        
        private void UpdateText()
        {
            var key = _isSelected ? _selectedLocalizationKey : _selectLocalizationKey;
            _buttonSelectText.text = _localizationService.GetValue(key);
        }

        private void OnEnable()
        {
            _disposables.Add(_buttonSelect.OnClick.AsObservable().Subscribe(OnSelect));
        }
        
        private void OnDisable()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
                _disposables.Clear();
            }
        }
        
        private void OnDestroy()
        {
            _localizationDisposable?.Dispose();
        }

        private void OnSelect(Unit _)
        {
            OnSelectClick?.Invoke(this);
        }
    }
}