using _Client.Scripts.GameLoop.Screens.AdditionalWords;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class AdditionalWordsLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<AdditionalWordsPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}