using System;
using System.Collections.Generic;
using _Client.Scripts.Helpers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Buttons
{
    [RequireComponent(typeof(Slider))]
    public class ToggleAnimationButton : AnimationButton
    {
        [SerializeField] private bool _isToggled;
        [SerializeField] private Slider _slider;

        [SerializeField] private bool _animated = true;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _duration;
        [SerializeField] private List<ToggleValueChanger> _valueChangers;
        
        private float _value;
        private Sequence _sequence;
        
        public event Action<bool> OnValueChanged;

        protected override void Awake()
        {
            base.Awake();

            ChangeValue(false);
        }

        public void SetValue(bool value)
        {
            _isToggled = value;
            ChangeValue(_animated);
            OnValueChanged?.Invoke(_isToggled);
        }
        
        public void SetValue(bool value, bool animate)
        {
            _isToggled = value;
            ChangeValue(animate);
            OnValueChanged?.Invoke(_isToggled);
        }

        protected override void Press()
        {
            if(_isInteractable == false)
                return;
            
            SetValue(!_isToggled);
            base.Press();
        }

        private void ChangeValue(bool animate = false)
        {
            var endValue = _isToggled ? 1f : 0f;
            var fromValue = 1f - endValue;

            if (animate)
            {
                if (_sequence != null && _sequence.IsPlaying())
                {
                    _sequence.Kill();
                }

                _sequence = DOTween.Sequence();
                var startValue = _value;
                var duration = Mathf.Abs(startValue - endValue) / 1f * _duration;
                _sequence.Append(DOVirtual.Float(startValue, endValue, duration, value =>
                {
                    var normalizedValue = value.Remap(fromValue, endValue, 0f, 1f);
                    _value = value;
                    var currentValue = _curve.Evaluate(normalizedValue).Remap(0f, 1f, fromValue, endValue);
                    _slider.value = currentValue;
                    ChangeValueOnChangers(currentValue);
                    
                }));
                _sequence.OnComplete(() =>
                {
                    _value = endValue;
                    _slider.value = endValue;
                    _sequence = null;
                });
                _sequence.OnKill(() => { _sequence = null; });
            }
            else
            {
                _value = endValue;
                _slider.value = endValue;
                ChangeValueOnChangers(endValue);
            }
        }

        private void ChangeValueOnChangers(float value)
        {
            if(_valueChangers == null)
                return;
            
            foreach (var changer in _valueChangers)
            {
                changer.ChangeValue(value);
            }
        }

        private void Reset()
        {
            TryGetComponent(out _slider);
        }
    }
}