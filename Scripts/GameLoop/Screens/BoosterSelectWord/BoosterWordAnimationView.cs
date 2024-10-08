using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public class BoosterWordAnimationView : MonoBehaviour
    {
        [SerializeField] private ParticleImage _burstParticles;
        [SerializeField] private float _duration;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;
        
        private BoosterWordAnimator _boosterWordAnimator;
        private RectTransform _to;
        
        public float Duration => _duration;

        public Sequence PlayAnimation(RectTransform target)
        {
            _to = target;
            
            _rectTransform.position = _to.position;
            _burstParticles.Clear();
            var sequence = DOTween.Sequence();
            sequence.Append(CreateAnimation());
            sequence.AppendCallback(PlayBurstAnimation);
            sequence.AppendInterval(_burstParticles.duration);
            sequence.OnComplete(OnComplete);
            return sequence;
        }

        private void PlayBurstAnimation()
        {
            _burstParticles.Play();
        }

        private Sequence CreateAnimation()
        {
            var endPosition = (Vector2)_to.position;
            _rectTransform.position = endPosition;
            var sequence = DOTween.Sequence();
            
            sequence.Append(_canvasGroup.DOFade(0f, 0f));
            sequence.Append(_canvasGroup.DOFade(1f, _duration / 3f));
            
            return sequence;
        }

        private void OnComplete()
        {
            _burstParticles.Stop();
            _burstParticles.Clear();
            Release();
        }

        private void Release()
        {
            _boosterWordAnimator.Release(this);
        }

        public void Register(BoosterWordAnimator boosterCharAnimator)
        {
            _boosterWordAnimator = boosterCharAnimator;
        }
    }
}