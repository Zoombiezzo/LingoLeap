using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class SpinWheelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _wheelRectTransform;
        [SerializeField] private SpinWheelHandle _handle;
        [SerializeField] private List<SpinWheelItemView> _items;
        [SerializeField] private ParticleImage _spinParticle;
        [SerializeField] private ParticleImage _winPaerticle;
        [SerializeField] private AnimationCurve _baseAnimationCurve;
        [SerializeField] private List<AnimationCurve> _randomCurves;
        
        [SerializeField] private int _baseCountSpins = 10;
        [SerializeField] private float _duration = 5f;
        
        [SerializeField] private bool _useConcreateCurve = true;

        private Sequence _sequence;
        
        public SpinWheelHandle Handle => _handle;
        public IReadOnlyList<SpinWheelItemView> Items => _items;

        public Sequence Spin(int targetItemIndex)
        {
            if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying())
            {
                _sequence.Kill();
                _sequence = null;
            }
            
            var angle = 360f / _items.Count * targetItemIndex;
            var currentAngle = _wheelRectTransform.localEulerAngles.z;
            var targetAngle = (360f - currentAngle + angle) % 360f;
            
            var additionalSpins = _baseCountSpins * 360f;

            var curve = _baseAnimationCurve;

            if (_useConcreateCurve == false) curve = _randomCurves[Random.Range(0, _randomCurves.Count)];
            
            _sequence = CreateSequence(targetAngle + additionalSpins, curve);

            _spinParticle.Play();
            
            return _sequence;
        }

        private Sequence CreateSequence(float targetAngle, AnimationCurve animationCurve)
        {
            var sequence = DOTween.Sequence();
            sequence
                .Append(_wheelRectTransform.DOLocalRotate(new Vector3(0f, 0f, targetAngle),
                        _duration, RotateMode.LocalAxisAdd)
                    .SetEase(animationCurve))
                .AppendCallback(PlayWinParticle);

            return sequence;
        }

        private void PlayWinParticle()
        {
            _winPaerticle.Play();
        }
    }
}