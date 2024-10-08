using System;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public class LocationCategoryPreview : MonoBehaviour
    {
        [SerializeField] private RectTransform _headerPanel;
        [SerializeField] private RectTransform _headerTextRectTransform;
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _header;
        [SerializeField] private Image _headerImage;
        [SerializeField] private RectOffset _padding;
        [SerializeField] private float _offsetAlpha = 200f;
        
        private string _nameLocalizationKey;
        
        private ILocalizationService _localizationService;
        
        private IDisposable _localizationDisposable;
        private RectTransform _canvasRect;
        private readonly Vector3[] _corners = new Vector3[4];
        
        public RectTransform Content => _content;
        public TMP_Text Header => _header;
        public Image HeaderImage => _headerImage;

        private void Awake()
        {
            _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        public void SetNameLocalizationKey(string name)
        {
            _nameLocalizationKey = name;

            _header.text = _localizationService.GetValue(_nameLocalizationKey);
        }

        public void SetColor(Color color)
        {
            _headerImage.color = color;
        }
        
        public void RegisterLocalization(ILocalizationService localizationService)
        {
            _localizationDisposable?.Dispose();
            
            _localizationService = localizationService;
            
            _localizationDisposable = Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h,
                h => _localizationService.OnLanguageChanged -= h).Subscribe(OnLocalizationChanged);
        }
        
        private void OnLocalizationChanged(string obj)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _header.text = _localizationService.GetValue(_nameLocalizationKey);
        }

        private void OnDestroy()
        {
            _localizationDisposable?.Dispose();
        }
        
        public void TryUpdateHeader()
        {
            _headerPanel.GetWorldCorners(_corners);

            var min = _corners[0];
            var max = _corners[2];
            
            if(min.x > Screen.width || max.x < 0)
                return;
            
            var size = _canvasRect.sizeDelta;
            var multiplierX = size.x / Screen.width;
            
            var minOffset = min.x < 0 ? -min.x * multiplierX : 0;
            minOffset += _padding.left;
            var maxOffset = max.x > Screen.width ? Mathf.Abs(max.x - Screen.width) * multiplierX : 0;
            maxOffset += _padding.right;
            
            var sizeDelta = _headerPanel.sizeDelta;
            var sizeOffset = sizeDelta;
            sizeOffset.x = sizeOffset.x - minOffset - maxOffset;
            
            if(sizeOffset.x < _header.preferredWidth)
                return;
            
            if(sizeDelta.x < sizeOffset.x)
                return;

            var alpha = Mathf.Clamp01((sizeOffset.x - _header.preferredWidth) / _offsetAlpha);
            _header.alpha = alpha;
            
            _headerTextRectTransform.offsetMin = new Vector2(minOffset, 0);
            _headerTextRectTransform.offsetMax = new Vector2(-maxOffset, 0);
        }
    }
}