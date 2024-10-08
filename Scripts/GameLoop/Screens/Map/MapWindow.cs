using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Map
{
    public class MapWindow : Window
    {
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private AnimationButton _buttonNext;
        [SerializeField] private Image _buttonNextImage;
        [SerializeField] private AnimationButton _buttonPrevious;
        [SerializeField] private Image _buttonPreviousImage;
        [SerializeField] private ToggleComponent _toggleComponent;
        [SerializeField] private MapContainer _container;

        [SerializeField] [FoldoutGroup("RESOURCES")]
        private Sprite _arrowBase;
        [SerializeField] [FoldoutGroup("RESOURCES")]
        private Sprite _arrowElement;
        
        public AnimationButton CloseButton => _closeButton;
        public AnimationButton ButtonNext => _buttonNext;
        public Image ButtonNextImage => _buttonNextImage;
        public AnimationButton ButtonPrevious => _buttonPrevious;
        public Image ButtonPreviousImage => _buttonPreviousImage;
        public MapContainer Container => _container;
        
        public Sprite ArrowBase => _arrowBase;
        public Sprite ArrowElement => _arrowElement;

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
            await _toggleComponent.Enable();
        }
        
        private async void HideAnimation()
        {
            await _toggleComponent.Disable();
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