using _Client.Scripts.GameLoop.Screens.Map;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class MapRewardWindowLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MapRewardPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}