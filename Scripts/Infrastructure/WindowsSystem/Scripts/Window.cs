using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Scripts
{
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))] 
    [DisallowMultipleComponent]
    public class Window : MonoBehaviour, IWindow
    {
        [SerializeField]
        private string _id;
        
        [SerializeField]
        private bool _isShowOnStart;

        [SerializeField]
        private bool _isStaticOrder;

        [SerializeField]
        private int _preferredOrder;
        
        [SerializeField]
        private bool _disablePrevousWindowsRendering;
        
        [SerializeField]
        private bool _ignoreDisableRendering;
        
        [SerializeField]
        private bool _noBlockRaycastAndInteract;

        [SerializeField]
        private AnimationWindow _openAnimation;
        
        [SerializeField]
        private AnimationWindow _closeAnimation;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        private Coroutine _animationOpenCoroutine;
        private Coroutine _animationCloseCoroutine;
        
        private bool _isShow;

        public string Id => _id;
        public int PreferredOrder => _preferredOrder;
        public int SortingOrder => _canvas.sortingOrder;
        public bool IsStaticOrder => _isStaticOrder;
        public bool DisablePreviousWindowsRendering => _disablePrevousWindowsRendering;
        public bool IgnoreDisableRendering => _ignoreDisableRendering;
        
        public Action OnInitialize { get; set; }
        public Action OnBeforeShow { get; set; }
        public Action OnShow { get; set; }
        public Action OnBeforeHide { get; set; }
        public Action OnHide { get; set; }

        public virtual void Initialize()
        {
            OnInitialize?.Invoke();
        }

        public virtual void Show()
        {
            WindowsService.Show(_id);
        }

        public virtual void Hide()
        {
            WindowsService.Hide(_id);
        }

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas.enabled = false;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
            WindowsService.Register(this);
        }

        private void OnDestroy()
        {
            WindowsService.Unregister(this);
        }
        
        protected virtual void Start()
        {
            if (_isShowOnStart)
            {
                Show();
            }
            else
            {
                OnHidden();
            }
        }

        public bool IsShow() => _isShow;
        internal bool IsShowing() => _animationOpenCoroutine != null;
        internal bool IsHiding() => _animationCloseCoroutine != null;

         internal void ShowInternal(int order = -1)
         {
             OnBeforeShown();
             OnBeforeShow?.Invoke();
             
             _canvas.sortingOrder = order;
             _canvas.enabled = true;
             
             if (_openAnimation != null)
             {
                 StartAnimationOpen();
                 return;
             }

             _isShow = true;
             _canvasGroup.alpha = 1;
             _canvasGroup.interactable = _noBlockRaycastAndInteract == false;
             _canvasGroup.blocksRaycasts = _noBlockRaycastAndInteract == false;
             OnShow?.Invoke();
             OnShown();
         }

         internal void StartAnimationOpen()
         {
             StopAnimationOpen();

             if (_openAnimation != null)
             {
                 _openAnimation.IsFinished = false;
                 _openAnimation.Play();
                 _openAnimation.IsPlayed = true;
             }

             _animationOpenCoroutine = StartCoroutine(AnimationOpen());
         }

         internal void StopAnimationOpen()
         {
             if (_openAnimation != null && _openAnimation.IsPlayed)
             {
                 _openAnimation.Stop();
                 _openAnimation.IsPlayed = false;
                 _openAnimation.IsFinished = false;
             }
             
             if (_animationOpenCoroutine != null)
             {
                 StopCoroutine(_animationOpenCoroutine);
                 _animationOpenCoroutine = null;
             }
         }

         internal IEnumerator AnimationOpen()
         {
             _canvasGroup.interactable = false;
             _canvasGroup.blocksRaycasts = _noBlockRaycastAndInteract == false;
             
             while (_openAnimation.IsPlayed && _openAnimation.IsFinished == false)
             {
                 yield return null;
             }
             
             _canvasGroup.interactable = _noBlockRaycastAndInteract == false;
             _canvasGroup.blocksRaycasts = _noBlockRaycastAndInteract == false;

             _isShow = true;
             OnShow?.Invoke();
             OnShown();
             StopAnimationOpen();
         }

         internal void HideInternal(Action onHide)
         {
             OnBeforeHidden();
             OnBeforeHide?.Invoke();
             
             if (_closeAnimation != null)
             {
                 StartAnimationClose(onHide);
                 return;
             }
             
             _canvas.enabled = false;
             _canvasGroup.alpha = 0;
             _canvasGroup.interactable = false;
             _canvasGroup.blocksRaycasts = false;
             
             _isShow = false;
             onHide?.Invoke();
             OnHide?.Invoke();
             OnHidden();
         }
         
         internal void StartAnimationClose(Action onHide)
         {
             StopAnimationClose();

             if (_closeAnimation != null)
             {
                 _closeAnimation.IsFinished = false;
                 _closeAnimation.Play();
                 _closeAnimation.IsPlayed = true;
             }

             _animationCloseCoroutine = StartCoroutine(AnimationClose(onHide));
         }

         internal void StopAnimationClose()
         {
             if (_closeAnimation != null && _closeAnimation.IsPlayed)
             {
                 _closeAnimation.Stop();
                 _closeAnimation.IsPlayed = false;
                 _closeAnimation.IsFinished = false;
             }
             
             if (_animationCloseCoroutine != null)
             {
                 StopCoroutine(_animationCloseCoroutine);
                 _animationCloseCoroutine = null;
             }
         }

         internal IEnumerator AnimationClose(Action onHide)
         {
             while (_closeAnimation.IsPlayed && _closeAnimation.IsFinished == false)
             {
                 yield return null;
             }

             _canvasGroup.interactable = false;
             _canvasGroup.blocksRaycasts = false;
             _canvas.enabled = false;
             
             _isShow = false;
             onHide?.Invoke();
             OnHide?.Invoke();
             OnHidden();
             StopAnimationClose();
         }

         internal void SetSortingOrder(int order)
         {
             _canvas.sortingOrder = order;
         }
         
         internal void DisableRendering()
         {
             _canvas.enabled = false;
             _canvasGroup.alpha = 0;
         }
         
         internal void EnableRendering()
         {
             _canvas.enabled = true;
             _canvasGroup.alpha = 1;
         }

         protected virtual void OnShown()
         {
             
         }

         protected virtual void OnHidden()
         {
             
         }
         
         protected virtual void OnBeforeShown()
         {
             
         }
         
         protected virtual void OnBeforeHidden()
         {
             
         }
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_canvas != null)
                _canvas = GetComponent<Canvas>();
            if (_canvasGroup != null)
                _canvasGroup = GetComponent<CanvasGroup>();
            
            _preferredOrder = Mathf.Min(_preferredOrder, WindowsService.MaxOrder);
        }

        [UnityEngine.ContextMenu( "Window/Update Links" )]
        private void UpdateLinks()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        [UnityEngine.ContextMenu( "Window/Debug/Show", true )]
        private bool DebugShowValidate()
        {
            return UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying;
        }
        
        [UnityEngine.ContextMenu( "Window/Show" )]
        private void ShowEditor()
        {
            Show();
        }
        
        [UnityEngine.ContextMenu( "Window/Debug/Show", true )]
        private bool DebugHideValidate()
        {
            return UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying;
        }
        
        [UnityEngine.ContextMenu( "Window/Hide" )]
        private void HideEditor()
        {
            Hide();
        }

        private void Reset()
        {
            UpdateLinks();

            if (string.IsNullOrEmpty(_id)) _id = GetType().Name;
        }
#endif
    }
}
