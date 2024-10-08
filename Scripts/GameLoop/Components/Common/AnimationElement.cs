using _Client.Scripts.Tools.Animation;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Common
{
    public class AnimationElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _hideAnimation;
        [SerializeField] private float _durationShow;
        [SerializeField] private float _durationHide;
        
        public float DurationShow => _durationShow;
        public float DurationHide => _durationHide;
        public UiAnimation ShowAnimation => _showAnimation;
        public UiAnimation HideAnimation => _hideAnimation;
        
        
        public void Show(bool animate = false)
        {
            _canvasGroup.alpha = 1;
            if (animate)
            {
                _hideAnimation.Stop();
                _showAnimation.Play();
            }
        }
        
        public void Hide(bool animate = false)
        {
            _canvasGroup.alpha = 0;
            if (animate)
            {
                _showAnimation.Stop();
                _hideAnimation.Play();
            }
        }
    }
}