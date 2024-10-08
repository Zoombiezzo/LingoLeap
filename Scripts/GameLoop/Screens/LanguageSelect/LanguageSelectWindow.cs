using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.LanguageSelect
{
    public class LanguageSelectWindow : Window
    {
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private Button _closePanel;
        [SerializeField] private LanguageSelector _languageSelector;
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;
        
        public AnimationButton CloseButton => _closeButton;
        public Button ClosePanel => _closePanel;
        public LanguageSelector LanguageSelector => _languageSelector;
        
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

        private void ShowAnimation()
        {
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying == false)
                    anim.Play();
            }   
        }
        
        private void HideAnimation()
        {
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying)
                    anim.Stop();
            }
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