using System;
using System.Collections.Generic;
using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.WordViewer
{
    public class WordViewer : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private CharView _charViewPrefab;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private float _durationShowAnimation = 0.5f;
        [SerializeField] private UiAnimation _hideAnimation;
        [SerializeField] private float _durationHideAnimation = 0.5f;
        [SerializeField] private UiAnimation _failWordAnimation;
        [SerializeField] private float _durationFailAnimation = 0.5f;
        [SerializeField] private UiAnimation _successWordAnimation;
        [SerializeField] private float _durationSuccessAnimation = 0.5f;
        
        [SerializeField] private List<CharView> _charViews = new(8);
        [SerializeField] private List<CharView> _charViewsFree = new(8);
        
        private string _currentWord = string.Empty;
        private bool _isShowed;

        public float DurationShowAnimation => _durationShowAnimation;
        public float DurationHideAnimation => _durationHideAnimation;
        public float DurationSuccessAnimation => _durationSuccessAnimation;
        public float DurationFailAnimation => _durationFailAnimation;
        public RectTransform Container => _container;
        

        public void SetText(string text)
        {
            if(_currentWord == text)
                return;

            int index;
            for (index = 0; index < text.Length; index++)
            {
                var c = text[index];
                
                CharView currentChar = null;

                if (index >= _charViews.Count)
                {
                    currentChar = GetCharView(); 
                }
                else
                {
                    currentChar = _charViews[index];
                    if (currentChar.Char == c)
                        continue;
                }

                currentChar.transform.SetSiblingIndex(index);
                currentChar.SetChar(c);
                currentChar.Show(true);
            }

            if (index < _charViews.Count)
            {
                for (int i = index; i < _charViews.Count; i++)
                {
                    var charView = _charViews[i];
                    charView.Hide();
                    charView.Clear();
                    _charViewsFree.Add(charView);
                }
                
                _charViews.RemoveRange(index, _charViews.Count - index);
            }

            _currentWord = text;
        }

        public void Show()
        {
            if (_isShowed)
                return;
            
            _isShowed = true;
            
            StopAllAnimations();
            _showAnimation?.Play();
        }
        
        public void Hide()
        {
            if (_isShowed == false)
                return;

            _isShowed = false;
            
            StopAllAnimations();
            _hideAnimation?.Play();
        }

        public void PlayFailWordAnimation()
        {
            StopAllAnimations();
            _failWordAnimation?.Play();
        }
        
        public void PlaySuccessWordAnimation()
        {
            StopAllAnimations();
            _successWordAnimation?.Play();
        }

        private CharView GetCharView()
        {
            CharView charView = null;
            
            if (_charViewsFree.Count > 0)
            {
                charView = _charViewsFree[^1];
                _charViewsFree.RemoveAt(_charViewsFree.Count - 1);
            }
            else
            {
                charView = Instantiate(_charViewPrefab, _container);
            }

            _charViews.Add(charView);
            
            return charView;
        }

        public void ClearWord()
        {
            foreach (var charView in _charViews)
            {
                charView.Clear();
                charView.Hide();
                _charViewsFree.Add(charView);
            } 
            
            _charViews.Clear();
            _currentWord = string.Empty;
        }

        private void StopAllAnimations()
        {
            _showAnimation?.Stop();
            _hideAnimation?.Stop();
            _failWordAnimation?.Stop();
            _successWordAnimation?.Stop();
        }

        private void OnDestroy()
        {
            StopAllAnimations();
        }
    }
}