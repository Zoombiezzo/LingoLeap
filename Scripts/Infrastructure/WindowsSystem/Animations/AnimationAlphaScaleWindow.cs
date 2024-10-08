using System;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Animations
{
    public class AnimationAlphaScaleWindow: AnimationWindow
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
        private float _startAlpha;
        private Vector3 _startScale;

        private void Awake()
        {
            _startAlpha = 1f;
            _startScale = _rectTransform.localScale;
        }

        public override void Play()
        {
            _canvasGroup.alpha = _startAlpha + _curveAlpha.Evaluate(0f) * _multiplierAlpha;
            _rectTransform.localScale = _startScale + _curveScale.Evaluate(0f) * _multiplierScale * Vector3.one;
            
            IsPlayed = true;
            IsFinished = false;
            var x = 0f;

            _sequence = DOTween.Sequence();
            _sequence.Append(DOTween.To(() => x, value =>
                {
                    x = value;
                    _canvasGroup.alpha = _startAlpha + _curveAlpha.Evaluate(value) * _multiplierAlpha * 1f;
                }, 1f, _duration))
                .Join(DOTween.To(() => x, value =>
                {
                    x = value;
                    _rectTransform.localScale =
                        _startScale + _curveScale.Evaluate(value) * _multiplierScale * Vector3.one;
                }, 1f, _duration));

            _sequence.SetEase(_ease);
            
            _sequence.OnComplete(() =>
            {
                IsFinished = true;
            });
        }

        public override void Stop()
        {
            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Complete();
                _sequence.Kill();
            }
            
            IsPlayed = false;
        }
    }
}