using _Client.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Common
{
    public class ParallaxScrollEffect : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _target;
        [SerializeField] private bool _changePosition;
        [SerializeField] private Vector2 _minPosition;
        [SerializeField] private Vector2 _maxPosition;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private AnimationCurve _additionalCurve;
        [SerializeField] private float _additionalMaxOffset;
        
        private Vector2 _defaultPosition;
        
        private Vector2 _previousPosition;

        private void Start()
        {
            _defaultPosition = _target.anchoredPosition;
            
            OnValueChanged(_scrollRect.normalizedPosition);
        }

        private void OnEnable()
        {
            Subscribe();
        }
        
        private void Subscribe()
        {
            _scrollRect.onValueChanged.AddListener(OnValueChanged);
        }
        
        private void Unsubscribe()
        {
            _scrollRect.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnValueChanged(Vector2 position)
        {
            if((position - _previousPosition).sqrMagnitude >= 1f)
                return;

            _previousPosition = position;
            
            position.y = _scrollRect.vertical == false ? 0 : 1f - position.y;
            position.x = _scrollRect.horizontal == false ? 0 : position.x;
            
            var additionalValue = new Vector2(GetDifference(position.x), GetDifference(position.y));

            additionalValue.x = additionalValue.x.Remap(-_additionalMaxOffset, _additionalMaxOffset, -1f, 1f);
            additionalValue.y = additionalValue.y.Remap(-_additionalMaxOffset, _additionalMaxOffset, -1f, 1f);
            
            if (_changePosition)
            {
                var x = _defaultPosition.x;
                var y = _defaultPosition.y;

                if (_scrollRect.horizontal)
                {
                    x = _minPosition.x + Mathf.Lerp(_minPosition.x, _maxPosition.x, _curve.Evaluate(position.x));

                    var absAdditionalValue = Mathf.Abs(additionalValue.x);
                    if (absAdditionalValue > Mathf.Epsilon)
                        x += Mathf.Sign(additionalValue.x) * Mathf.Lerp(0, _maxPosition.x, _additionalCurve.Evaluate(absAdditionalValue));

                }

                if (_scrollRect.vertical)
                {
                    y = _minPosition.y + Mathf.Lerp(_minPosition.y, _maxPosition.y, _curve.Evaluate(position.y));
                    
                    var absAdditionalValue = Mathf.Abs(additionalValue.y);
                    if (absAdditionalValue > Mathf.Epsilon) 
                        y += Mathf.Sign(additionalValue.y) * Mathf.Lerp(0, _maxPosition.y, _additionalCurve.Evaluate(absAdditionalValue));
                }

                _target.anchoredPosition = new Vector2(x, y);
            }
        }
        
        private float GetDifference(float value) =>
            value switch
            {
                > 1 => value - 1,
                < 0 => value,
                _ => 0
            };
    }
}