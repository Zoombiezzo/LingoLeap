using _Client.Scripts.GameLoop.Screens.BoosterSelectChar;
using _Client.Scripts.GameLoop.Screens.BoosterSelectWord;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class BoosterSelectWordLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BoosterSelectWordPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}