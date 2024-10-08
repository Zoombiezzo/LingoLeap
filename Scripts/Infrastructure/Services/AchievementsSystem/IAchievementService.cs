using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    public interface IAchievementService : IConfigData, IStorable
    {
        event Action<IAchievementRecord> OnAchievementStageChanged;
        public IReadOnlyCollection<IAchievementRecord> Achievements { get; }
        int MaxAllStages { get; }
        int CurrentStages { get; }
        IReadOnlyList<AchievementRuleRecord> GetRules<T>() where T : AchievementRuleRecord;
        IReadOnlyList<AchievementRuleRecord> GetRules(AchievementRecord record);
        void RulesChanged(IReadOnlyCollection<AchievementRuleRecord> rules);
        bool TryGetAchievement(AchievementRuleRecord rule, out IAchievementRecord record);
        void RegisterUpdater(IAchievementUpdater updater);
        void EnableUpdaters();
        void DisableUpdaters();
        bool IsPossibleGetReward(IAchievementRecord record);
        bool TryGetReward(IAchievementRecord record, out IRewardInfo reward);
    }
}