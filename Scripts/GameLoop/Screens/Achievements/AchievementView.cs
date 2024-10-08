using System;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Progressbar;
using _Client.Scripts.GameLoop.Components.Progressbar.StageProgressbar;
using _Client.Scripts.Infrastructure.Services.AchievementsSystem;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.Achievements
{
    public class AchievementView : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconBackground;
        [SerializeField] private Image _gradientBackground;
        [SerializeField] private CanvasGroup _buttonCollectActiveCanvas;
        [SerializeField] private CanvasGroup _completedIcon;
        [SerializeField] private CanvasGroup _panelReward;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private AnimationButton _buttonCollectReward;
        [SerializeField] private StageProgressbar _progressbarStage;
        [SerializeField] private UISliderProgressbarText _progressbarSlider;

        private IDisposable _disposable;
        private IAchievementRecord _record;
        private ILocalizationService _localizationService;
        private IAchievementService _achievementService;

        public event Action<AchievementView> OnCollectRewardClicked;
        
        public IAchievementRecord Record => _record;
        public RectTransform RectTransform => _rectTransform;
        
        public void Initialize(IAchievementRecord record, ILocalizationService localizationService, IAchievementService achievementService)
        {
            _record = record;
            _localizationService = localizationService;
            _achievementService = achievementService;

            InitializeParameters();
        }

        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }
        
        public void SetVisible(bool visible) => _canvasGroup.alpha = visible ? 1f : 0f;

        private void InitializeParameters()
        {
            _progressbarStage.Initialize(_record.MaxStage);
            _progressbarStage.SetProgress(_record.Stage);
            
            _progressbarSlider.SetProgress(_record.Progress);
            _progressbarSlider.SetProgressText(_record.ProgressText);
                
            _localizationService.RegisterTMPText(_record.Title, _titleText);
            
            _descriptionText.text = _record.GetDescription(_localizationService);

            _buttonCollectActiveCanvas.alpha = 0f;

            _iconBackground.color = _record.ColorBackground;
            _gradientBackground.color = _record.ColorBackground;
            
            ChangeState();
            
            _buttonCollectActiveCanvas.alpha = _achievementService.IsPossibleGetReward(_record) ? 1f : 0f;
            
            var builder = Disposable.CreateBuilder();

            _buttonCollectReward.OnClick.AsObservable().Subscribe(OnClickCollectReward).AddTo(ref builder);
            Observable.FromEvent<IAchievementRecord>(h => _record.OnProgressChanged += h,
                h => _record.OnProgressChanged -= h).Subscribe(OnProgressChanged).AddTo(ref builder);
            
            Observable.FromEvent<IAchievementRecord>(h => _record.OnStageChanged += h,
                h => _record.OnStageChanged -= h).Subscribe(OnStageChanged).AddTo(ref builder);
            
            Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h, h => _localizationService.OnLanguageChanged -= h)
                .Subscribe(OnLocalizationChanged).AddTo(ref builder);
            
            _disposable = builder.Build();
        }
        
        private void OnLocalizationChanged(string _)
        {
            _descriptionText.text = _record.GetDescription(_localizationService);
        }

        private void OnClickCollectReward(Unit _)
        {
            OnCollectRewardClicked?.Invoke(this);
        }

        private void ChangeState()
        {
            ChangeCanvasGroup(_panelReward, _record.IsCompleted == false);
            ChangeCanvasGroup(_completedIcon, _record.IsCompleted);

            if (_record.IsCompleted)
                _progressbarSlider.Hide();
            else
                _progressbarSlider.Show();
        }

        private void ChangeCanvasGroup(CanvasGroup canvasGroup, bool isShowed)
        {
            canvasGroup.alpha = isShowed ? 1f : 0f;
            canvasGroup.interactable = isShowed;
            canvasGroup.blocksRaycasts = isShowed;
        }
        
        private void OnProgressChanged(IAchievementRecord record)
        {
            _progressbarSlider.SetProgress(record.Progress);
            _progressbarSlider.SetProgressText(record.ProgressText);
            
            _buttonCollectActiveCanvas.alpha = _achievementService.IsPossibleGetReward(record) ? 1f : 0f;
            ChangeState();
        }
        
        private void OnStageChanged(IAchievementRecord record)
        {
            _progressbarStage.SetProgress(record.Stage);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            
            _localizationService.UnregisterTMPText(_record.Title, _titleText);
        }
    }
}