using _Client.Scripts.GameLoop.Screens.SpinWheel;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class SpinWheelLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SpinWheelPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}