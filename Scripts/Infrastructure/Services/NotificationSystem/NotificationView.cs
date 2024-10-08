using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public abstract class NotificationView : MonoBehaviour, INotificationView
    {
        public bool IsShowed => _isShowed;
        
        protected INotificationService _notificationService;
        protected bool _isShowed;

        public virtual void Show()
        {
            _isShowed = true;
        }

        public virtual void Hide()
        {
            
        }

        public virtual void RegisterData(INotificationData data)
        {
            
        }

        protected virtual void OnHided()
        {
            _isShowed = false;
            _notificationService.UpdateNotifications();
        }
        
        protected virtual void OnShowed()
        {
            
        }
        
        public void RegisterService(INotificationService service)
        {
            _notificationService = service;
        }
    }
}