using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Animations
{
    public class AnimationAlphaWindow: AnimationWindow
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _ease = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private Sequence _sequence;

        public override void Play()
        {
            _canvasGroup.alpha = _curve.Evaluate(0f);
            IsPlayed = true;
            IsFinished = false;

            _sequence = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 1f, _duration, value => _canvasGroup.alpha = _curve.Evaluate(value))
                    .SetEase(_ease))
                .SetEase(_ease)
                .OnComplete(() =>
                {
                    _canvasGroup.alpha = 0f;
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