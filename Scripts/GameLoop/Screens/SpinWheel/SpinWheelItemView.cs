using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class SpinWheelItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _imageShadow;
        [SerializeField] private CanvasGroup _textCanvasGroup;
        [SerializeField] private TMP_Text _text;

        [SerializeField] [FoldoutGroup("ANIMATIONS")]
        private UiAnimation _collectAnimation;
        
        [SerializeField] [FoldoutGroup("ANIMATIONS")]
        private UiAnimation _showAnimation;

        private Sequence _currentAnimation;
        
        public Image Image => _image;
        public Image ImageShadow => _imageShadow;
        public CanvasGroup TextCanvasGroup => _textCanvasGroup;
        public TMP_Text Text => _text;

        public Sequence PlayCollectAnimation()
        {
            if (_currentAnimation != null && _currentAnimation.IsPlaying() && _currentAnimation.IsActive())
            {
                _currentAnimation.Kill();
            }

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_collectAnimation.Play());

            return _currentAnimation;
        }
        
        public Sequence PlayShowAnimation()
        {
            if (_currentAnimation != null && _currentAnimation.IsPlaying() && _currentAnimation.IsActive())
            {
                _currentAnimation.Kill();
            }

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_showAnimation.Play());

            return _currentAnimation;
        }
    }
}