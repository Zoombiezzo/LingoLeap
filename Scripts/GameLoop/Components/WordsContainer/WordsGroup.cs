using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.WordsContainer
{
    [RequireComponent(typeof(LayoutElement), typeof(CanvasGroup))]
    public class WordsGroup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private VerticalLayoutGroup _layoutGroup;
        [SerializeField] private WordView _wordViewPrefab;
        
        [SerializeField] private List<WordView> _wordViews = new(4);
        [SerializeField] private List<WordView> _wordViewsFree = new(4);

        private Dictionary<string, WordView> _wordsViewsMap = new(16);
        
        private int _maxSizeWord = 0;
        
        public int MaxSizeWord => _maxSizeWord;
        public int WordsCount => _wordViews.Count;
        
        public IReadOnlyList<WordView> WordViews => _wordViews;

        public WordView AddWord(string word)
        {
            WordView wordView;
            if (_wordViewsFree.Count > 0)
            {
                wordView = _wordViewsFree[^1];
                _wordViewsFree.RemoveAt(_wordViewsFree.Count - 1);
            }
            else
            {
                wordView = Instantiate(_wordViewPrefab, _layoutGroup.transform);
            }

            wordView.SetWord(word);
            wordView.Enable();
            
            _wordViews.Add(wordView);

            _wordsViewsMap.Add(word, wordView);
            
            var length = word.Length;
            if(_maxSizeWord < length)
                _maxSizeWord = length;
            
            return wordView;
        }
        
        
        public void SetCharSize(Vector2 size)
        {
            foreach (var wordView in _wordViews)
            {
                wordView.SetSize(size);
            }
        }
        
        public void SetCharSpacing(Vector2 spacing)
        {
            foreach (var wordView in _wordViews)
            {
                wordView.SetSpacing(spacing);
            }
        }
        public void SetWordsSpacing(float spacing)
        {
            _layoutGroup.spacing = spacing;
        }

        public void ClearWords()
        {
            _maxSizeWord = 0;
            _wordsViewsMap.Clear();
            
            foreach (var wordView in _wordViews)
            {
                wordView.ClearWord();
                wordView.Disable();
                _wordViewsFree.Add(wordView);
            }
            
            _wordViews.Clear();
        }
        
        public void Enable()
        {
            _layoutElement.ignoreLayout = false;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
        
        public void Disable()
        {
            _layoutElement.ignoreLayout = true;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        public void PumpWord(string word)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.PumpAllChars();
        }
        
        public void ShowWord(string word, bool animate = false)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.ShowAllChars(animate);
        }

        public void HideWord(string word)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.HideAllChars();
        }

        public void ShowChar(string word, string c)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.ShowChar(c);
        }
        
        public void SetCharState(string word, int index, CharViewState state)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.SetCharState(index, state);
        }
        
        public void ShowChar(string word, int c)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.ShowChar(c);
        }
        
        public void HideChar(string word, string c)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.HideChar(c);
        }

        public void HideChar(string word, int c)
        {
            if (_wordsViewsMap.TryGetValue(word, out var wordView) == false) return;
            wordView.HideChar(c);
        }

        public void ShowCellsAnimation()
        {
            foreach (var wordView in _wordViews)
            {
                wordView.ShowAllCellsAnimation();
            }
        }

        private void Reset()
        { 
            TryGetComponent(out _layoutElement);
            TryGetComponent(out _canvasGroup);
        }
    }
}