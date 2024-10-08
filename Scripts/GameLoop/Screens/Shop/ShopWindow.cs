using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Shop
{
    public class ShopWindow : Window
    {
        [SerializeField] private ShopContainer _container;
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private ToggleComponent _toggleComponent;

        private string _targetCategoryId = string.Empty;

        public ShopContainer Container => _container;
        public AnimationButton CloseButton => _closeButton;
        public string TargetCategoryId => _targetCategoryId;

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
        
        public string SetTargetCategoryId(string categoryId)
        {
            _targetCategoryId = categoryId;
            return _targetCategoryId;
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