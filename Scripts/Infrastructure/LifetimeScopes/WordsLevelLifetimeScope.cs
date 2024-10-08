using _Client.Scripts.GameLoop.Screens.WordsLevel;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class WordsLevelLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<WordsLevelPresenter>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WordsLevelAdditionalWordsPresenter>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<WordsLevelCurrencyPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}
