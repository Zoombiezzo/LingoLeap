using System;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class AlphaScaleUIAnimation : UiAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private AnimationCurve _curveScale;
        [SerializeField] private float _multiplierScale;
        [SerializeField] private AnimationCurve _curveAlpha;
        [SerializeField] private float _multiplierAlpha;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private float _duration = 1f;
        
        private Sequence _sequence;
        private Action _onComplete;
        
        private Vector3 _startScale;
        private float _startAlpha;
        
        private void Awake()
        {
            _startScale = _rectTransform.localScale;
            _startAlpha = _canvasGroup.alpha;
        }

        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            _canvasGroup.alpha = _curveAlpha.Evaluate(0f) * _multiplierAlpha;
            _rectTransform.localScale = _curveScale.Evaluate(0f) * _multiplierScale * Vector3.one;
            
            IsPlaying = true;
            IsFinished = false;
            var x = 0f;

            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => x, value =>
                {
                    x = value;
                    _canvasGroup.alpha = _curveAlpha.Evaluate(value) * _multiplierAlpha * 1f;
                }, 1f, _duration))
                .Join(DOTween.To(() => x, value =>
                {
                    x = value;
                    _rectTransform.localScale = _curveScale.Evaluate(value) * _multiplierScale * Vector3.one;
                }, 1f, _duration));

            sequence.SetEase(_ease);
            
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
            
            _canvasGroup.alpha = _startAlpha;
            _rectTransform.localScale = _startScale;
        }
    }
}