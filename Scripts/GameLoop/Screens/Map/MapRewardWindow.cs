using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Common;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Map
{
    public class MapRewardWindow : Window
    {
        [SerializeField] private Image _icon;
        [SerializeField] private ParticleImage _particles;
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private AnimationButton _applyButton;
        [SerializeField] private AnimationElement _animationElement;
        [SerializeField] private float _duration;
        [SerializeField] private ComponentToggler _toggleComponent;
        
        public Image Icon => _icon;
        public ParticleImage Particles => _particles;
        public AnimationButton CloseButton => _closeButton;
        public AnimationButton ApplyButton => _applyButton;
        public float Duration => _duration;
        public AnimationElement AnimationElement => _animationElement;
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