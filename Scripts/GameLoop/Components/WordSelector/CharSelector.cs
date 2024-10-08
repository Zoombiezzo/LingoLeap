using System;
using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Client.Scripts.GameLoop.Components.WordSelector
{
    public class CharSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private RectTransform _rectTransform;
        
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private TMP_Text _charText;
        
        [SerializeField]
        private CanvasGroup _selectedCanvasGroup;
        
        [SerializeField]
        private UiAnimation _selectAnimation;
        
        [SerializeField]
        private UiAnimation _hideAnimation;
        
        [SerializeField]
        private UiAnimation _showAnimation;

        private bool _isShowed = false;
        private bool _isSelected = false;
        private string _char = string.Empty;
        
        private WordSelector _wordSelector;
        
        public bool IsShowed => _isShowed;
        public bool IsSelected => _isSelected;
        public string Char => _char;

        public Vector2 Position => _rectTransform.position;
        public Vector2 LocalPosition => _rectTransform.localPosition;

        public void RegisterWordSelector(WordSelector wordSelector)
        {
            _wordSelector = wordSelector;
        }
        
        public void SetChar(string c)
        {
            _char = c;
            _charText.text = c;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _isShowed = true;
        }
        
        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _isShowed = false;
            Deselect();
        }

        public void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }
        
        public void SetSize(Vector2 size)
        {
            _rectTransform.sizeDelta = size;
        }
        
        public void HideAnimation()
        {
            StopAllAnimations();
            _hideAnimation?.Play();
        }

        public void ShowAnimation()
        {
            StopAllAnimations();
            _showAnimation?.Play();
        }

        public void Select()
        {
            _selectedCanvasGroup.alpha = 1f;
            _isSelected = true;

            StopAllAnimations();
            _selectAnimation.Play();
        }

        public void Deselect()
        {
            _selectedCanvasGroup.alpha = 0f;
            _isSelected = false;
            
            StopAllAnimations();
            _selectAnimation?.Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnDownChar(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnUpChar(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnDragChar(this, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnBeginDragChar(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnEndDragChar(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnPointerEnterChar(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_wordSelector == null)
                return;
            
            _wordSelector.OnPointerExitChar(this, eventData);
        }

        private void StopAllAnimations()
        {
            _selectAnimation?.Stop();
            _hideAnimation?.Stop();
            _showAnimation?.Stop();
        }
    }
}