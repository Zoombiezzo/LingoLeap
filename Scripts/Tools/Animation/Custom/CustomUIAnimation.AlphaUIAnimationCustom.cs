using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable] [HideLabel]
        private class AlphaUIAnimationCustom : CustomAnimation
        {
            [SerializeField] private CanvasGroup _canvasGroup;
            [SerializeField] private AnimationCurve _curveAlpha;
            [SerializeField] private AnimationCurve _ease;
            [SerializeField] private float _multiplierAlpha = 1f;
            [SerializeField] private float _duration = 1f;

            private float _startedAlpha;

            public AlphaUIAnimationCustom()
            {
                
            }

            public AlphaUIAnimationCustom(CanvasGroup canvasGroup)
            {
                _canvasGroup = canvasGroup;
            }
            
            public override void Initialize()
            {
                _startedAlpha = _canvasGroup.alpha;
            }

            public override Sequence Create()
            {
                _canvasGroup.alpha = _curveAlpha.Evaluate(0f) * _multiplierAlpha;

                var sequence = DOTween.Sequence();
                sequence.Append(DOVirtual.Float(0f, 1f, _duration,
                    value => _canvasGroup.alpha = _curveAlpha.Evaluate(value) * _multiplierAlpha));

                sequence.SetEase(_ease);
                
                return sequence;
            }

            public override void Reset()
            {
                _canvasGroup.alpha = _startedAlpha;
            }
        }
    }
}