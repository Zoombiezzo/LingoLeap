using System;
using System.Collections.Generic;
using System.Text;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Helpers;
using _Client.Scripts.Tools.Animation;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

namespace _Client.Scripts.GameLoop.Components.WordSelector
{
    public class CircleWordSelector : WordSelector
    {
        [SerializeField] private Vector2 _maxSize;
        [SerializeField] private Vector2 _minSize;
        [SerializeField] private int _minCharsCount;
        [SerializeField] private int _maxCharsCount;
        [SerializeField] private UILineRenderer _lineRenderer;
        [SerializeField] private AnimationButton _buttonShuffle;
        [SerializeField] private ParticleImage _particleImageSpread;
        
        private HashSet<CharSelector> _charsSelectedHashSet = new();
        private readonly StringBuilder _sb = new();
        
        [SerializeField] [Sirenix.OdinInspector.ReadOnly]
        private List<CharSelector> _charsSelected = new();

        [SerializeField]
        private UiAnimation _showAnimation;
        
        [SerializeField]
        private float _shuffleDuration = 0.5f;
        
        private char[] _chars;

        private Sequence _shuffleSequence;

        public override string SelectedWord => _sb.ToString();

        public override char[] Chars => _chars;

        public override event Action<WordSelector> OnCharsChanged;
        public override event Action<WordSelector> OnWordSelected;
        public override event Action<WordSelector> OnShuffle;

        private void OnEnable()
        {
            _buttonShuffle.OnClick.AddListener(OnShuffleClick);
        }
        
        private void OnDisable()
        {
            _buttonShuffle.OnClick.RemoveListener(OnShuffleClick);
        }

        private void OnShuffleClick()
        {
            if(_isBlocked)
                return;
            
            OnShuffle?.Invoke(this);
        }

        public override void SetChars(params char[] chars)
        {
            var size = _parent.rect.size;
            var count = chars.Length;
            _chars = chars;
            
            var sizeChar = Vector2.Lerp(_maxSize, _minSize, MathExtensions.Remap(((float)count), _minCharsCount, _maxCharsCount, 0f, 1f));
            
            ClearChars();
            
            var step = 360f / chars.Length;

            for (var index = 0; index < chars.Length; index++)
            {
                var c = chars[index];
                var charSelector = GetFreeCharSelector(c);
                
                var angle = step * index;
                var position = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * size.x / 2f, Mathf.Cos(angle * Mathf.Deg2Rad) * size.y / 2f);
                charSelector.RegisterWordSelector(this);
                charSelector.SetPosition(position);
                charSelector.SetSize(sizeChar);
                charSelector.Show();
            }
            
            _lineRenderer.Points = Array.Empty<Vector2>();

            InitializeShuffleAnimation();
        }

        public override void SetChars(string chars)
        {
            SetChars(chars.ToCharArray());
        }

        public override void OverrideChars(params char[] chars)
        {
            if(chars.Length != _charsUsed.Count)
                return;
            
            for (int i = 0; i < _charsUsed.Count; i++)
            {
                var charSelector = _charsUsed[i];
                charSelector.SetChar(chars[i].ToString());
            }

            PlayShuffleAnimation();
        }

        public override void Show(bool animate = false)
        {
            base.Show(animate);
            StopAllAnimations();

            _showAnimation?.Play();
        }
        
        public override void Hide(bool animate = false)
        {
            base.Hide(animate);
        }

        public override void Block(bool block)
        {
            base.Block(block);
            
            DeselectAllChars();
            UpdateLineRenderer();
            UpdateSelectedWord();

            if (_charsSelected.Count == 0)
            {
                ShowShuffle();
            }
        }

        public override void ShowEffect()
        {
            if(_particleImageSpread.isPlaying)
                return;
            
            _particleImageSpread.Play();
        }

        private void InitializeShuffleAnimation()
        {
            _shuffleSequence = DOTween.Sequence();
            _shuffleSequence.SetAutoKill(false);

            var interval = _shuffleDuration / _charsUsed.Count;
            
            _shuffleSequence.AppendCallback(() => _isBlocked = true);
            _shuffleSequence.AppendCallback(() => _buttonShuffle.IsInteractable = false);
            
            foreach (var charSelector in _charsUsed)
            {
                _shuffleSequence.AppendCallback(charSelector.HideAnimation);
            }
            
            foreach (var charSelector in _charsUsed)
            {
                _shuffleSequence.AppendInterval(interval);
                _shuffleSequence.AppendCallback(charSelector.ShowAnimation);
            }
            
            _shuffleSequence.AppendCallback(() => _buttonShuffle.IsInteractable = true);
            _shuffleSequence.AppendCallback(() => _isBlocked = false);

            _shuffleSequence.Pause();
        }
        
        private void PlayShuffleAnimation()
        {
            _shuffleSequence?.Restart();
            _shuffleSequence?.Play();
        }
        
