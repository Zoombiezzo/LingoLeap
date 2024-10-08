using _Client.Scripts.GameLoop.Screens.MainMenu;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuPresenter>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<MainMenuCurrencyPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}
