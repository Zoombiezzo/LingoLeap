using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using GridLayoutGroup = _Client.Scripts.Tools.GridLayoutGroup;

namespace _Client.Scripts.GameLoop.Components.WordsContainer
{
    [RequireComponent(typeof(LayoutElement), typeof(CanvasGroup))]
    public class WordView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CharView _charViewPrefab;
        [SerializeField] private List<CharView> _charViews = new(4);
        [SerializeField] private List<CharView> _charViewsFree = new(4);
        [SerializeField] private float _delayCharsShown = 0.1f;

        private Dictionary<string, List<CharView>> _charViewsMap = new(16);
        
        private Dictionary<CharViewState, List<CharView>> _charViewsStates = new(16);
        
        [SerializeField] private WordViewState _state = WordViewState.Hidden;

        private List<CharView> _charsToAnimate = new(8);
        
        private string _currentWord;

        private Sequence _showWordAnimation;
        private Sequence _pumpCharsAnimation;
        public WordViewState State => _state;
        public string Word => _currentWord;
        public IReadOnlyList<CharView> CharViews => _charViews;
        
        public void SetWord(string word)
        {
            _currentWord = word;
            
            for (var index = 0; index < word.Length; index++)
            {
                var c = word[index];
                CharView charView;
                if (_charViewsFree.Count > 0)
                {
                    charView = _charViewsFree[^1];
                    _charViewsFree.RemoveAt(_charViewsFree.Count - 1);
                }
                else
                {
                    charView = Instantiate(_charViewPrefab, _gridLayoutGroup.transform);
                }

                var charStr = c.ToString();

                charView.SerChar(charStr);
                charView.transform.SetSiblingIndex(index);
                charView.Enable();
                charView.HideChar();
                
                _charViews.Add(charView);

                if (_charViewsMap.TryGetValue(charStr, out var list) == false)
                {
                    list = new List<CharView>();
                    _charViewsMap.Add(charStr, list);
                }

                list.Add(charView);

                if (_charViewsStates.TryGetValue(charView.State, out list) == false)
                {
                    list = new List<CharView>();
                    _charViewsStates.Add(charView.State, list);
                }
                
                list.Add(charView);
            }

            InitializePumpAllCharsAnimation();
        }

        public void ShowChar(string c, bool allEqual = true)
        {
            if (_charViewsMap.TryGetValue(c, out var list) == false)
            {
                return;
            }
            
            foreach (var charView in list)
            {
                if (charView.State == CharViewState.Hidden)
                {
                    SetShowCharState(charView);
                    charView.ShowChar();

                    if (allEqual == false)
                        break;
                }
            }
        }

        public void ShowChar(int index)
        {
            if (index < 0 || _charViews.Count <= index) return;

            var charView = _charViews[index];
            if (charView.State == CharViewState.Hidden)
            {
                SetShowCharState(charView);
                charView.ShowChar();
            }
        }

        public void HideChar(string c, bool allEqual = true)
        {
            if (_charViewsMap.TryGetValue(c, out var list) == false)
            {
                return;
            }
            
            foreach (var charView in list)
            {
                if (charView.State == CharViewState.Shown)
                {
                    HideChar(charView);
                    
                    if (allEqual == false)
                        break;
                }
            }
        }
        
        public void HideChar(int index)
        {
            if (index < 0 || _charViews.Count <= index) return;
            
            var charView = _charViews[index];
            if (charView.State == CharViewState.Shown)
            {
                HideChar(charView);
            }
        }
        
        public void ShowAllChars(bool animate = false)
        {
            _charsToAnimate.Clear();
            
            foreach (var charView in _charViews)
            {
                if (charView.State == CharViewState.Hidden)
                {
                    SetShowCharState(charView);

                    if (animate)
                    {
                        _charsToAnimate.Add(charView);
                    }
                    else
                    {
                        charView.ShowChar(false);
                    }
                }
            }

            if (animate)
            {
                _showWordAnimation = DOTween.Sequence();

                foreach (var charView in _charsToAnimate)
                {
                    _showWordAnimation.AppendCallback(charView.ShowCharAnimation);
                    _showWordAnimation.AppendInterval(_delayCharsShown);
                }
            }
        }

