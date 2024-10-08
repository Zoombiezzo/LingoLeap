using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable] [HideLabel]
        private class MoveUIAnimationCustom : CustomAnimation
        {
            private const float WIDTH_LABEL = 50f;
            
            [SerializeField] private RectTransform _rectTransform;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useX == false")]
            private AnimationCurve _curveX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useX == false")]
            private float _modifierX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useY;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useY == false")]
            private AnimationCurve _curveY;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useY == false")]
            private float _modifierY;
            [SerializeField] 
            private AnimationCurve _ease;
            [SerializeField] 
            private float _duration = 1f;

            private Vector2 _startedAnchoredPosition;

            public MoveUIAnimationCustom()
            {
                
            }
            
            public MoveUIAnimationCustom(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }
            
            public override void Initialize()
            {
                _startedAnchoredPosition = _rectTransform.anchoredPosition;
            }

            public override Sequence Create()
            {
                var sequence = DOTween.Sequence();
                sequence.Append(DOVirtual.Float(0f, 1f, _duration,
                    value =>
                    {
                        var x = _useX ? _curveX.Evaluate(value) * _modifierX : 0f;
                        var y = _useY ? _curveY.Evaluate(value) * _modifierY : 0f;
                        _rectTransform.anchoredPosition = new Vector3(x, y);
                    }));

                sequence.SetEase(_ease);
            
                return sequence;
            }

            public override void Reset()
            {
                _rectTransform.anchoredPosition = _startedAnchoredPosition;
            }
        }
    }
}