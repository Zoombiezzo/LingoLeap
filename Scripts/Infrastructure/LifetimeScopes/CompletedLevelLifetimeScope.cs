using _Client.Scripts.GameLoop.Screens.CompletedLevelWindow;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class CompletedLevelLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CompletedLevelPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}