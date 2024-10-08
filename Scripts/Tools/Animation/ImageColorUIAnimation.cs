using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Tools.Animation
{
    public class ImageColorUIAnimation : UiAnimation
    {
        [SerializeField] private Image _image;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private float _loopDelay = 0f;
        
        private Sequence _sequence;
        private Action _onComplete;
        private Color _startColor;

        private void Awake()
        {
            _startColor = _image.color;
        }

        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            _image.color = _gradient.Evaluate(0f);
            
            IsPlaying = true;
            IsFinished = false;
            var x = 0f;

            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => x, value =>
                {
                    x = value;
                    _image.color = _gradient.Evaluate(value);
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

            _image.color = _startColor;
        }
    }
}
