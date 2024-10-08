using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.CompletedLevelWindow
{
    public class CompletedLevelWindow : Window
    {
        [SerializeField] private AnimationButton _buttonHome;
        [SerializeField] private AnimationButton _buttonNextLevel;
        [SerializeField] private CounterField _mindScoreCounterField;
        [SerializeField] private CounterField _softCounterField;
        [SerializeField] private TMP_Text _completedLevelText;
        [SerializeField] private TMP_Text _nextLevelText;
        [SerializeField] private UIProgressbar _mapProgressbar;
        [SerializeField] private ToggleComponent _toggleComponent;

        public AnimationButton ButtonHome => _buttonHome;
        public AnimationButton ButtonNextLevel => _buttonNextLevel;
        public CounterField MindScoreCounterField => _mindScoreCounterField;
        public CounterField SoftCounterField => _softCounterField;
        public TMP_Text CompletedLevelText => _completedLevelText;
        public TMP_Text NextLevelText => _nextLevelText;
        public UIProgressbar MapProgressbar => _mapProgressbar;
        
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