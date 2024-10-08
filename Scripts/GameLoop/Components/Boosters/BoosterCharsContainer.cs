using System;
using System.Collections;
using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace _Client.Scripts.GameLoop.Components.Boosters
{
    public class BoosterCharsContainer : BoosterPanel
    {
        [SerializeField] private BoosterCharView _boosterCharViewPrefab;
        [SerializeField] private RectTransform _container;
        
        private readonly Dictionary<BoosterCharView, CharView> _charViewsMap = new(16);
        private readonly Dictionary<CharView, int> _charViewsIndexMap = new(16);
        private readonly Dictionary<CharView, WordView> _charViewsWordMap = new(16);
        private readonly Dictionary<WordView, List<BoosterCharView>> _wordViewsCharsBoosterMap = new(16);
        private readonly List<BoosterCharView> _boosterCharViews = new(16);
        
        private ObjectPool<BoosterCharView> _boosterCharViewPool;
        private readonly CompositeDisposable _disposable = new();
        
        public event Action<BoosterCharView> OnClickedChar;
        
        private void Awake()
        {
            _boosterCharViewPool = new ObjectPool<BoosterCharView>(
                CreateBoosterCharView,
                GetBoosterCharView,
                ReleaseBoosterCharView,
                DestroyBoosterCharView);
        }
        
        public void Initialize(IReadOnlyList<WordView> words)
        {
            _charViewsMap.Clear();
            _charViewsIndexMap.Clear();
            _charViewsWordMap.Clear();
            _boosterCharViews.Clear();
            
            foreach (var word in words)
            {
                if (_wordViewsCharsBoosterMap.TryGetValue(word, out var boosterCharViews))
                {
                    boosterCharViews.Clear();
                }
                else
                {
                    boosterCharViews = new List<BoosterCharView>(16);
                    _wordViewsCharsBoosterMap.Add(word, boosterCharViews);
                }
                
                for (var index = 0; index < word.CharViews.Count; index++)
                {
                    var charView = word.CharViews[index];
                    
                    if(charView.State == CharViewState.Shown)
                        continue;
                    
                    _charViewsIndexMap.Add(charView, index);
                    _charViewsWordMap.Add(charView, word);
                    var view = CreateView(charView);
                    
                    boosterCharViews.Add(view);
                }
            }
        }

        public bool TryGetWordAndChar(BoosterCharView boosterCharView, out string word, out int indexChar)
        {
            word = string.Empty;
            indexChar = -1;
            
            if (boosterCharView == null)
                return false;
            
            if (_charViewsMap.TryGetValue(boosterCharView, out var charView) == false)
                return false;
            
            word = _charViewsWordMap[charView].Word;
            indexChar = _charViewsIndexMap[charView];
            
            return true;
        }

        public bool TryGetCharView(BoosterCharView boosterCharView, out CharView charView)
        {
            charView = null;
            
            if (_charViewsMap.TryGetValue(boosterCharView, out var view) == false)
                return false;
            
            charView = view;
            
            return true;
        }

        public bool GetWordByChar(BoosterCharView boosterCharView, out IReadOnlyList<BoosterCharView> boosterCharViews)
        {
            boosterCharViews = null;
            
            if (_charViewsMap.TryGetValue(boosterCharView, out var charView) == false)
                return false;

            if (_charViewsWordMap.TryGetValue(charView, out var wordView) == false)
                return false;

            if (_wordViewsCharsBoosterMap.TryGetValue(wordView, out var boosterCharViewsWord) == false)
                return false;

            boosterCharViews = boosterCharViewsWord;
            
            return true;
        }
        
        private void OnClickedCharHandler(BoosterCharView boosterCharView)
        {
            OnClickedChar?.Invoke(boosterCharView);
        }
        
        public void ReleaseAll()
        {
            foreach (var boosterCharView in _boosterCharViews)
            {
                boosterCharView.Clear();
                _boosterCharViewPool.Release(boosterCharView);
            }
        }

        public void UpdatePositions()
        {
            foreach (var boosterCharView in _boosterCharViews)
            {
                UpdatePosition(boosterCharView);
            }
        }

        private BoosterCharView CreateView(CharView charView)
        {
            var view = _boosterCharViewPool.Get();
            
            _charViewsMap.Add(view, charView);
            _boosterCharViews.Add(view);
            
            SetValues(view, charView);

            return view;
        }

        private void UpdatePosition(BoosterCharView boosterCharView)
        {
            if(boosterCharView == null)
                return;
            
            if (_charViewsMap.TryGetValue(boosterCharView, out var charView) == false)
                return;
            
            SetValues(boosterCharView, charView);
        }

        private void SetValues(BoosterCharView boosterCharView, CharView charView)
        {
            var boosterRectTransform = boosterCharView.RectTransform;
            var charViewRectTransform = charView.RectTransform;
            
            boosterRectTransform.position = charViewRectTransform.position;
            boosterRectTransform.sizeDelta = charViewRectTransform.sizeDelta;
        }

        private void OnDestroy()
        {
            _boosterCharViewPool.Dispose();
            _disposable?.Dispose();
        }

        private BoosterCharView CreateBoosterCharView()
        {
            var view = Instantiate(_boosterCharViewPrefab, _container);
            _disposable.Add(Observable.FromEvent<BoosterCharView>(h => view.OnClicked += h, h => view.OnClicked -= h).Subscribe(OnClickedCharHandler));
            return view;
        }

        private void GetBoosterCharView(BoosterCharView boosterCharView) => boosterCharView.gameObject.SetActive(true);
        private void ReleaseBoosterCharView(BoosterCharView boosterCharView) => boosterCharView.gameObject.SetActive(false);
        private void DestroyBoosterCharView(BoosterCharView boosterCharView) => Destroy(boosterCharView.gameObject);
    }
}