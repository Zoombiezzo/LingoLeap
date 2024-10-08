using System.Collections.Generic;
using _Client.Scripts.Tools.Animation;
using AssetKits.ParticleImage;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public class BoosterWordPenAnimationView : MonoBehaviour
    {
        [SerializeField] private UiAnimation _animationIdle;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _hideAnimation;
        [SerializeField] private ParticleImage[] _idleParticles;
        [SerializeField] private float _duration;
        [SerializeField] private float _durationOnTarget;
        [SerializeField] private float _durationTranslate;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;
        
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
        
        private BoosterWordAnimator _boosterWordAnimator;
        
        [SerializeField]
        private List<RectTransform> _targets;

        private Sequence _sequence;
        
        public float Duration => _duration;

        public Sequence PlayAnimation(List<RectTransform> targets, float durationTranslate, float durationOnTarget)
        {
            _durationTranslate = durationTranslate;
            _durationOnTarget = durationOnTarget;
            _targets = targets;
            return PlayAnimation();
        }

        public void Register(BoosterWordAnimator boosterWordAnimator)
        {
            _boosterWordAnimator = boosterWordAnimator;
        }

        [Button]
        private Sequence PlayAnimation()
        {
            if(_sequence != null && _sequence.IsPlaying())
                _sequence.Kill();
            
            _sequence = DOTween.Sequence();

            if(_targets == null || _targets.Count == 0)
                return _sequence;
            
            _rectTransform.position = _targets[0].position;
            
            _sequence.Append(_showAnimation.Play());
            _sequence.AppendInterval(_durationOnTarget);
            for (var index = 0; index < _targets.Count - 1; index++)
            {
                var from = _targets[index];
                var to = _targets[index + 1];
                _sequence
                    .Append(_rectTransform.DOMove(from.position, 0))
                    .Append(CreateAnimation(from.position, to.position))
                    .AppendInterval(_durationOnTarget);
            }

            _sequence
                .Append(_rectTransform.DOMove(_targets[^1].position, 0));
            _sequence.Append(_hideAnimation.Play());
            _animationIdle.Play();
            _sequence.OnComplete(OnComplete);

            foreach (var particle in _idleParticles)
            {
                particle.Play();
            }

            return _sequence;
        }

        private void OnComplete()
        {
            _animationIdle.Stop();
            
            foreach (var particle in _idleParticles)
            {
                particle.Stop();
                particle.Clear();
            }
            
            Release();
        }
        
        private void Release()
        {
            _boosterWordAnimator.Release(this);
        }

        private Sequence CreateAnimation(Vector2 from , Vector2 to)
        {
            var startedPosition =from;
            var endPosition =to;
            
            var sequence = DOTween.Sequence();
            sequence.Append(DOVirtual.Float(0f, 1f, _durationTranslate,
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
    }
}