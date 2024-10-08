using System.Collections.Generic;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.MainMenu
{
    public class MainMenuWindow : Window
    {
        [SerializeField] private AnimationButton _buttonPlayGame;
        [SerializeField] private AnimationButton _buttonSettings;
        [SerializeField] private AnimationButton _buttonAchievements;
        [SerializeField] private AnimationButton _buttonShop;
        [SerializeField] private AnimationButton _buttonSpinWheel;
        [SerializeField] private AnimationButton _buttonMap;
        [SerializeField] private AnimationButton _coinsButton;
        [SerializeField] private Button _clearSaveButton;
        [SerializeField] private TMP_Text _levelNumberText;
        [SerializeField] private CounterField _mindScoreCounterField;
        [SerializeField] private CounterField _softCounterField;
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;
        [SerializeField] private ToggleComponent _rootToggler;

        public AnimationButton ButtonPlayGame => _buttonPlayGame;
        public AnimationButton ButtonSettings => _buttonSettings;
        public AnimationButton ButtonAchievements => _buttonAchievements;
        public AnimationButton ButtonShop => _buttonShop;
        public AnimationButton ButtonSpinWheel => _buttonSpinWheel;
        public AnimationButton ButtonMap => _buttonMap;
        public AnimationButton CoinsButton => _coinsButton;
        public Button ClearSaveButton => _clearSaveButton;
        public TMP_Text LevelNumberText => _levelNumberText;
        public CounterField MindScoreCounterField => _mindScoreCounterField;
        public CounterField SoftCounterField => _softCounterField;
        
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
            _rootToggler.Enable();
            
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying == false)
                    anim.Play();
            }   
        }
        
        private void HideAnimation()
        {
            _rootToggler.Disable();

            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying)
                    anim.Stop();
            }
        }
    }
}