using System;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Notifications
{
    public class NotificationsPresenter : IStartable, IDisposable
    {
        private NotificationsWindow _window;
        private readonly INotificationService _notificationService;
        private IDisposable _disposable;
        
        private NotificationView _currentNotificationView;

        public NotificationsPresenter(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            
            var builder = Disposable.CreateBuilder();

            Observable.FromEvent<INotificationElement>(h => _notificationService.NotificationPushed += h,
                h => _notificationService.NotificationPushed -= h).Subscribe(OnNotificationPushed).AddTo(ref builder);
             Observable.FromEvent(h => _notificationService.NotificationUpdated += h,
                h => _notificationService.NotificationUpdated -= h).Subscribe(OnNotificationUpdated).AddTo(ref builder);
            
            _disposable = builder.Build();
        }
        

        private void OnNotificationPushed(INotificationElement notification)
        {
            TryShowNotification();
        }
        
        private void OnNotificationUpdated(Unit _)
        {
            TryShowNotification();
        }
        
        private void TryShowNotification()
        {
            if(_currentNotificationView != null && _currentNotificationView.IsShowed)
                return;
            
            if (_notificationService.TryPeekNotification(out var notification) == false)
                return;

            NotificationView view = null;
            var type = notification.View.GetType();
            if (_notificationService.TryGetNotificationViewInstance(type, out view) == false)
            {
                if (_notificationService.TryGetNotificationView(type, out view))
                {
                    view = CreateNotificationView(view);
                }
            }

            if (view == null)
            {
                Debug.LogWarning($"[NotificationsPresenter]: View {type} not found");
                return;
            }
            
            if(view.IsShowed)
                return;

            view.RegisterData(notification.Data);
            view.Show();
            
            _notificationService.TryPopNotification(out _);
            
            _currentNotificationView = view;
        }

        private NotificationView CreateNotificationView(NotificationView notificationView)
        {
            var view = _window.CreateNotification(notificationView);
            view.RegisterService(_notificationService);
            _notificationService.RegisterNotificationViewInstance(view);
            return view;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}