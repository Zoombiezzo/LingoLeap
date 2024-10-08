using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.CloudCurtain
{
    public class CloudCurtainHideAnimationWindow : AnimationWindow
    {
        [SerializeField] private RectTransform _rectTransformParent;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _canvasGroupBlocker;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private UiAnimation _uiAnimation;
        [SerializeField] private float _offset = 100f;
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
            
            _sequence = DOTween.Sequence()
                .Append(DOVirtual.Float(0f, 1f, _duration, value =>
                {
                    _canvasGroupBlocker.alpha = 1f - value;
                    var curveValue = _curve.Evaluate(value);
                    var position = Mathf.Lerp(0, _rectTransformParent.sizeDelta.y + _offset, curveValue);
                    var offset = _rectTransform.offsetMin;
                    _rectTransform.offsetMin = new Vector2(offset.x, position);
                })).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _uiAnimation?.Stop();
                    IsFinished = true;
                    _canvasGroup.alpha = 0f;
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
            _uiAnimation?.Stop();
        }
    }
}