using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Achievements
{
    public class AchievementsWindow : Window
    {
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private Button _closePanel;
        [SerializeField] private AchievementNotificationView _notificationView;
        [SerializeField] private AchievementContainer _achievementContainer;
        [SerializeField] private TMP_Text _currentStarsText;
        [SerializeField] private TMP_Text _maxStarsText;
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;
        [SerializeField] private ComponentToggler _toggleComponent;

        public AnimationButton CloseButton => _closeButton;
        public Button ClosePanel => _closePanel;
        public TMP_Text CurrentStarsText => _currentStarsText;
        public TMP_Text MaxStarsText => _maxStarsText;
        public AchievementNotificationView NotificationView => _notificationView;
        public AchievementContainer AchievementContainer => _achievementContainer;
        
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
            
            _toggleComponent.Enable();
        }
        
        private void HideAnimation()
        {
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying)
                    anim.Stop();
            }
            
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