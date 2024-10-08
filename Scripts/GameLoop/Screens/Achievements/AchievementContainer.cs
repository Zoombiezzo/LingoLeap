using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.GameLoop.Screens.LanguageSelect;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Client.Scripts.GameLoop.Screens.Achievements
{
    public class AchievementContainer : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private AchievementView _achievementView;

        private readonly List<AchievementView> _achievementViews = new(32);
        private readonly Dictionary<string, AchievementView> _achievementViewMap = new(32);
        
        private LanguageView _selectedLanguageView;

        private readonly List<IDisposable> _disposables = new();
        private readonly CompositeDisposable _compositeDisposable = new();
        
        private readonly Vector3[] _cornersViewport = new Vector3[4];
        private readonly Vector3[] _cornersElement = new Vector3[4];
        
        private ILocalizationService _localizationService;
        private ISpriteDatabaseService _spriteDatabaseService;
        private IAchievementService _achievementService;
        public event Action<AchievementView> OnTryCollectReward;

        [Inject]
        public void Construct(ILocalizationService localizationService, ISpriteDatabaseService spriteDatabaseService, IAchievementService achievementService)
        {
            _localizationService = localizationService;
            _spriteDatabaseService = spriteDatabaseService;
            _achievementService = achievementService;
        }
        
        public void CreateAchievementView(IAchievementRecord record)
        {
            var achievementView = Instantiate(_achievementView, _content);
            achievementView.Initialize(record, _localizationService, _achievementService);
            achievementView.SetIcon(_spriteDatabaseService.GetSprite(record.IconId));
            
            _achievementViews.Add(achievementView);
            _achievementViewMap.Add(record.Id, achievementView);

            Observable.FromEvent<AchievementView>(h => achievementView.OnCollectRewardClicked += h, h => achievementView.OnCollectRewardClicked -= h)
                .Subscribe(OnCollectRewardClicked).AddTo(_disposables);
        }

        public void SortAchievements()
        {
            _achievementViews.Sort(AchievementsComparer);
            
            for(int i = 0; i < _achievementViews.Count; i++)
            {
                var achievementView = _achievementViews[i];
                achievementView.transform.SetSiblingIndex(i);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
            DelayedUpdateVisibleElements();
        }
        
        private void OnCollectRewardClicked(AchievementView achievementView)
        {
            OnTryCollectReward?.Invoke(achievementView);
        }
        
        private void Clear()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            
            foreach (var achievementView in _achievementViews)
            {
                Destroy(achievementView);
            }
            
            _achievementViews.Clear();
            _achievementViewMap.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
        
        public static int AchievementsComparer(AchievementView x, AchievementView y)
        {
            var recordX = x.Record;
            var recordY = y.Record;
            
            if(recordX == null && recordY == null)
                return 0;

            if(recordX == null)
                return -1;

            if(recordY == null)
                return 1;
            
            var result = recordY.Progress.CompareTo(recordX.Progress);
            
            if(result != 0)
                return result;
            
            return recordY.MaxStage.CompareTo(recordX.MaxStage);
        }

        private void OnEnable()
        {
            _scrollRect.onValueChanged.AsObservable().Subscribe(OnScrollValueChanged).AddTo(_disposables);            
        }

        private void OnDisable()
        {
            _compositeDisposable?.Dispose();
        }

        private void OnScrollValueChanged(Vector2 position)
        {
            UpdateVisibleElements();
        }

        private async void DelayedUpdateVisibleElements()
        {
            await UniTask.NextFrame();
            UpdateVisibleElements();
        }

        private void UpdateVisibleElements()
        {
            foreach (var achievement in _achievementViews)
            {
                var result = IsRectTransformVisibleInScrollRect(achievement.RectTransform);
                achievement.SetVisible(result);
            }
        }
        
        private bool IsRectTransformVisibleInScrollRect(RectTransform target)
        {
            _viewport.GetWorldCorners(_cornersViewport);
            target.GetWorldCorners(_cornersElement);
            
            Rect viewportRect = new Rect(_cornersViewport[0], _cornersViewport[2] - _cornersViewport[0]);

            foreach (var corner in _cornersElement)
                if (viewportRect.Contains(corner))
                    return true;

            return false;
        }
    }
}