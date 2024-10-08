using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class ParticleImageUIAnimation : UiAnimation
    {
        [SerializeField] private List<ParticleImage> _particleImages;

        private Sequence _sequence;
        private Action _onComplete;

        public override Sequence Play(Action onComplete = null)
        {
            _onComplete = onComplete;
            
            IsPlaying = true;
            IsFinished = false;

            var sequence = DOTween.Sequence();
            
            foreach (var particleImage in _particleImages)
            {
                sequence.JoinCallback(particleImage.Play);
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

                OnKill();
            }
            
            IsPlaying = false;
        }

        private void OnKill()
        {
            foreach (var particleImage in _particleImages)
            {
                particleImage.Stop();
            }
                
            IsPlaying = false;
        }
    }
}