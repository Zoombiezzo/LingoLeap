using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.NotificationSystem
{
    public class NotificationService : INotificationService
    {
        private Dictionary<Type, INotificationView> _views = new(8);
        private Dictionary<Type, INotificationView> _viewsInstance = new(8);
        
        private Queue<INotificationElement> _notificationQueue = new Queue<INotificationElement>(4);

        public event Action<INotificationElement> NotificationPushed;
        public event Action NotificationUpdated;

        public int CountNotificationsInQueue => _notificationQueue.Count;

        public void RegisterNotificationView<T>(T view) where T : INotificationView
        {
            var type = typeof(T);
            if (_views.TryAdd(typeof(T), view) == false)
            {
                Debug.LogWarning($"[NotificationService]: View {type} already exists");
                return;
            }
        }
        
        public void UnregisterNotificationView<T>() where T : INotificationView
        {
            var type = typeof(T);
            if (_views.Remove(type) == false)
            {
                Debug.LogWarning($"[NotificationService]: View {type} not found");
                return;
            }
        }
        
        public void RegisterNotificationViewInstance<T>(T view) where T : INotificationView
        {
            var type = view.GetType();
            if (_viewsInstance.TryAdd(type, view) == false)
            {
                Debug.LogWarning($"[NotificationService]: View instance {type} already exists");
                return;
            }
        }
        
        public void UnregisterNotificationViewInstance<T>() where T : INotificationView
        {
            var type = typeof(T);
            if (_viewsInstance.Remove(type) == false)
            {
                Debug.LogWarning($"[NotificationService]: View instance {type} not found");
                return;
            }
        }
        
        public bool TryGetNotificationView<T>(Type type, out T outView) where T : INotificationView
        {
            outView = default;
            
            if (_views.TryGetValue(type, out var view))
            {
                outView = (T)view;
                return true;
            }
            
            Debug.LogWarning($"[NotificationService]: View {typeof(T)} not found");
            return false;
        }
        
        public bool TryGetNotificationViewInstance<T>(Type type, out T outView) where T : INotificationView
        {
            outView = default;
            
            if (_viewsInstance.TryGetValue(type, out var view))
            {
                outView = (T)view;
                return true;
            }
            
            Debug.LogWarning($"[NotificationService]: View instance {typeof(T)} not found");
            return false;
        }
        
        public void PushNotification<T>(INotificationData data) where T : INotificationView
        {
            if (TryGetNotificationView<T>(typeof(T), out var view) == false)
                return;
            
            var element = new NotificationElement(data, view);
            _notificationQueue.Enqueue(element);
            
            NotificationPushed?.Invoke(element);
        }

        public bool TryPopNotification(out INotificationElement element)
        {
            if(_notificationQueue.Count == 0)
            {
                element = default;
                return false;
            }
            
            element = _notificationQueue.Dequeue();
            return true;
        }

        public bool TryPeekNotification(out INotificationElement element)
        {
            if(_notificationQueue.Count == 0)
            {
                element = default;
                return false;
            }
            
            element = _notificationQueue.Peek();
            return true;
        }

        public void UpdateNotifications()
        {
            NotificationUpdated?.Invoke();
        }
    }
}