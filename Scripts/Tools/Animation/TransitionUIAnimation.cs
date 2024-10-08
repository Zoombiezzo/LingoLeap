using System;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class TransitionUIAnimation : UiAnimation
    {
        [SerializeField]
        private RectTransform _rootTransform;
        
        [SerializeField]
        private RectTransform _rectTransform;
        
        [SerializeField]
        private float _duration = 1f;
        
        [SerializeField]
        private float _startDelay = 0f;

        [SerializeField] 
        private EdgeType _edgeType;
        
        [SerializeField] 
        private TransitionType _transitionType;
        
        [SerializeField] 
        private AnimationCurve _curveTransition;
        
        [SerializeField] 
        private float _offset;
        
        [SerializeField] 
        private bool _initializeStartPosition = false;

        private Sequence _sequence;
        private Action _onComplete;

        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            if (_rootTransform == null)
            {
                _rootTransform = _rectTransform.parent.GetComponent<RectTransform>();
            }

            var rootPivot = _rootTransform.pivot;
            var rootSizeDelta = _rootTransform.rect.size;
            var rootPosition = (Vector2)_rootTransform.localPosition;
            var rootLocalScale = _rootTransform.localScale;
            var rootLocalScaleSign = new Vector2(Mathf.Sign(rootLocalScale.x), Mathf.Sign(rootLocalScale.y));
            var pivotOffsetRoot = (new Vector2(0.5f, 0.5f) - rootPivot) * rootSizeDelta;
            
            var minRootValue = rootSizeDelta * -0.5f + rootPosition + pivotOffsetRoot  * rootLocalScaleSign;
            var maxRootValue = rootSizeDelta * 0.5f  + rootPosition + pivotOffsetRoot  * rootLocalScaleSign;
            
            var currentPivot = _rectTransform.pivot;
            var currentPosition = (Vector2)_rectTransform.localPosition;

            var sizeDelta = _rectTransform.rect.size;
            
            var pivotOffset = (new Vector2(0.5f, 0.5f) - currentPivot) * sizeDelta;
            var localScale = _rectTransform.localScale;
            var localScaleSign = new Vector2(Mathf.Sign(localScale.x), Mathf.Sign(localScale.y));
            
            var minRectValue =  sizeDelta * -0.5f + currentPosition + pivotOffset* localScaleSign;
            var maxRectValue =  sizeDelta * 0.5f + currentPosition + pivotOffset * localScaleSign;
            
            var endPosition = _transitionType switch
            {
                TransitionType.Outside => GetOutsidePosition(currentPosition, minRootValue, maxRootValue, minRectValue,
                    maxRectValue),
                TransitionType.Inside => GetInsidePosition(currentPosition, minRootValue, maxRootValue, minRectValue,
                    maxRectValue),
                _ => Vector2.zero
            };

            if (_initializeStartPosition)
            {
                var startPosition = _transitionType switch
                {
                    TransitionType.Outside => GetInsidePosition(currentPosition, minRootValue, maxRootValue, minRectValue,
                        maxRectValue),
                    TransitionType.Inside => GetOutsidePosition(currentPosition, minRootValue, maxRootValue, minRectValue,
                        maxRectValue),
                    _ => Vector2.zero
                };
                
                _rectTransform.localPosition = startPosition;
                currentPosition = startPosition;
            }

            IsPlaying = true;
            IsFinished = false;
            
            var sequence = DOTween.Sequence();
            
            if(_startDelay > 0) sequence.AppendInterval(_startDelay);
            sequence.Append(DOVirtual.Float(0f, 1f, _duration, x =>
            {
                var value = _curveTransition.Evaluate(x);
                _rectTransform.localPosition = Vector2.LerpUnclamped(currentPosition, endPosition, value);
            }));
            
            sequence.OnComplete(() =>
            {
                IsFinished = true;
                IsPlaying = false;
                _onComplete?.Invoke();
            });

            _sequence = DOTween.Sequence().Append(sequence);
            
            return _sequence;
        }
        
        private Vector2 GetOutsidePosition(Vector2 currentPosition, Vector2 minRootPosition, Vector2 maxRootPosition, Vector2 minCurrentPosition, Vector2 maxCurrentPosition)
        {
            return _edgeType switch
            {
                EdgeType.Upper => new Vector2(currentPosition.x, currentPosition.y + (maxRootPosition.y - minCurrentPosition.y) + _offset ),
                EdgeType.Right => new Vector2(currentPosition.x + (maxRootPosition.x - minCurrentPosition.x) + _offset, currentPosition.y),
                EdgeType.Lower => new Vector2(currentPosition.x, currentPosition.y + (minRootPosition.y - maxCurrentPosition.y) - _offset),
                EdgeType.Left => new Vector2(currentPosition.x + (minRootPosition.x - maxCurrentPosition.x) - _offset, currentPosition.y),
                _ => Vector2.zero
            };
        }
        
        private Vector2 GetInsidePosition(Vector2 currentPosition, Vector2 minRootPosition, Vector2 maxRootPosition, Vector2 minCurrentPosition, Vector2 maxCurrentPosition)
        {
            return _edgeType switch
            {
                EdgeType.Upper => new Vector2(currentPosition.x, currentPosition.y + (maxRootPosition.y - maxCurrentPosition.y) - _offset),
                EdgeType.Right => new Vector2(currentPosition.x + (maxRootPosition.x - maxCurrentPosition.x) - _offset, currentPosition.y),
                EdgeType.Lower => new Vector2(currentPosition.x, currentPosition.y + (minRootPosition.y - minCurrentPosition.y) + _offset),
                EdgeType.Left => new Vector2(currentPosition.x + (minRootPosition.x - minCurrentPosition.x) + _offset, currentPosition.y),
                _ => Vector2.zero
            };
        }
        
        public override void Stop()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                IsFinished = false;
                IsPlaying = false;
                _sequence.Kill();
                _sequence = null;
            }
        }

        private enum EdgeType
        {
            Upper,
            Right,
            Lower,
            Left
        }
        
        private enum TransitionType
        {
            Inside,
            Outside
        }
    }
}