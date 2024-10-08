using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Achievements
{
    public class AchievementNotificationView : NotificationView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconBackground;
        [SerializeField] private Image _gradientBackground;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private UISliderProgressbarText _progressbarSlider;
        [SerializeField] private UiAnimation _showAnimation;
        [SerializeField] private UiAnimation _hideAnimation;
        [SerializeField] private float _duration;
        
        private AchievementNotificationData _data;
        private Sequence _sequence;
        
        public override void Show()
        {
            base.Show();
            _showAnimation.Play(OnShowed);
        }

        protected override void OnShowed()
        {
            base.OnShowed();
            _progressbarSlider.SetProgress(_data.Progress, true);

            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Kill();
            }
            
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(_duration);
            _sequence.AppendCallback(Hide);
        }

        public override void Hide()
        {
            base.Hide();
            _hideAnimation.Play().OnComplete(OnHided);
        }

        public override void RegisterData(INotificationData data)
        {
            base.RegisterData(data);
            
            if (data is AchievementNotificationData achievementData)
            {
                _icon.sprite = achievementData.Icon;
                _iconBackground.color = achievementData.IconColor;
                _gradientBackground.color = achievementData.IconColor;
                _descriptionText.text = achievementData.Description;
                _progressbarSlider.SetProgressText(achievementData.ProgressText);
                _progressbarSlider.SetProgress(achievementData.PreviousProgress);

                _data = achievementData;
            }
        }
    }
}