using _Client.Scripts.Tools.Animation.Custom;
using AssetKits.ParticleImage;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordAnimationView : MonoBehaviour
    {
        [SerializeField] private ParticleImage _particleImage;
        [SerializeField] private CustomUIAnimation _customAnimation;
        [SerializeField] private float _duration;
        [SerializeField] private RectTransform _rectTransform;
        
        [SerializeField] [FoldoutGroup("TARGET")]
        private RectTransform _targetRectTransform;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
        private bool _useX;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
        [DisableIf("@_useX == false")]
        private AnimationCurve _curveX;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/X")] [HorizontalGroup("OFFSET/X/VALUE")]
        [DisableIf("@_useX == false")]
        private float _modifierX;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")]
        private bool _useY;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
        [DisableIf("@_useY == false")]
        private AnimationCurve _curveY;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
        [DisableIf("@_useY == false")]
        private float _modifierY;
        [SerializeField] 
        private AnimationCurve _ease;

        public AdditionalWordAnimator _animator;

        private RectTransform _from;
        private RectTransform _to;
        
        public float Duration => _duration;

        public void PlayAnimation(RectTransform from, RectTransform target)
        {
            _from = from;
            _to = target;
            _rectTransform.position = _from.position;
            _particleImage.Play();
            var sequence = _customAnimation.Play();
            sequence.Join(CreateAnimation());
            sequence.OnComplete(OnComplete);
        }

        public void Clear()
        {
            _particleImage.Clear();
            _particleImage.Stop();
            _customAnimation.Stop();
        }

        internal void AttachAnimator(AdditionalWordAnimator animator)
        {
            _animator = animator;
        }
        
        public Sequence CreateAnimation()
        {
            var startedPosition = (Vector2)_from.position;
            var endPosition = (Vector2)_to.position;
                
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

        private void OnComplete()
        {
            _particleImage.Stop();
            _animator.Free(this);
        }
    }
}