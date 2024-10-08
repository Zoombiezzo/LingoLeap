using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Components.Progressbar
{
    public class UISliderProgressbar : UIProgressbar
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private TMP_Text _maxCount;
        [SerializeField] [Range(0f, 1f)] private float _progress;

        public override void SetProgress(float progress, bool animatie = false)
        {
            _progress = progress;
            var min = _slider.minValue;
            var max = _slider.maxValue;
            _slider.value = Mathf.Lerp(min, max, _progress);
        }

        public override void SetProgress(int count, int maxCount, bool animatie = false)
        {
            SetProgress((float)count / maxCount, animatie);

            if (_count != null)
                _count.text = count.ToString();

            if (_maxCount != null)
                _maxCount.text = maxCount.ToString();
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