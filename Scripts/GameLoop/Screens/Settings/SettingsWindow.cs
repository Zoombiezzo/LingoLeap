using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Settings
{
    public class SettingsWindow : Window
    {
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private AnimationButton _signInButton;
        [SerializeField] private AnimationButton _rateButton;
        [SerializeField] private GameObject _rateButtonGO;
        [SerializeField] private GameObject _signInButtonGO;
        [SerializeField] private Button _closePanel;
        [SerializeField] private ToggleAnimationButton _toggleMusic;
        [SerializeField] private ToggleAnimationButton _toggleSound;
        [SerializeField] private AnimationButton _languageButton;
        [SerializeField] private TMP_Text _versionText;
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;

        public AnimationButton CloseButton => _closeButton;
        public AnimationButton SignInButton => _signInButton;
        public AnimationButton RateButton => _rateButton;
        public GameObject RateButtonGO => _rateButtonGO;
        public GameObject SignInButtonGO => _signInButtonGO;
        public AnimationButton LanguageButton => _languageButton;
        public ToggleAnimationButton ToggleMusic => _toggleMusic;
        public ToggleAnimationButton ToggleSound => _toggleSound;
        public Button ClosePanel => _closePanel;
        public TMP_Text VersionText => _versionText;
        
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