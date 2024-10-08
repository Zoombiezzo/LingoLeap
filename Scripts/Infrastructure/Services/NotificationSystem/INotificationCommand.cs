namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public interface INotificationElement
    {
        INotificationData Data { get; }
        INotificationView View { get; }
    }
}