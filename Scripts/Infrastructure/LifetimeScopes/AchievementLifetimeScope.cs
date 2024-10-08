using _Client.Scripts.GameLoop.Screens.Achievements;
using VContainer;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.LifetimeScopes
{
    public class AchievementLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<AchievementPresenter>(Lifetime.Scoped).AsSelf();
        }
    }
}