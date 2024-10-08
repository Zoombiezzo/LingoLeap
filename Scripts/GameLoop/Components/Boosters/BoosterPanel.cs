using _Client.Scripts.Tools.Animation;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Boosters
{
    public abstract class BoosterPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private UiAnimation _animationShow;
        [SerializeField] private UiAnimation _animationHide;
                
        protected bool _isShown = false;
        
        public bool IsShown => _isShown;
        
        public virtual void Show()
        {
            _isShown = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _animationShow.Play();
        }

        public virtual void Hide()
        {
            _isShown = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _animationHide.Play();
        }

        private void StopAllAnimation()
        {
            if (_animationShow != null)
                _animationShow.Stop();
            if (_animationHide != null)
                _animationHide.Stop();
        }
    }
}