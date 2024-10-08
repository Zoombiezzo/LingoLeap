using _Client.Scripts.GameLoop.Screens.Map;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class MapLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MapPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}