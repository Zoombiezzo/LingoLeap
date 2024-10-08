using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [Serializable]
    public class AchievementRecord : IAchievementRecord
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _stage = 0;
        [JsonProperty] private AchievementRuleRecord _rule;

        [JsonIgnore] public string Id => _id;
        [JsonIgnore] public int Stage => _stage;
        [JsonIgnore] public int MaxStage => _info.MaxStage;
        [JsonIgnore] public float NotificationStep => _info.NotificationStep;
        [JsonIgnore] public string Description => _info.Description;
        [JsonIgnore] public string Title => _info.Title;
        [JsonIgnore] public string CompletedProgressText => _info.CompletedProgressText;
        [JsonIgnore] public string IconId => _info.IconId;
        [JsonIgnore] public Color ColorBackground => _info.Color;
        [JsonIgnore] public bool IsCompleted => _stage >= _info.MaxStage;
        [JsonIgnore] public IReadOnlyList<IRewardInfo> Rewards => _info.Rewards;
        [JsonIgnore] public float Progress => _progress;
        [JsonIgnore] public float PreviousProgress => CalculatePreviousProgress();

        [JsonIgnore] public float NextStepNotify => _nextStepNotify;

        [JsonIgnore] public string ProgressText => _progressText;

        [JsonIgnore] public AchievementRuleRecord Rule => _rule;

        public event Action<IAchievementRecord> OnProgressChanged;

        public event Action<IAchievementRecord> OnStageChanged;

        [JsonIgnore] private AchievementInfo _info;

        [JsonIgnore] private float _progress;

        [JsonIgnore] private string _progressText;

        [JsonIgnore] private float _nextStepNotify;

        public AchievementRecord()
        {
            
        }

        public AchievementRecord(string id, AchievementInfo info)
        {
            _id = id;
            _info = info;
            _rule = _info.Rule.CreateRecord();
            CalculateNextStepNotify();
        }

        public void RegisterInfo(AchievementInfo info)
        {
            _info = info;
            CalculateNextStepNotify();
        }

        public void RecalculateProgress()
        {
            _progress = _rule.Progress;
            _progressText = _rule.ProgressText;

            Debug.Log($"[AchievementRecord] Progress for {_id}: {_progress} [{_progressText}]");
            
            OnProgressChanged?.Invoke(this);
        }

        public void SetStage(int stage)
        {
            stage = Mathf.Clamp(stage, 0, _info.MaxStage);
            
            _stage = stage;
            
            OnStageChanged?.Invoke(this);
        }

        public void CalculateNextStepNotify()
        {
            var step = MathF.Max(_info.NotificationStep, float.Epsilon);
            _nextStepNotify = MathF.Min((int)(_progress / step) * step + step, 1f);
        }

        public string GetDescription(ILocalizationService localizationService) =>
            _info.GetDescription(localizationService);

        private float CalculatePreviousProgress()
        {
            var step = MathF.Max(_info.NotificationStep, float.Epsilon);
            return MathF.Min((int)(_progress / step) * step - step, 1f);
        }
    }
}