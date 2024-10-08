using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Progressbar.StageProgressbar
{
    public class StageProgressbar : UIProgressbar
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private StageElement _prefab;
        [SerializeField] private List<StageElement> _elements = new List<StageElement>(16);

        [SerializeField] private int _progress;
        [SerializeField] private int _maxValue;

        public void Initialize(int maxValue)
        {
            SetMaxValue(maxValue);
        }

        private void SetMaxValue(int maxValue)
        {
            int currentCount = _elements.Count;

            if (maxValue > currentCount)
            {
                for (int i = 0; i < maxValue - currentCount; i++)
                {
                    var element = Instantiate(_prefab, _container);
                    element.Hide();
                    _elements.Add(element);
                }
            }
            
            for (int i = 0; i < currentCount; i++)
            {
                var view = _elements[i];
                view.Hide();
                
                var element = view.gameObject;
                element.transform.SetSiblingIndex(i);
                element.SetActive(i < maxValue);
            }

            _maxValue = maxValue;
        }

        public override void SetProgress(float progress, bool animate = false)
        {
            progress = Mathf.Clamp(progress, 0f, 1f);
            var newProgress = Mathf.RoundToInt(progress * _maxValue);
            base.SetProgress(progress, animate);
            UpdateElements(newProgress);
            _progress = newProgress;
        }

        public override void SetProgress(int count, bool animate = false)
        {
            count = Mathf.Clamp(count, 0, _maxValue);
            base.SetProgress(count, animate);
            UpdateElements(count);
            _progress = count;
        }

        private void UpdateElements(int progress)
        {
            for (int i = 0; i < _maxValue; i++)
            {
                var element = _elements[i];
                if (i < progress)
                {
                    if(element.IsShown == false)
                        element.Show();
                }
                else
                {
                    if(element.IsShown)
                        element.Hide();
                }
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                SetMaxValue(_maxValue);
                SetProgress(_progress);
            }
        }
#endif
    }
}