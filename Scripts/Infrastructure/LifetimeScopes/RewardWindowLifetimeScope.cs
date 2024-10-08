using _Client.Scripts.GameLoop.Screens.Reward;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class RewardWindowLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RewardPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}