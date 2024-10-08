using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Common;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Reward
{
    public class RewardWindow : Window
    {
        [SerializeField] private RewardView _rewardPrefab;
        [SerializeField] private RectTransform _rewardsContainer;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AnimationButton _multiplierButton;
        [SerializeField] private TMP_Text _multiplierText;
        [SerializeField] private AnimationElement _animationElement;
        [SerializeField] private float _duration;
        [SerializeField] private ComponentToggler _toggleComponent;
        
        public RewardView RewardPrefab => _rewardPrefab;
        public RectTransform RewardsContainer => _rewardsContainer;
        public Button CloseButton => _closeButton;
        public AnimationButton MultiplierButton => _multiplierButton;
        public TMP_Text MultiplierText => _multiplierText;
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