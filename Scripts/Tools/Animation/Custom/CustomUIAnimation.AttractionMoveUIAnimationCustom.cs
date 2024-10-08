using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable] [HideLabel]
        private class AttractionMoveUIAnimationCustom : CustomAnimation
        {
            private const float WIDTH_LABEL = 50f;
            
            [SerializeField] private RectTransform _rectTransform;
            [SerializeField] [FoldoutGroup("TARGET")]
            [LabelWidth(WIDTH_LABEL)]
            private RectTransform _targetRectTransform;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useX;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useX == false")]
            private AnimationCurve _curveX;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useX == false")]
            private float _modifierX;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useY;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useY == false")]
            private AnimationCurve _curveY;
            [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)] [DisableIf("@_useY == false")]
            private float _modifierY;
            [SerializeField] 
            private AnimationCurve _ease;
            [SerializeField] 
            private float _duration = 1f;

            private Vector2 _startedPosition;
            
            public AttractionMoveUIAnimationCustom()
            {
                
            }
            
            public AttractionMoveUIAnimationCustom(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }
            
            public override void Initialize()
            {
                _startedPosition = _rectTransform.position;
            }

            public void SetTarget(RectTransform target)
            {
                _targetRectTransform = target;
            }

            public override Sequence Create()
            {
                var startedPosition = _startedPosition;
                var endPosition = (Vector2)_targetRectTransform.position;
                
                var sequence = DOTween.Sequence();
                sequence.Append(DOVirtual.Float(0f, 1f, _duration,
                    value =>
                    {
                        var position = Vector2.Lerp(startedPosition, endPosition, value);
                        var x = _useX ? _curveX.Evaluate(value) * _modifierX : 0f;
                        var y = _useY ? _curveY.Evaluate(value) * _modifierY : 0f;
                        _rectTransform.position = position + new Vector2(x, y);
                    }));

                sequence.SetEase(_ease);
            
                return sequence;
            }

            public override void Reset()
            {
                _rectTransform.position = _startedPosition;
            }
        }
    }
}