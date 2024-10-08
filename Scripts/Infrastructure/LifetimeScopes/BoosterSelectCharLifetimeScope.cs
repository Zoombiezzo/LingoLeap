using _Client.Scripts.GameLoop.Screens.BoosterSelectChar;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class BoosterSelectCharLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BoosterSelectCharPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}