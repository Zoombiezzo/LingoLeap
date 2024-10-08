using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Screens.Achievements;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.NotificationSystem;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    public class AchievementService : IAchievementService
    {
        private const string AssetPath = "Achievements";
        
        private readonly List<AchievementInfo> _infos = new(32);
        private readonly List<IAchievementUpdater> _updaters = new(32);
        private readonly Dictionary<string, AchievementInfo> _infoMap = new(32);
        private readonly Dictionary<string, AchievementRecord> _recordsMap = new(32);
        private readonly Dictionary<AchievementRecord, List<AchievementRuleRecord>> _rulesAchievementMap = new(32);
        private readonly Dictionary<Type, List<AchievementRuleRecord>> _rulesTypeMap = new(32);
        private readonly Dictionary<AchievementRuleRecord, AchievementRecord> _achievementsRulesMap = new(32);

        private readonly IAssetProvider _assetProvider;
        private readonly IStorageService _storageService;
        private readonly AchievementStorage _storage;
        private readonly ILocalizationService _localizationService;
        private readonly IRewardService _rewardService;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly INotificationService _notificationService;

        private int _maxAllStages;
        private int _currentStages;
        
        public event Action<IAchievementRecord> OnAchievementStageChanged;
        public IReadOnlyCollection<IAchievementRecord> Achievements => _storage.Achievements;

        public int MaxAllStages => _maxAllStages;
        public int CurrentStages => _currentStages;
        
        public AchievementService(IAssetProvider assetProvider, IStorageService storage, 
            ILocalizationService localizationService, IRewardService rewardService,
            ISpriteDatabaseService spriteDatabaseService,
            INotificationService notificationService)
        {
            _assetProvider = assetProvider;
            _storageService = storage;
            _localizationService = localizationService;
            _rewardService = rewardService;
            _spriteDatabaseService = spriteDatabaseService;
            _notificationService = notificationService;
            _storage = new AchievementStorage();
            _storageService.Register<IAchievementService>(new StorableData<IAchievementService>(this, _storage));
        }
        
        private void AddAchievementInfo(AchievementInfo info)
        {
            if (_infoMap.ContainsKey(info.Id))
            {
                Debug.LogWarning($"Achievement {info.Id} already exists");
                return;
            }
            
            _infos.Add(info);
            _infoMap.Add(info.Id, info);
        }
        
        private void AddAchievementRecord(AchievementRecord record)
        {
            _storage.Achievements.Add(record);
            RegisterAchievementRecord(record);
        }
        
        private void RegisterAchievementRecord(AchievementRecord record)
        {
            if (_recordsMap.TryAdd(record.Id, record) == false)
            {
                Debug.LogWarning($"Achievement {record.Id} already exists");
                return;
            }
            
            RegisterRules(record);
            record.RecalculateProgress();
        }

        private void RecalculateCurrentStages()
        {
            _currentStages = 0;
            foreach (var record in _storage.Achievements) 
                _currentStages += record.Stage;
        }

        private AchievementRecord CreateRecordFromInfo(AchievementInfo info) => info.CreateRecord();
        public IReadOnlyList<AchievementRuleRecord> GetRules<T>() where T : AchievementRuleRecord => _rulesTypeMap.GetValueOrDefault(typeof(T));

        public IReadOnlyList<AchievementRuleRecord> GetRules(AchievementRecord record) => _rulesAchievementMap.GetValueOrDefault(record);

        public void RulesChanged(IReadOnlyCollection<AchievementRuleRecord> rules)
        {
            foreach (var rule in rules)
            {
                if (_achievementsRulesMap.TryGetValue(rule, out var record) == false)
                    continue;
                
                var previousProgress = record.Progress;
                record.RecalculateProgress();

                if (previousProgress < 1f)
                {
                    TryNotify(record);
                }
            }

            _storageService.Save<IAchievementService>();
        }

        public bool TryGetAchievement(AchievementRuleRecord rule, out IAchievementRecord record)
        {
            record = null;

            if (_achievementsRulesMap.TryGetValue(rule, out var recordOut) == false) return false;
            
            record = recordOut;
            return true;
        }

        private void RegisterRules(AchievementRecord record)
        {
            var rules = new List<AchievementRuleRecord>();
            if(record.Rule == null)
                return;
            
            Stack<AchievementRuleRecord> stack = new Stack<AchievementRuleRecord>();
            stack.Push(record.Rule);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                currentNode.RegisterAchievement(record);
                
                if (currentNode.IsComposite == false)
                {
                    rules.Add(currentNode);
                    continue;
                }
                
                foreach (var node in currentNode.Children)
                {
                    stack.Push(node);
                }
            }

            foreach (var rule in rules)
            {
                RegisterRule(record, rule);
            }
        }

        private void TryNotify(AchievementRecord record)
        {
            if(record.Progress < record.NextStepNotify)
                return;
            
            var notificationData = new AchievementNotificationData
            {
                Icon = _spriteDatabaseService.GetSprite(record.IconId),
                Description = record.GetDescription(_localizationService),
                IconColor = record.ColorBackground,
                PreviousProgress = record.PreviousProgress,
                Progress = record.Progress,
                ProgressText = record.ProgressText
            };

            _notificationService.PushNotification<AchievementNotificationView>(notificationData);
            
            record.CalculateNextStepNotify();
        }

        private void RegisterRule(AchievementRecord record, AchievementRuleRecord rule)
        {
            if (_rulesAchievementMap.TryGetValue(record, out var rules) == false)
            {
                rules = new List<AchievementRuleRecord>();
                _rulesAchievementMap.Add(record, rules);
            }
            
            rules.Add(rule);

            var typeRule = rule.GetType();
            if (_rulesTypeMap.TryGetValue(typeRule, out var rulesType) == false)
            {
                rulesType = new List<AchievementRuleRecord>();
                _rulesTypeMap.Add(typeRule, rulesType);
            }
            
            rulesType.Add(rule);
            
            _achievementsRulesMap.Add(rule, record);
        }

        public async Task LoadData()
        {
            var assets = await _assetProvider.LoadAll<AchievementsCategory>(AssetPath);

            if(assets.Count == 0)
                return;

            foreach (var asset in assets)
            {
                foreach (var achievement in asset.Achievements)
                {
                    AddAchievementInfo(achievement);
                }
            }
        }
        
        private bool TryRegisterInfoToRecords(AchievementRecord record)
        {
            if (string.IsNullOrEmpty(record.Id))
                return false;
            
            if (_infoMap.TryGetValue(record.Id, out var info) == false)
                return false;

            record.RegisterInfo(info);
            
            if(record.Rule == null)
                return false;
            
            Stack<AchievementRuleRecord> stackRecords = new Stack<AchievementRuleRecord>();
            stackRecords.Push(record.Rule);
            
            Stack<AchievementRuleInfo> stackInfos = new Stack<AchievementRuleInfo>();
            stackInfos.Push(info.Rule);
            
            while (stackRecords.Count > 0)
            {
                var currentNodeRecord = stackRecords.Pop();
                var currentNodeInfo = stackInfos.Pop();
                
                var typeRecord = currentNodeRecord.GetType();
                var typeInfo = currentNodeInfo.TypeRecord;
                
                if (typeRecord != typeInfo)
                {
                    Debug.LogWarning($"Record {currentNodeRecord.Id} and Info {currentNodeInfo.Id} have different types");
                    return false;
                }
                
                if(currentNodeRecord.Id != currentNodeInfo.Id)
                {
                    Debug.LogWarning($"Record {currentNodeRecord.Id} and Info {currentNodeInfo.Id} have different ids");
                    return false;
                }
                
                currentNodeRecord.RegisterInfo(currentNodeInfo);
                currentNodeRecord.RegisterAchievement(record);
                
                if (currentNodeRecord.IsComposite == false)
                    continue;
                
                foreach (var node in currentNodeRecord.Children)
                {
                    stackRecords.Push(node);
                }
                
                foreach (var node in currentNodeInfo.Children)
                {
                    stackInfos.Push(node);
                }
            }
            
            return true;
        }

        public void Load(IStorage data)
        {
            var achievementsForDelete = new List<AchievementRecord>(16);
            
            foreach (var achievement in _storage.Achievements)
            {
                if(TryRegisterInfoToRecords(achievement) == false)
                {
                    achievementsForDelete.Add(achievement);
                    continue;
                }

                RegisterAchievementRecord(achievement);

                if (achievement.Stage >= achievement.MaxStage)
                {
                    if (achievement.Progress >= 1f)
                    {
                        achievement.SetStage(achievement.MaxStage);
                    }
                    else
                    {
                        achievement.SetStage(Mathf.Max(0, achievement.MaxStage - 1));
                    }
                    
                    achievement.RecalculateProgress();
                }
            }
            
            foreach (var achievement in achievementsForDelete)
            {
                _storage.Achievements.Remove(achievement);
            }

            foreach (var info in _infos)
            {
                if (_recordsMap.ContainsKey(info.Id))
                    continue;
                
                AddAchievementRecord(CreateRecordFromInfo(info));
            }

            var orderedRecords = new List<AchievementRecord>();

            foreach (var info in _infos)
            {
                if (_recordsMap.TryGetValue(info.Id, out var record) == false)
                    continue;
                
                orderedRecords.Add(record);
            }
            
            _storage.Achievements = orderedRecords;

            _maxAllStages = 0;
            _currentStages = 0;
            
            foreach (var achievement in _storage.Achievements)
            {
                _maxAllStages += achievement.MaxStage;
                _currentStages += achievement.Stage;
            }
            
            _storageService.Save<IAchievementService>();
        }

        public string ToStorage() => _storage.ToData(this);
        
        public void RegisterUpdater(IAchievementUpdater updater)
        {
            _updaters.Add(updater);
        }

        public void EnableUpdaters()
        {
            foreach (var updater in _updaters)
            {
                updater.Enable();
            }
        }

        public void DisableUpdaters()
        {
            foreach (var updater in _updaters)
            {
                updater.Disable();
            }
        }

        public bool IsPossibleGetReward(IAchievementRecord record)
        {
            if(record.Stage >= record.MaxStage)
                return false;
            
            return record.Progress >= 1f;
        }

        public bool TryGetReward(IAchievementRecord record, out IRewardInfo reward)
        {
            reward = null;
            
            var stage = record.Stage;
            if(stage >= record.MaxStage)
                return false;
            
            if(record.Progress < 1f)
                return false;
            
            var rewards = record.Rewards;
            if(stage >= rewards.Count)
                return false;
            
            reward = rewards[stage];
            
            if (_rewardService.TryCollectReward(reward) == false)
                return false;
            
            record.SetStage(record.Stage + 1);
            record.RecalculateProgress();
            record.CalculateNextStepNotify();

            RecalculateCurrentStages();
            
            OnAchievementStageChanged?.Invoke(record);
            
            _storageService.Save<IAchievementService>();
            return true;
        }
    }
}