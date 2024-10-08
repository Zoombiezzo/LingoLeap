using System;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class ScaleUIAnimation : UiAnimation
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private AnimationCurve _curveScale;
        [SerializeField] private float _multiplierScale;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private float _loopDelay = 0f;
        
        private Sequence _sequence;
        private Action _onComplete;
        private Vector3 _startScale;

        private void Awake()
        {
            _startScale = _rectTransform.localScale;
        }

        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            _rectTransform.localScale = _curveScale.Evaluate(0f) * _multiplierScale * Vector3.one;
            
            IsPlaying = true;
            IsFinished = false;
            var x = 0f;

            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => x, value =>
                {
                    x = value;
                    _rectTransform.localScale = _curveScale.Evaluate(value) * _multiplierScale * Vector3.one;
                }, 1f, _duration));
            
            if (_isLoop)
            {
                sequence.AppendInterval(_loopDelay);
            }

            sequence.SetEase(_ease);

            if (_isLoop)
            {
                sequence.SetLoops(int.MaxValue);
            }
            
            sequence.OnComplete(() =>
            {
                IsFinished = true;
                _onComplete?.Invoke();
            });

            _sequence = DOTween.Sequence().Append(sequence);

            return _sequence;
        }

        public override void Stop()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Complete();
                _sequence.Kill();
                _onComplete?.Invoke();
            }
            
            IsPlaying = false;

            _rectTransform.localScale = _startScale;
        }
    }
}