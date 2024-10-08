using System;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Progressbar.StageProgressbar
{
    public class StageElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroupShown;
        [SerializeField] private bool _isShown;

        public bool IsShown => _isShown;

        private void Awake()
        {
            _canvasGroupShown.alpha = 0f;
            _isShown = false;
        }

        public virtual void Show(bool animate = false)
        {
            if(_isShown)
                return;
            
            _isShown = true;

            _canvasGroupShown.alpha = 1f;
        }

        public virtual void Hide(bool animate = false)
        {
            if(_isShown == false)
                return;
            
            _isShown = false;
            
            _canvasGroupShown.alpha = 0f;
        }
    }
}