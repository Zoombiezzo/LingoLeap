using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public class UIAnimationGroupAppend : UiAnimation
    {
        [SerializeField] private List<UiAnimation> _animations;
        
        private Sequence _sequence;
        private Action _onComplete;

        public override Sequence Play(Action onComplete = null)
        {
            Stop();
            
            _onComplete = onComplete;
            
            IsPlaying = true;
            IsFinished = false;
            
            var sequence = DOTween.Sequence();

            foreach (var uiAnimation in _animations)
            {
                sequence.Append(uiAnimation.Play());
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
                foreach (var uiAnimation in _animations)
                {
                    if (uiAnimation.IsPlaying)
                    {
                        uiAnimation.Stop();
                    }
                }
                
                _sequence.Complete();
                _sequence.Kill();
                _onComplete?.Invoke();

                _sequence = null;
            }
            
            IsPlaying = false;
        }
    }
}