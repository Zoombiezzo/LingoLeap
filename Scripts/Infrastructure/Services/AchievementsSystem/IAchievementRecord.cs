using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using Color = UnityEngine.Color;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    public interface IAchievementRecord
    {
        string Id { get; }
        int Stage { get; }
        int MaxStage { get; }
        string Description { get; }
        string Title { get; }
        string CompletedProgressText { get; }
        string IconId { get; }
        Color ColorBackground { get; } 
        float PreviousProgress { get; }
        float Progress { get; }
        string ProgressText { get; }
        bool IsCompleted { get; }
        IReadOnlyList<IRewardInfo> Rewards { get; }
        event Action<IAchievementRecord> OnProgressChanged;
        event Action<IAchievementRecord> OnStageChanged;
        void RecalculateProgress();
        void CalculateNextStepNotify();
        void SetStage(int stage);
        string GetDescription(ILocalizationService localizationService);
    }
}