using _Client.Scripts.Tools.Animation.Custom;
using AssetKits.ParticleImage;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar
{
    public class BoosterCharAnimationView : MonoBehaviour
    {
        [SerializeField] private ParticleImage _particleImage;
        [SerializeField] private ParticleImage _burstParticles;
        [SerializeField] private float _duration;
        [SerializeField] private CanvasGroup _canvasGroup;
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
        private AnimationCurve _modifierX;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")]
        private bool _useY;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
        [DisableIf("@_useY == false")]
        private AnimationCurve _curveY;
        [SerializeField] [FoldoutGroup("OFFSET")] [VerticalGroup("OFFSET/Y")] [HorizontalGroup("OFFSET/Y/VALUE")] 
        [DisableIf("@_useY == false")]
        private AnimationCurve _modifierY;
        [SerializeField] 
        private AnimationCurve _ease;
        
        private BoosterCharAnimator _boosterCharAnimator;
        private RectTransform _from;
        private RectTransform _to;
        
        public float Duration => _duration;

        public Sequence PlayAnimation(RectTransform from, RectTransform target)
        {
            _from = from;
            _to = target;
            
            _rectTransform.position = _from.position;
            _particleImage.Clear();
            _burstParticles.Clear();
            _particleImage.Play();
            var sequence = DOTween.Sequence();
            sequence.Append(CreateAnimation());
            sequence.AppendCallback(PlayBurstAnimation);
            sequence.AppendInterval(_burstParticles.duration);
            sequence.OnComplete(OnComplete);
            return sequence;
        }

        private void PlayBurstAnimation()
        {
            _particleImage.Stop();
            _particleImage.Clear();
            _burstParticles.Play();
        }

        private Sequence CreateAnimation()
        {
            var startedPosition = (Vector2)_from.position;
            var endPosition = (Vector2)_to.position;
            var modifierX = _modifierX.Evaluate(Random.Range(0f, 1f));
            var modifierY = _modifierY.Evaluate(Random.Range(0f, 1f));
            
            var sequence = DOTween.Sequence();
            sequence.Append(_canvasGroup.DOFade(0f, 0f));
            sequence.Append(_canvasGroup.DOFade(1f, _duration/3f));
            sequence.Join(DOVirtual.Float(0f, 1f, _duration,
                value =>
                {
                    var position = Vector2.Lerp(startedPosition, endPosition, value);
                    var x = _useX ? _curveX.Evaluate(value) * modifierX : 0f;
                    var y = _useY ? _curveY.Evaluate(value) * modifierY : 0f;
                    _rectTransform.position = position + new Vector2(x, y);
                }));

            sequence.SetEase(_ease);
            
            return sequence;
        }

        private void OnComplete()
        {
            _particleImage.Stop();
            _burstParticles.Stop();
            _burstParticles.Clear();
            _particleImage.Clear();
            Release();
        }

        private void Release()
        {
            _boosterCharAnimator.Release(this);
        }

        public void Register(BoosterCharAnimator boosterCharAnimator)
        {
            _boosterCharAnimator = boosterCharAnimator;
        }
    }
}