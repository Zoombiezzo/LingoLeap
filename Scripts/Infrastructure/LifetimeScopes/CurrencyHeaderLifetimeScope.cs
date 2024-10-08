using _Client.Scripts.GameLoop.Screens.CurrencyHeader;
using _Client.Scripts.GameLoop.Screens.Shop;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class CurrencyHeaderLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CurrencyHeaderPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}