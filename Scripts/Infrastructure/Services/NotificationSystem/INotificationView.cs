namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public interface INotificationView
    {
        bool IsShowed { get; }
        void RegisterService(INotificationService service);
        void Show();
        void Hide();
        void RegisterData(INotificationData data);
    }
}