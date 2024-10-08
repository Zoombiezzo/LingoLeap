using System;

namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public interface INotificationService
    {
        event Action<INotificationElement> NotificationPushed;
        event Action NotificationUpdated;
        int CountNotificationsInQueue { get; }
        void RegisterNotificationView<T>(T view) where T : INotificationView;
        void UnregisterNotificationView<T>() where T : INotificationView;
        void RegisterNotificationViewInstance<T>(T view) where T : INotificationView;
        void UnregisterNotificationViewInstance<T>() where T : INotificationView;
        bool TryGetNotificationView<T>(Type type, out T outView) where T : INotificationView;
        bool TryGetNotificationViewInstance<T>(Type type, out T outView) where T : INotificationView;
        void PushNotification<T>(INotificationData data) where T : INotificationView;
        bool TryPopNotification(out INotificationElement element);
        bool TryPeekNotification(out INotificationElement element);
        void UpdateNotifications();
    }
}