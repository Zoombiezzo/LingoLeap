using _Client.Scripts.GameLoop.Screens.Settings;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class SettingsLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SettingsPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}