using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.Tools.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private AnimationButton _button;
        [SerializeField] private UIProgressbar _progressbar;
        [SerializeField] private UiAnimation _pumpAnimation;
        
        public Button.ButtonClickedEvent OnClick
        {
            get => _button.OnClick;
            set => _button.OnClick = value;
        }
        
        public RectTransform RectTransform => _rectTransform;
        
        public void PlayPumpAnimation()
        {
            StopAnimations();
            _pumpAnimation?.Play();
        }

        public virtual void SetProgress(int count, int maxCount, bool animatie = false) => 
            _progressbar.SetProgress(count, maxCount, animatie);
        
        public virtual void SetProgress(float progress, bool animatie = false) => 
            _progressbar.SetProgress(progress, animatie);
        
        private void StopAnimations()
        {
            _pumpAnimation?.Stop();
        }
    }
}