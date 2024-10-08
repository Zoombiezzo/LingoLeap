using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Progressbar
{
    public class UISliderProgressbarText : UIProgressbar
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float _progress;

        private Sequence _progressSequence;
        
        public override void SetProgress(float progress, bool animatie = false)
        {
            if (_progressSequence != null && _progressSequence.IsPlaying())
            {
                _progressSequence.Kill();
            }
            
            _progress = progress;
            var min = _slider.minValue;
            var max = _slider.maxValue;
            
            if (animatie == false)
            {
                _slider.value = Mathf.Lerp(min, max, _progress);
                return;
            }
            
            _progressSequence = DOTween.Sequence();
            _progressSequence.Append(_slider.DOValue(Mathf.Lerp(min, max, _progress), _duration));
        } 
        
        public void SetProgressText(string text)
        {
            _value.text = text;
        }

        public override void SetProgress(int count, int maxCount, bool animatie = false)
        {
            SetProgress((float)count / maxCount, animatie);

            if (_value != null)
                _value.text = $"{count}/{maxCount}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slider != null)
            {
                var min = _slider.minValue;
                var max = _slider.maxValue;
                _slider.value = Mathf.Lerp(min, max, _progress);
            }
        }
#endif
    }
}