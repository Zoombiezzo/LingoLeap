using _Client.Scripts.GameLoop.Screens.MainMenu;
using _Client.Scripts.GameLoop.Screens.Notifications;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class NotificationsLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<NotificationsPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}