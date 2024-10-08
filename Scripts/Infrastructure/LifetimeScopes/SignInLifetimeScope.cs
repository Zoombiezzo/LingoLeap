using _Client.Scripts.GameLoop.Screens.Settings;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class SignInLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SignInPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}