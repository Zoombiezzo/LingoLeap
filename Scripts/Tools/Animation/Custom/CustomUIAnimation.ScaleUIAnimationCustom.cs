using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable]
        [HideLabel]
        private class ScaleUIAnimationCustom : CustomAnimation
        {
            [SerializeField] private RectTransform _rectTransform;
            [SerializeField] private bool _useSeparateScale;
            [SerializeField] [HideIf("_useSeparateScale")]
            private AnimationCurve _curveScale;
            [SerializeField] [HideIf("_useSeparateScale")] 
            private float _multiplierScale;
            [SerializeField] [ShowIf("_useSeparateScale")]
            private AnimationCurve _curveXScale;
            [SerializeField] [ShowIf("_useSeparateScale")]
            private float _multiplierXScale;
            [SerializeField] [ShowIf("_useSeparateScale")]
            private AnimationCurve _curveYScale;
            [SerializeField] [ShowIf("_useSeparateScale")]
            private float _multiplierYScale;
            [SerializeField] private AnimationCurve _ease;
            [SerializeField] private float _duration = 1f;

            private Vector3 _startedScale;
            
            public ScaleUIAnimationCustom()
            {
                
            }
            
            public ScaleUIAnimationCustom(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }
            
            public override void Initialize()
            {
                _startedScale = _rectTransform.localScale;
            }

            public override Sequence Create()
            {
                _rectTransform.localScale = _curveScale.Evaluate(0f) * _multiplierScale * Vector3.one;

                var sequence = DOTween.Sequence();
                sequence.Append(DOVirtual.Float(0f, 1f, _duration,
                    value =>
                    {
                        var scale = _useSeparateScale
                            ? new Vector2(_curveXScale.Evaluate(value) * _multiplierXScale,
                                _curveYScale.Evaluate(value) * _multiplierYScale)
                            : _curveScale.Evaluate(value) * _multiplierScale * Vector2.one;
                        
                        _rectTransform.localScale = scale;
                    }));

                sequence.SetEase(_ease);

                return sequence;
            }

            public override void Reset()
            {
                _rectTransform.localScale = _startedScale;
            }
        }
    }
}