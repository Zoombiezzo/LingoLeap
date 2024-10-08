using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.CurrencyHeader
{
    public class CurrencyHeaderWindow : Window
    {
        [SerializeField] private CounterField _softCounterField;
        [SerializeField] private AnimationButton _softButton;
        [SerializeField] private ToggleComponent _toggleComponent;
        
        public CounterField SoftCounterField => _softCounterField;
        public AnimationButton SoftButton => _softButton;
        
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
        private void ShowScreen()
        {
            Show();
        }

        [Button]
        private void HideScreen()
        {
            Hide();
        }
#endif
    }
}