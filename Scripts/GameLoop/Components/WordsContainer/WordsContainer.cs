using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.WordsContainer
{
    public class WordsContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private WordsGroup _wordGroupPrefab;
        [SerializeField] private float _charMaxSize = 80;
        [SerializeField] private float _charsSpacing = 5;
        [SerializeField] private float _wordsSpacing = 10;
        [SerializeField] private float _groupsSpacing = 20;
        
        [SerializeField] private List<WordsGroup> _wordsGroups = new(2);
        
        [SerializeField]
        private List<WordsGroup> _wordsGroupsFree = new(2);
        [SerializeField] [ReadOnly] private Vector2 _preferredSize;
        
        private Dictionary<string, WordsGroup> _wordsGroupsMap = new(16);
        private Dictionary<int, string> _indexWords = new(16);
        private Dictionary<string, int> _indexWordsReverse = new(16);
        
        private Coroutine _recalculateSizeCoroutine;
        private int _minSizeWord = 0;
        private int _maxSizeWord = 0;

        private List<WordView> _wordsCache = new(16);
        
        public int MinSizeWord => _minSizeWord;
        public int MaxSizeWord => _maxSizeWord;
        
        public event Action OnSizeChanged;

        public void CreateWords(string[] words, int columns)
        {
            words = words.OrderBy(el => el.Length).ToArray();

            if (words.Length > 0)
            {
                _minSizeWord = words[0].Length;
                _maxSizeWord = words[^1].Length;
            }
            
            ClearWords();

            columns = Mathf.Min(words.Length, columns);
            var countInColumn = Mathf.CeilToInt(words.Length * 1.0f / columns);
            columns = Mathf.CeilToInt(words.Length * 1.0f / countInColumn);
            
            var groups = GetOrCreateWordsGroup(columns);
            
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                var column = i / countInColumn;
                var wordsGroup = groups[column];
                var wordView = wordsGroup.AddWord(word);
                wordView.transform.SetSiblingIndex(i);
                
                _wordsGroupsMap.Add(word, wordsGroup);
                _indexWordsReverse.Add(word, i);
                _indexWords.Add(i, word);
            }

            var size = new Vector2(0, 0);

            foreach (var wordsGroup in _wordsGroups)
            {
                size.x += wordsGroup.MaxSizeWord;

                if (size.y < wordsGroup.WordsCount)
                    size.y = wordsGroup.WordsCount;
            }

            _preferredSize = new Vector2(size.x * _charMaxSize, size.y * _charMaxSize);
            _preferredSize += new Vector2(_charsSpacing * (size.x - 1 * _wordsGroups.Count) + _groupsSpacing * (_wordsGroups.Count - 1), 0);
            _preferredSize += new Vector2(0, _wordsSpacing * (size.y - 1));

            RecalculateSizes();
        }

        public IReadOnlyList<WordView> GetWords()
        {
            _wordsCache.Clear();
            
            foreach (var wordsGroup in _wordsGroups)
            {
                _wordsCache.AddRange(wordsGroup.WordViews);
            }
            
            return _wordsCache;
        }

        public bool ContainsWord(string word) => _wordsGroupsMap.ContainsKey(word);
        
        public bool TryGetWordIndex(string word, out int index) => _indexWordsReverse.TryGetValue(word, out index);

        public void ShowCellsAnimation()
        {
            foreach (var wordsGroup in _wordsGroups)
            {
                wordsGroup.ShowCellsAnimation();
            }
        }

        public void PumpWord(string word)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.PumpWord(word);
        }

        public void ShowWord(string word, bool animate = false)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.ShowWord(word, animate);
        }

        public void ShowWord(int index)
        {
            if (_indexWords.TryGetValue(index, out var word) == false)
                return;
            
            ShowWord(word);
        }

        public void TryShowChar(string word, string c)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.ShowChar(word, c);
        }

        public void SetCharState(string word, int index, CharViewState state)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.SetCharState(word, index, state);
        }
        
        public void ShowChar(int wordIndex, int index)
        {
            if (_indexWords.TryGetValue(wordIndex, out var word) == false)
                return;
            
            TryShowChar(word, index);
        }
        public void ShowChar(string word, int index)
        {
            TryShowChar(word, index);
        }

        public void TryShowChar(string word, int index)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.ShowChar(word, index);
        }
        
        public void HideChar(string word, string c)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.HideChar(word, c);
        }
        
        public void HideWord(string word)
        {
            if (_wordsGroupsMap.TryGetValue(word, out var wordsGroup) == false)
                return;
            
            wordsGroup.HideWord(word);
        }
        
        public void HideWord(int index)
        {
            if (_indexWords.TryGetValue(index, out var word) == false)
                return;
            
            HideWord(word);
        }
        
        public void RecalculateSizes()
        {
            _container.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
            
            var ratio = _container.rect.size / _preferredSize;
            var multiplier = Mathf.Min(Mathf.Min(ratio.x, ratio.y), 1f);
            
            for (var index = 0; index < _wordsGroups.Count; index++)
            {
                var wordsGroup = _wordsGroups[index];
                wordsGroup.SetCharSize(Vector2.one * _charMaxSize * multiplier);
                wordsGroup.SetCharSpacing(Vector2.one * _charsSpacing * multiplier);
                wordsGroup.SetWordsSpacing(_wordsSpacing * multiplier);
                wordsGroup.transform.SetSiblingIndex(index);
                wordsGroup.Enable();
            }
            
            _layoutGroup.spacing = _groupsSpacing * multiplier;
            
            OnSizeChanged?.Invoke();
        }

        public void RecalculateSizeCoroutine()
        {
            StopUpdateCoroutine();
            
            if(isActiveAndEnabled == false)
                return;
            
            _recalculateSizeCoroutine = StartCoroutine(UpdateSizeCoroutine());
        }
        
        private void StopUpdateCoroutine()
        {
            if (_recalculateSizeCoroutine != null)
            {
                StopCoroutine(_recalculateSizeCoroutine);
                _recalculateSizeCoroutine = null;
            }
        }

        private IEnumerator UpdateSizeCoroutine()
        {
            _container.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container);

            yield return null;
            
            RecalculateSizes();
        }

        private void ClearWords()
        {
            _wordsGroupsMap.Clear();
            
            foreach (var wordsGroup in _wordsGroups)
            {
                wordsGroup.ClearWords();
                wordsGroup.Disable();
                _wordsGroupsFree.Add(wordsGroup);
            }
            
            _wordsGroups.Clear();
            _indexWords.Clear();
            _indexWordsReverse.Clear();
        }

        private List<WordsGroup> GetOrCreateWordsGroup(int columns)
        {
            var wordsGroups = new List<WordsGroup>(columns);
            
            for (int i = 0; i < columns; i++)
            {
                WordsGroup wordsGroup;
                
                if (_wordsGroupsFree.Count > 0)
                {
                    wordsGroup = _wordsGroupsFree[^1];
                    _wordsGroupsFree.RemoveAt(_wordsGroupsFree.Count - 1);
                }
                else
                {
                    wordsGroup = Instantiate(_wordGroupPrefab, _container);
                }

                wordsGroups.Add(wordsGroup);
                _wordsGroups.Add(wordsGroup);
            }
            
            return wordsGroups;
        }

        private void OnDestroy()
        {
            StopUpdateCoroutine();
        }

#if UNITY_EDITOR

        [SerializeField] private List<string> _wordsTest;
        [SerializeField] private int _columns;
        
        [Button]
        public void CreateWords()
        {
            CreateWords(_wordsTest.ToArray(), _columns);
        }
        
        [Button("ClearWords")]
        public void ClearWordsEditor()
        {
            ClearWords();
        }
        
#endif
    }
}