        public void PumpAllChars()
        {
            _pumpCharsAnimation.Restart();
            _pumpCharsAnimation.Play();
        }

        public void ShowAllCellsAnimation()
        {
            foreach (var charView in _charViews)
            {
                charView.ShowCellAnimation();
            }
        }
        
        public void HideAllChars()
        {
            foreach (var charView in _charViews)
            {
                if (charView.State == CharViewState.Shown)
                {
                    HideChar(charView);
                }
            }
        }
        
        public void ClearWord()
        {
            _charViewsStates.Clear();
            _charViewsMap.Clear();
            
            foreach (var charView in _charViews)
            {
                charView.Disable();
                _charViewsFree.Add(charView);
            }
            
            _charViews.Clear();
        }
        
        public void SetSize(Vector2 size)
        {
            _gridLayoutGroup.cellSize = size;
        }
        
        public void SetSpacing(Vector2 spacing)
        {
            _gridLayoutGroup.spacing = spacing;
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
        
        private void SetShowCharState(CharView charView)
        {
            var state = charView.State;

            if (_charViewsStates.TryGetValue(state, out var list))
            {
                list.Remove(charView);
            }
            
            state = CharViewState.Shown;
            
            if (_charViewsStates.TryGetValue(state, out list) == false)
            {
                list = new List<CharView>();
                _charViewsStates.Add(state, list);
            }
            
            list.Add(charView);

            TryChangeState();
            
            charView.SetShowState();
        }

        public void SetCharState(int index, CharViewState newState)
        {
            if (index < 0 || _charViews.Count <= index) return;
            
            var charView = _charViews[index];
            
            var state = charView.State;

            if (_charViewsStates.TryGetValue(state, out var list))
            {
                list.Remove(charView);
            }
            
            state = newState;
            
            if (_charViewsStates.TryGetValue(state, out list) == false)
            {
                list = new List<CharView>();
                _charViewsStates.Add(state, list);
            }
            
            list.Add(charView);

            TryChangeState();
            
            charView.SetState(newState);
        }

        private void TryChangeState()
        {
            var state = WordViewState.Hidden;

            if (_charViewsStates.TryGetValue(CharViewState.Hidden, out var list))
            {
                var count = list.Count;
                var diff = count - _currentWord.Length;

                state = count == 0
                    ? WordViewState.Shown
                    : diff switch
                    {
                        < 0 => WordViewState.SemiShown,
                        0 => WordViewState.Hidden,
                        _ => state
                    };
            }

            _state = state;
        }

        private void HideChar(CharView charView)
        {
            var state = charView.State;
            
            if (_charViewsStates.TryGetValue(state, out var list))
            {
                list.Remove(charView);
            }
            
            charView.HideChar();
            
            state = charView.State;
            
            if (_charViewsStates.TryGetValue(state, out list) == false)
            {
                list = new List<CharView>();
                _charViewsStates.Add(state, list);
            }
            
            list.Add(charView);
            
            TryChangeState();
        }
        
        private void Reset()
        { 
            TryGetComponent(out _layoutElement);
            TryGetComponent(out _canvasGroup);
        }

        private void OnDestroy()
        {
            if(_showWordAnimation != null)
                _showWordAnimation.Kill();
            
            if(_pumpCharsAnimation != null)
                _pumpCharsAnimation.Kill();
        }

        private void InitializePumpAllCharsAnimation()
        {
            _pumpCharsAnimation = DOTween.Sequence();
            _pumpCharsAnimation.SetAutoKill(false);
            _pumpCharsAnimation.Pause();
            
            foreach (var charView in _charViews)
            {
                _pumpCharsAnimation.AppendCallback(charView.PumpCharAnimation);
            }
        }
    }
}