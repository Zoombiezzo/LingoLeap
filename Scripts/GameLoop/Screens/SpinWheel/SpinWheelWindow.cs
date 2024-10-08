using System;
using System.Text;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class SpinWheelWindow : Window
    {
        [SerializeField] private SpinWheelView _view;
        [SerializeField] private AnimationButton _spinButton;
        [SerializeField] private AnimationButton _closeButton;
        [SerializeField] private ParticleImage _buttonSpinParticle;
        
        [SerializeField] private TMP_Text _textCurrencyValue;
        [SerializeField] private TMP_Text _textSpinsValue;
        [SerializeField] private Image _imageCurrency;
        
        [SerializeField] private TMP_Text _textTimeLeft;

        [SerializeField] private CanvasGroup _canvasGroupButtonEnabled;
        [SerializeField] private CanvasGroup _canvasGroupButtonDisabled;
        
        [SerializeField] private ToggleComponent _rootToggler;
        
        [SerializeField] [FoldoutGroup("AUDIO")]
        private AudioSelector _spinStartSound;
        [SerializeField] [FoldoutGroup("AUDIO")]
        private AudioSelector _spinRewardSound;

        private StringBuilder _timerUpdater = new(16);
        
        public SpinWheelView View => _view;
        public AnimationButton SpinButton => _spinButton;
        public AnimationButton CloseButton => _closeButton;
        
        public AudioSelector SpinStartSound => _spinStartSound;
        public AudioSelector SpinRewardSound => _spinRewardSound;

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
        }
        
        private void HideAnimation()
        {
            _rootToggler.Disable();
        }

        public void EnableSpinButton(bool enable)
        {
            if (enable)
            {
                _canvasGroupButtonEnabled.alpha = 1;
                _canvasGroupButtonDisabled.alpha = 0;
                _textSpinsValue.gameObject.SetActive(true);
                _imageCurrency.gameObject.SetActive(true);
                _buttonSpinParticle.Play();
            }
            else
            {
                _canvasGroupButtonEnabled.alpha = 0;
                _canvasGroupButtonDisabled.alpha = 1;
                _textSpinsValue.gameObject.SetActive(false);
                _imageCurrency.gameObject.SetActive(false);
                _buttonSpinParticle.Stop();
            }
        }

        public void SetCurrencySpin(Sprite sprite, string text)
        {
            _textCurrencyValue.text = text;
            _imageCurrency.sprite = sprite;
            _imageCurrency.gameObject.SetActive(sprite != null);
        }
        
        public void SetSpinLeft(int spinLeft, int maxSpins)
        {
            _textSpinsValue.text = $"({spinLeft}/{maxSpins})";
        }
        
        public void SetLeftTime(TimeSpan time)
        {
            _timerUpdater.Clear();
            
            if (time.TotalHours > 23)
            {
                _timerUpdater.AppendFormat("{0}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
                _textTimeLeft.SetText(_timerUpdater);
                return;
            }
            
            _timerUpdater.AppendFormat("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
            _textTimeLeft.SetText(_timerUpdater);
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