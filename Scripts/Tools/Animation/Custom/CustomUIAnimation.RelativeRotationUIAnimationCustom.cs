using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable] [HideLabel]
        private class RelativeRotationUIAnimationCustom : CustomAnimation
        {
            private const float WIDTH_LABEL = 50f;
            [SerializeField] private RectTransform _rectTransform;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useX == false")]
            private AnimationCurve _curveX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/X")] [HorizontalGroup("COORDINATES/X/VALUE")]
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useX == false")]
            private float _modifierX;
            [SerializeField] [FoldoutGroup("COORDINATES")] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useY;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useY == false")]
            private AnimationCurve _curveY;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Y")] [HorizontalGroup("COORDINATES/Y/VALUE")] 
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useY == false")]
            private float _modifierY;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Z")] [HorizontalGroup("COORDINATES/Z/VALUE")]
            [LabelWidth(WIDTH_LABEL)]
            private bool _useZ;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Z")] [HorizontalGroup("COORDINATES/Z/VALUE")] 
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useZ == false")]
            private AnimationCurve _curveZ;
            [SerializeField] [FoldoutGroup("COORDINATES")] [VerticalGroup("COORDINATES/Z")] [HorizontalGroup("COORDINATES/Z/VALUE")] 
            [LabelWidth(WIDTH_LABEL)]  [DisableIf("@_useZ == false")]
            private float _modifierZ;
            [SerializeField] 
            private AnimationCurve _ease;
            [SerializeField] 
            private float _duration = 1f;

            private Quaternion _startedRotation;
            
            public RelativeRotationUIAnimationCustom()
            {
                
            }
            
            public RelativeRotationUIAnimationCustom(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }
            
            public override void Initialize()
            {
                _startedRotation = _rectTransform.localRotation;
            }

            public override Sequence Create()
            {
                var sequence = DOTween.Sequence();
                var startedRotation = _startedRotation.eulerAngles;
                sequence.Append(DOVirtual.Float(0f, 1f, _duration,
                    value =>
                    {
                        var x = _startedRotation.x + (_useX ? _curveX.Evaluate(value) * _modifierX : 0);
                        var y = _startedRotation.y + (_useY ? _curveY.Evaluate(value) * _modifierY : 0);
                        var z = _startedRotation.z + (_useZ ? _curveZ.Evaluate(value) * _modifierZ : 0);
                        _rectTransform.localRotation = Quaternion.Euler(startedRotation.x + x, startedRotation.y + y, startedRotation.z + z);
                    }));

                sequence.SetEase(_ease);
            
                return sequence;
            }

            public override void Reset()
            {
                _rectTransform.localRotation = _startedRotation;
            }
        }
    }
}