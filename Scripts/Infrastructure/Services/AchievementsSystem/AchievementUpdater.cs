namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    public abstract class AchievementUpdater : IAchievementUpdater
    {
        public abstract void Enable();
        public abstract void Disable();
    }
}