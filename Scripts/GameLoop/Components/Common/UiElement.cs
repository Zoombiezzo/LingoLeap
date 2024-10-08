using _Client.Scripts.Tools.Animation;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Common
{
    public class UiElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _hideAnimation;
        
        private bool _isShown = false;
        
        public bool IsShown => _isShown;

        private void Awake()
        {
            _isShown = _canvasGroup.alpha > 0;
        }

        public void Show()
        {
            if (_isShown)
                return;
            
            _isShown = true;
            StopAllAnimations();
            _showAnimation.Play();
        }
        
        public void Hide()
        {
            if (_isShown == false)
                return;
            
            _isShown = false;
            StopAllAnimations();
            _hideAnimation.Play();
        }

        private void StopAllAnimations()
        {
            _showAnimation.Stop();
            _hideAnimation.Stop();
        }
    }
}