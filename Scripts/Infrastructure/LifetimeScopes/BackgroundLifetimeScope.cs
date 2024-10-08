using _Client.Scripts.GameLoop.Screens.Background;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class BackgroundLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BackgroundPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}