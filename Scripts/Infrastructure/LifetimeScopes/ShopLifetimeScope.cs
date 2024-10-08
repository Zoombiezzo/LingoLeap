using _Client.Scripts.GameLoop.Screens.Shop;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class ShopLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ShopPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}