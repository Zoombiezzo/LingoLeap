using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class AlphaUIAnimation : UiAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private AnimationCurve _curveAlpha;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private float _multiplierAlpha = 1f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private float _loopDelay = 0f;
        
        private Sequence _sequence;
        private Action _onComplete;
        private float _startAlpha;
        
        private void Awake()
        {
            _startAlpha = _canvasGroup.alpha;
        }
        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            _canvasGroup.alpha = _curveAlpha.Evaluate(0f) * _multiplierAlpha;
            
            IsPlaying = true;
            IsFinished = false;

            var sequence = DOTween.Sequence();
            
            if(_duration <= 0f)
            {
                _canvasGroup.alpha = _curveAlpha.Evaluate(1f) * _multiplierAlpha;
                IsFinished = true;
                _onComplete?.Invoke();
                sequence.Complete();
                return sequence;
            }
            
            sequence.Append(DOVirtual.Float(0f, 1f, _duration, value =>
            {
                _canvasGroup.alpha = _curveAlpha.Evaluate(value) * _multiplierAlpha;
            }));

            sequence.SetEase(_ease);
            
            if (_isLoop)
            {
                sequence.AppendInterval(_loopDelay);
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

        [Button]
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
        }
    }
}