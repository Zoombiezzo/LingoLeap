using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Client.Scripts.Tools
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CounterField : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _duration;

        private Sequence _sequence;
        private float _currentValue;
        private bool _isShowed;

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }
        
        public bool IsShowed => _isShowed;

        private void Awake()
        {
            _isShowed = _canvasGroup.alpha != 0f;
        }

        public void SetValue(int value, bool animatie = false)
        {
            StopAnimation();
            
            if (animatie)
            {
                PlayAnimation((int)_currentValue, value);
            }
            else
            {
                _text.text = value.ToString();
            }

            _currentValue = value;
        }
        public void SetValue(float value, bool animatie = false)
        {
            StopAnimation();
            
            if (animatie)
            {
                PlayAnimation(_currentValue, value);
            }
            else
            {
                _text.text = value.ToString("F1");
            }

            _currentValue = value;
        }
        public void SetValue(string value)
        {
            StopAnimation();
            _text.text = value;
            _currentValue = 0;
        }

        public void Show(bool animate = true)
        {
            if(_isShowed) return;

            if (animate)
            {
                _canvasGroup.DOFade(1f, _duration);
            }
            else
            {
                _canvasGroup.alpha = 1f;
            }

            _isShowed = true;
        }

        public void Hide(bool animate = true)
        {
            if(_isShowed == false) return;

            if (animate)
            {
                _canvasGroup.DOFade(0f, _duration);
            }
            else
            {
                _canvasGroup.alpha = 0f;
            }

            _isShowed = false;
        }

        private void PlayAnimation(int prevValue, int value)
        {
            _sequence = DOTween.Sequence();
            
            _sequence.Append(
                DOTween.To(() => prevValue, newValue => _text.text = ((int)newValue).ToString(), value, _duration).SetEase(Ease.OutCubic));
        }
        
        private void PlayAnimation(float prevValue, float value)
        {
            _sequence = DOTween.Sequence();
            
            _sequence.Append(
                DOTween.To(() => prevValue, newValue => _text.text = newValue.ToString("F1"), value, _duration).SetEase(Ease.OutCubic));
        }

        private void StopAnimation()
        {
            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Complete();
                _sequence.Kill();
            }
        }

        private void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
