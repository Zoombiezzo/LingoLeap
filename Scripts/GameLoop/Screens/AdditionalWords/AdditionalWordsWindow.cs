using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.AdditionalWords
{
    public class AdditionalWordsWindow : Window
    {
        [SerializeField] private AnimationButton _buttonExit;
        [SerializeField] private Button _panelExit;
        [SerializeField] private AdditionalWordsContainer _additionalWordsContainer;
        [SerializeField] private UIProgressbar _progressbar;
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;
        [SerializeField] private ComponentToggler _toggler;
        
        public AnimationButton ButtonExit => _buttonExit;
        public Button PanelExit => _panelExit;
        public UIProgressbar Progressbar => _progressbar;
        public AdditionalWordsContainer AdditionalWordsContainer => _additionalWordsContainer;

        protected override void OnBeforeShown()
        {
            base.OnBeforeShown();
            ShowAnimation();
        }
        
        protected override void OnHidden()
        {
            base.OnHidden();
            HideAnimation();
        }

        private async void ShowAnimation()
        {
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying == false)
                    anim.Play();
            }

            await _toggler.Enable();
        }
        
        private async void HideAnimation()
        {
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying)
                    anim.Stop();
            }
            
            await _toggler.Disable();
        }

#if UNITY_EDITOR
        [Button]
        private void ShowButton()
        {
            Show();
        }

        [Button]
        private void HideButton()
        {
            Hide();
        }
#endif
    }
}