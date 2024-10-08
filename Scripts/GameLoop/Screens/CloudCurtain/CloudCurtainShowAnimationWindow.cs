using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.CloudCurtain
{
    public class CloudCurtainShowAnimationWindow : AnimationWindow
    {
        [SerializeField] private RectTransform _rectTransformParent;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _canvasGroupBlocker;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private UiAnimation _uiAnimation;
        [SerializeField] private float _duration = 1f;

        private Sequence _sequence;
        [Button]
        public override void Play()
        {
            if(_sequence != null && _sequence.IsPlaying())
                _sequence.Kill();
            
            IsPlayed = true;
            IsFinished = false;

            _rectTransform.offsetMax = new Vector2(0, 0);
            _rectTransform.offsetMin = new Vector2(0, 0);
            _canvasGroup.alpha = 1f;
            
            _uiAnimation?.Play();
            
            _sequence = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 1f, _duration, value =>
                {
                    _canvasGroupBlocker.alpha = value;
                    var curveValue = _curve.Evaluate(value);
                    var position = Mathf.Lerp(-_rectTransformParent.sizeDelta.y, 0, curveValue);
                    var offset = _rectTransform.offsetMax;
                    _rectTransform.offsetMax = new Vector2(offset.x, position);
                })).SetEase(Ease.Linear)
                .OnComplete(() =>
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