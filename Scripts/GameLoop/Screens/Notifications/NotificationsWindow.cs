using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Notifications
{
    public class NotificationsWindow : Window
    {
        [SerializeField] private RectTransform _container;
        
        public RectTransform Container => _container;
        
        public NotificationView CreateNotification(NotificationView notificationView)
        {
            var notification = Instantiate(notificationView, _container);
            return notification;
        }
    }
}