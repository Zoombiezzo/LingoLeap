using System;
using _Client.Scripts.Tools.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Buttons
{
    [Serializable]
    public class AnimationButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected bool _isInteractable = true;
        [SerializeField] private UiAnimation _animationDown;
        [SerializeField] private UiAnimation _animationUp;
        [SerializeField] private UiAnimation _animationClick;
        [SerializeField] private UiAnimation _animationHide;
        [SerializeField] private UiAnimation _animationShow;
        [SerializeField] private UiAnimation _animationEnter;
        [SerializeField] private UiAnimation _animationExit;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        protected Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();

        protected bool _isShowed = true;

        public bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }
        
        public Button.ButtonClickedEvent OnClick
        {
            get => _onClick;
            set => _onClick = value;
        }

        public RectTransform RectTransform => _rectTransform;
        
        public bool IsShowed => _isShowed;

        protected virtual void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isActiveAndEnabled == false || _isInteractable == false)
                return;

            if (_animationEnter != null)
            {
                StopAllAnimation();
                _animationEnter.Play();
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActiveAndEnabled == false || _isInteractable == false)
                return;

            if (_animationExit != null)
            {
                StopAllAnimation();
                _animationExit.Play();
            }
        }

        public virtual void Hide(bool animate = true)
        {
            if(_isShowed == false)
                return;
            
            _isShowed = false;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (_animationHide != null && animate)
            {
                StopAllAnimation();
                _animationHide.Play();
            }
            else
            {
                _canvasGroup.alpha = 0f;
            }
        }
        
        public virtual void Show(bool animate = true)
        {
            if(_isShowed)
                return;
            
            _isShowed = true;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            if (_animationShow != null && animate)
            {
                StopAllAnimation();
                _animationShow.Play();
            }
            else
            {
                _canvasGroup.alpha = 1f;
            }
        }
        protected virtual void Press()
        {
            if (isActiveAndEnabled == false || _isInteractable == false)
                return;

            _onClick.Invoke();
            
            if (_animationClick != null)
            {
                StopAllAnimation();
                _animationClick.Play();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        private void StopAllAnimation()
        {
            StopAnimation(_animationDown);
            StopAnimation(_animationUp);
            StopAnimation(_animationClick);
            StopAnimation(_animationEnter);
            StopAnimation(_animationExit);
        }

        private void StopAnimation(UiAnimation animation)
        {
            if (animation == null)
                return;
            
            if(animation.IsPlaying == false)
                return;
            
            animation.Stop();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isActiveAndEnabled == false || _isInteractable == false)
                return;

            if (_animationDown != null)
            {
                StopAllAnimation();
                _animationDown.Play();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isActiveAndEnabled == false || _isInteractable == false)
                return;

            if (_animationUp != null)
            {
                StopAllAnimation();
                _animationUp.Play();
            }
        }

        private void OnValidate()
        {
            TryGetComponent(out _rectTransform);
        }
    }
}