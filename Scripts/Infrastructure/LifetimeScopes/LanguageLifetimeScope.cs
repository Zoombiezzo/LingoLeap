using _Client.Scripts.GameLoop.Screens.LanguageSelect;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class LanguageLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LanguageSelectPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}