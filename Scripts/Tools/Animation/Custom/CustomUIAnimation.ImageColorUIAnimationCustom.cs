using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable]
        [HideLabel]
        private class ImageColorUIAnimationCustom : CustomAnimation
        {
            [SerializeField] private Image _image;
            [SerializeField] private Gradient _gradient;
            [SerializeField] private AnimationCurve _ease;
            [SerializeField] private float _duration = 1f;

            private Color _startedColor;

            public ImageColorUIAnimationCustom()
            {
                
            }

            public ImageColorUIAnimationCustom(Image image)
            {
                _image = image;
            }
            
            public override void Initialize()
            {
                _startedColor = _image.color;
            }

            public override Sequence Create()
            {
                _image.color = _gradient.Evaluate(0f);

                var sequence = DOTween.Sequence();
                sequence.Append(DOVirtual.Float(0f, 1f,_duration, value => _image.color = _gradient.Evaluate(value)));
                
                sequence.SetEase(_ease);

                return sequence;
            }

            public override void Reset()
            {
                _image.color = _startedColor;
            }
        }
    }
}