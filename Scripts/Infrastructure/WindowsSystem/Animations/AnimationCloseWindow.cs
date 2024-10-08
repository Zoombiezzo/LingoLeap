using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Animations
{
    public class AnimationCloseWindow: AnimationWindow
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _duration = 1f;

        private Sequence _sequence;
        public override void Play()
        {
            _canvasGroup.alpha = 1f;
            IsPlayed = true;
            IsFinished = false;

            _sequence = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(0f, _duration)).SetEase(Ease.Linear)
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