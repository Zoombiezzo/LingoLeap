using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.SignIn
{
    public class SignInWindow : Window
    {
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private AnimationButton _signInButton;
        [SerializeField] private AnimationButton _rateButton;
        [SerializeField] private Button _closePanel;
        [SerializeField] private ToggleComponent _toggleComponent;

        public AnimationButton CloseButton => _closeButton;
        public AnimationButton SignInButton => _signInButton;
        public AnimationButton RateButton => _rateButton;
        public Button ClosePanel => _closePanel;
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
            _toggleComponent.Enable();
        }
        
        private void HideAnimation()
        {
            _toggleComponent.Disable();
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