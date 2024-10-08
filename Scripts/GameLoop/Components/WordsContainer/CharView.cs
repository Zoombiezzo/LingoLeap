using System;
using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.WordsContainer
{
    [RequireComponent(typeof(LayoutElement), typeof(CanvasGroup))]
    public class CharView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _canvasGroupBackground;
        [SerializeField] private CanvasGroup _canvasGroupChar;
        [SerializeField] private CanvasGroup _canvasGroupBackgroundShowed;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _pumpAnimation;
        [SerializeField] private UiAnimation _showCellAnimation;
        
        private CharViewState _state = CharViewState.Hidden;
        public CharViewState State => _state;
        
        public RectTransform RectTransform => _rectTransform;
        
        public void SerChar(string text)
        {
            _text.text = text;
        }
        
        public void Enable()
        {
            _layoutElement.ignoreLayout = false;
            _canvasGroup.alpha = 1f;
        }
        
        public void Disable()
        {
            _layoutElement.ignoreLayout = true;
            _canvasGroup.alpha = 0f;
        }

        public void SetShowState()
        {
            _state = CharViewState.Shown;
        }

        public void SetState(CharViewState state)
        {
            _state = state;
        }
        
        public void ShowChar(bool animate = true)
        {
            StopAllAnimations();
            
            _canvasGroupChar.alpha = 1f;
            _canvasGroupBackgroundShowed.alpha = 1f;
            
            if (animate)
                _showAnimation?.Play();
            
            _canvasGroupBackground.alpha = 0f;
        }

        public void ShowCellAnimation()
        {
            StopAllAnimations();
            _showCellAnimation?.Play();
        }

        public void ShowCharAnimation() => ShowChar(true);
        public void PumpCharAnimation()
        {
            StopAllAnimations();
            _pumpAnimation?.Play();
        }

        public void HideChar()
        {
            _canvasGroupBackgroundShowed.alpha = 0f;
            _canvasGroupChar.alpha = 0f;
            _canvasGroupBackground.alpha = 1f;
            _state = CharViewState.Hidden;
        }

        private void Reset()
        { 
            TryGetComponent(out _layoutElement);
            TryGetComponent(out _canvasGroup);
        }

        private void StopAllAnimations()
        {
            _showAnimation?.Stop();
            _pumpAnimation?.Stop();
            _showCellAnimation?.Stop();
        }
        
        private void OnDestroy()
        {
            StopAllAnimations();
        }
    }
}