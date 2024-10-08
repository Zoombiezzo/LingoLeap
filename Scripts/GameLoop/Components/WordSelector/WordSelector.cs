using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Client.Scripts.GameLoop.Components.WordSelector
{
    public abstract class WordSelector : MonoBehaviour
    {
        [SerializeField] protected RectTransform _parent;
        [SerializeField] protected CharSelector _charSelectorPrefab;

        protected Stack<CharSelector> _freeChars = new(8);
        protected List<CharSelector> _charsUsed = new(8);
        
        protected bool _isBlocked = false;
        
        public virtual string SelectedWord => string.Empty;
        public virtual char[] Chars => Array.Empty<char>();
        
        public virtual event Action<WordSelector> OnCharsChanged;
        public virtual event Action<WordSelector> OnWordSelected;
        public virtual event Action<WordSelector> OnShuffle;

        public virtual void SetChars(params char[] chars)
        {
        }

        public virtual void SetChars(string chars)
        {
        }

        public virtual void OverrideChars(params char[] chars)
        {
        }

        public virtual void Show(bool animate = false)
        {
            
        }
        
        public virtual void Hide(bool animate = false)
        {
            
        }

        public virtual void Block(bool block)
        {
            _isBlocked = block;
        }

        public virtual void ShowEffect()
        {
            
        }

        internal virtual void OnDownChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnUpChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnBeginDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnEndDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnPointerEnterChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }
        
        internal virtual void OnPointerExitChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }

#if UNITY_EDITOR
        [SerializeField] private string _testChars;

        [Button]
        private void CreateTestChars()
        {
            foreach (var charSelector in _charsUsed)
            {
                if(charSelector != null)
                    DestroyImmediate(charSelector.gameObject);
            }
            
            _freeChars.Clear();
            _charsUsed.Clear();
            
            SetChars(_testChars.ToCharArray());
        }
#endif
    }
}