using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    public class AchievementNotificationData : INotificationData
    {
        public Sprite Icon { get; set; }
        public Color IconColor { get; set; }
        public string Description { get; set; }
        public float PreviousProgress { get; set; }
        public float Progress { get; set; }
        public string ProgressText { get; set; }
    }
}