using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [ExecuteAlways]
    public class AdaptiveUIGroup : UIBehaviour
    {
        [SerializeField]
        private List<AdaptiveUIElement> _adaptiveUIElements = new List<AdaptiveUIElement>();
        [SerializeField] [ReadOnly]
        private AdaptiveUIOrientation _currentOrientation = AdaptiveUIOrientation.None;
        [SerializeField] [ReadOnly]
        private Vector2 _currentSize = Vector2.zero;

        private Coroutine _coroutine;
        private float _lastUpdatedTime = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            _currentOrientation = AdaptiveUIOrientation.None;
            _currentSize = Vector2.zero;
        }
        
        protected override void Start()
        {
            base.Start();
            TryChangeOrientation();
            TryChangeSize();
        }

        public void UpdateGroups(bool force = false, bool coroutine = true)
        {
            StopUpdateCoroutine();
            
            if (coroutine)
            {
                if (IsActive() == false)
                    return;
                
                if(gameObject.activeSelf == false)
                    return;
                
                _coroutine = StartCoroutine(UpdateGroupsCoroutine(force));
                return;
            }
            
            UpdateOrientation();
            UpdateSize();
        }

        internal void TryRegister(AdaptiveUIElement element)
        {
            if (_adaptiveUIElements.Contains(element))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[AdaptiveUIGroup] Element already registered: {element.name}");
#endif
                return;
            }


            _adaptiveUIElements.Add(element);

            _currentOrientation = CalculateOrientation();
            element.ChangeOrientation(_currentOrientation, true);
            _currentSize = new Vector2(Screen.width, Screen.height);
            element.ChangeSize(_currentSize, true);
        }

        private IEnumerator UpdateGroupsCoroutine(bool force = false)
        {
            yield return UpdateOrientationCoroutine(force);
            yield return UpdateSizeCoroutine(force);
        }

        private void StopUpdateCoroutine()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            
            if (!IsActive())
                return;
            
            if(Application.isPlaying == false)
                return;

            UpdateGroups();

            var currentTime = Time.time;
            if (currentTime - _lastUpdatedTime < 0.5f)
            {
                UpdateGroups(true, false);
            }
            
            _lastUpdatedTime = currentTime;
        }
        
        private void TryChangeOrientation()
        {
            var newOrientation = CalculateOrientation();
            
            if (_currentOrientation == newOrientation)
                return;
            
            _currentOrientation = newOrientation;

            UpdateOrientation();
        }

        private void UpdateOrientation(bool force = false)
        {
            _currentOrientation = CalculateOrientation();

            for (var index = 0; index < _adaptiveUIElements.Count; index++)
            {
                var element = _adaptiveUIElements[index];
                element.ChangeOrientation(_currentOrientation, force);
            }
        }
        
        private IEnumerator UpdateOrientationCoroutine(bool force = false)
        {
            _currentOrientation = CalculateOrientation();

            for (var index = 0; index < _adaptiveUIElements.Count; index++)
            {
                var element = _adaptiveUIElements[index];
                element.ChangeOrientation(_currentOrientation, force);
                yield return null;
            }
        }

        private AdaptiveUIOrientation CalculateOrientation()
        {
            ScreenOrientation orientation = Screen.orientation;

            var newOrientation = AdaptiveUIOrientation.None;
            
            var sizeScreen = new Vector2(Screen.width, Screen.height);

            if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown ||
                sizeScreen.x <= sizeScreen.y)
                newOrientation = AdaptiveUIOrientation.Vertical;

            if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight ||
                sizeScreen.x > sizeScreen.y)
                newOrientation = AdaptiveUIOrientation.Horizontal;


            return newOrientation;
        }

        private void TryChangeSize()
        {
            var newSize = new Vector2(Screen.width, Screen.height);
            
            if (_currentSize == newSize)
                return;
            
            _currentSize = newSize;

            UpdateSize();
        }

        private void UpdateSize(bool force = false)
        {
            _currentSize = new Vector2(Screen.width, Screen.height);

            for (var index = 0; index < _adaptiveUIElements.Count; index++)
            {
                var element = _adaptiveUIElements[index];
                element.ChangeSize(_currentSize, force);
            }
        }
        
        private IEnumerator UpdateSizeCoroutine(bool force = false)
        {
            _currentSize = new Vector2(Screen.width, Screen.height);

            for (var index = 0; index < _adaptiveUIElements.Count; index++)
            {
                var element = _adaptiveUIElements[index];
                element.ChangeSize(_currentSize, force);
                yield return null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopUpdateCoroutine();
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool _updateInEditMode = true;
        
        [SerializeField]
        private bool _editMode = true;
        
        protected void Update()
        {
            if (!IsActive())
                return;
            
            if(Application.isPlaying)
                return;

            if (_updateInEditMode == false)
            {
                _currentOrientation = CalculateOrientation();
            }

            if (_updateInEditMode)
            {
                TryChangeOrientation();
                TryChangeSize();
            }
            
            if (_editMode)
            {
                TryEditElements();
            }
        }

        private void TryEditElements()
        {
            foreach (var element in _adaptiveUIElements)
            {
                element.TryEdit(_currentOrientation);
            }
        }

        [Button]
        private void CollectAllUIElements()
        {
            _adaptiveUIElements.Clear();
            var elements = GetComponentsInChildren<AdaptiveUIElement>();
            foreach (var element in elements)
            {
                _adaptiveUIElements.Add(element);
            }
        }

        [Button]
        private void SavePreset()
        {
            foreach (var element in _adaptiveUIElements)
            {
                element.SavePreset(_currentOrientation);
            }
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            for (var index = 0; index < _adaptiveUIElements.Count; index++)
            {
                var element = _adaptiveUIElements[index];
                if (element == null)
                {
                    _adaptiveUIElements.RemoveAt(index);
                    index--;
                }
            }
        }

#endif
    }
}