        private void ShowShuffle()
        {
            _buttonShuffle.Show();
        }

        private void HideShuffle()
        {
            _buttonShuffle.Hide();
        }

        internal override void OnDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            if(_isBlocked)
                return;
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_parent, pointerEventData.position, pointerEventData.pressEventCamera, out var position);
            UpdateLineRenderer(_parent.transform.InverseTransformPoint(position));
        }

        internal override void OnBeginDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
        }

        internal override void OnEndDragChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
        }

        internal override void OnDownChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            if(_isBlocked)
                return;
            
            if (TrySelectChar(charSelector))
            {
                HideShuffle();
            }
            
            UpdateLineRenderer();
            UpdateSelectedWord();
            
            OnCharsChanged?.Invoke(this);
        }

        internal override void OnUpChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            if(_isBlocked)
                return;
            
            OnWordSelected?.Invoke(this);

            DeselectAllChars();
            UpdateLineRenderer();
            UpdateSelectedWord();

            if (_charsSelected.Count == 0)
            {
                ShowShuffle();
            }
        }
        
        internal override void OnPointerEnterChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            if(_isBlocked)
                return;
            
            if(_charsSelectedHashSet.Count == 0)
                return;

            if (charSelector.IsSelected)
            {
                if (_charsSelected.Count == 1)
                    return;
                
                var prevCharSelector = _charsSelected[^2];
                
                if(prevCharSelector != charSelector)
                    return;
                
                var lastCharSelector = _charsSelected[^1];
                TryDeselectChar(lastCharSelector);
                
                UpdateLineRenderer();
                UpdateSelectedWord();
                OnCharsChanged?.Invoke(this);
                return;
            }
            
            TrySelectChar(charSelector);
            UpdateLineRenderer();
            UpdateSelectedWord();
            
            OnCharsChanged?.Invoke(this);
        }
        
        internal override void OnPointerExitChar(CharSelector charSelector, PointerEventData pointerEventData)
        {
            
        }

        private bool TrySelectChar(CharSelector charSelector)
        {
            if(_charsSelectedHashSet.Add(charSelector) == false)
                return false;

            _charsSelected.Add(charSelector);
            
            charSelector.Select();
            return true;
        }

        private bool TryDeselectChar(CharSelector charSelector)
        {
            if(_charsSelectedHashSet.Remove(charSelector) == false)
                return false;
            
            _charsSelected.Remove(charSelector);
            charSelector.Deselect();
            return true;
        }

        private void DeselectAllChars()
        {
            foreach (var charSelector in _charsSelected)
            {
                charSelector.Deselect();
                _charsSelectedHashSet.Remove(charSelector);
            }
            
            _charsSelected.Clear();
        }

        private void ClearChars()
        {
            foreach (var charSelector in _charsUsed)
            {
                charSelector.Hide();
                _freeChars.Push(charSelector);
            }
            
            _charsUsed.Clear();
        }
        
        private CharSelector GetFreeCharSelector(char c)
        {
            var charSelector = _freeChars.Count > 0 ? _freeChars.Pop() : Instantiate(_charSelectorPrefab, _parent);
            
            charSelector.Hide();
            charSelector.SetChar(c.ToString());
            _charsUsed.Add(charSelector);
            return charSelector;
        }

        private void UpdateLineRenderer(Vector2 pointPosition)
        {
            if (_charsSelected.Count == 0)
            {
                _lineRenderer.Points = Array.Empty<Vector2>();
                return;
            }
            
            var points = new Vector2[_charsSelected.Count + 1];

            for (var index = 0; index < _charsSelected.Count; index++)
            {
                var charSelector = _charsSelected[index];
                points[index] = charSelector.LocalPosition;
            }
            
            points[^1] = pointPosition;
            
            _lineRenderer.Points = points;
        }
        
        private void UpdateLineRenderer()
        {
            if (_charsSelected.Count == 0)
            {
                _lineRenderer.Points = Array.Empty<Vector2>();
                return;
            }
            
            var points = new Vector2[_charsSelected.Count];

            for (var index = 0; index < _charsSelected.Count; index++)
            {
                var charSelector = _charsSelected[index];
                points[index] = charSelector.LocalPosition;
            }
            
            _lineRenderer.Points = points;
        }
        
        private void UpdateSelectedWord()
        {
            _sb.Clear();
            
            foreach (var charSelector in _charsSelected)
            {
                _sb.Append(charSelector.Char);
            }
        }

        private void StopAllAnimations()
        {
            _showAnimation?.Stop();
        }

        private void DisposeAllAnimations()
        {
            _shuffleSequence?.Kill();
            _shuffleSequence = null;
        }

        private void OnDestroy()
        {
            StopAllAnimations();
            DisposeAllAnimations();
        }
    }
}