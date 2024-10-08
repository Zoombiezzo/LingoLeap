namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public class NotificationElement : INotificationElement
    {
        public INotificationData Data { get; }
        public INotificationView View { get; }
        
        public NotificationElement(INotificationData data, INotificationView view)
        {
            Data = data;
            View = view;
        }
    }
}