using System;
using _Client.Scripts.GameLoop.Data.LevelProgress;
using _Client.Scripts.GameLoop.Screens.Achievements;
using _Client.Scripts.GameLoop.Screens.Map;
using _Client.Scripts.GameLoop.Screens.Settings;
using _Client.Scripts.GameLoop.Screens.Shop;
using _Client.Scripts.GameLoop.Screens.SpinWheel;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.SaveService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.StateMachine.States;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.MainMenu
{
    public class MainMenuPresenter : IStartable, IDisposable
    {
        private MainMenuWindow _mainMenuWindow;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ILevelProgressData _levelProgressData;
        private readonly ILocalizationService _localizationService;
        private readonly IStorageService _storageService;

        private IDisposable _disposable;

        public MainMenuPresenter(IGameStateMachine gameStateMachine,
            ILevelProgressData levelProgressData, ILocalizationService localizationService, IStorageService storageService)
        {
            _gameStateMachine = gameStateMachine;
            _levelProgressData = levelProgressData;
            _localizationService = localizationService;
            _storageService = storageService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _mainMenuWindow);

            var disposableBuilder = Disposable.CreateBuilder();
            
            _mainMenuWindow.ButtonPlayGame.OnClick.AsObservable().Subscribe(OnPlayGameClick).AddTo(ref disposableBuilder);
            _mainMenuWindow.ButtonSettings.OnClick.AsObservable().Subscribe(OnClickSettings).AddTo(ref disposableBuilder);
            _mainMenuWindow.ButtonAchievements.OnClick.AsObservable().Subscribe(OnClickAchievements).AddTo(ref disposableBuilder);
            _mainMenuWindow.ButtonShop.OnClick.AsObservable().Subscribe(OnClickShop).AddTo(ref disposableBuilder);
            _mainMenuWindow.CoinsButton.OnClick.AsObservable().Subscribe(OnClickShopCurrency).AddTo(ref disposableBuilder);
            _mainMenuWindow.ButtonSpinWheel.OnClick.AsObservable().Subscribe(OnClickSpinWheel).AddTo(ref disposableBuilder);
            _mainMenuWindow.ButtonMap.OnClick.AsObservable().Subscribe(OnClickMap).AddTo(ref disposableBuilder);
            _mainMenuWindow.ClearSaveButton.OnClickAsObservable().Subscribe(OnClearSave).AddTo(ref disposableBuilder);
            
            Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h, h => _localizationService.OnLanguageChanged -= h)
                .Subscribe(OnLocalizationChanged).AddTo(ref disposableBuilder);
            
            Observable.FromEvent(h => _levelProgressData.OnCurrentLevelChanged += h, h => _levelProgressData.OnCurrentLevelChanged -= h)
                .Subscribe(OnLevelChanged).AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
            
            InitializeFields();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
        
        private void OnClickMap(Unit _)
        {
            WindowsService.Show<MapWindow>();
        }
        
        private void OnClickShop(Unit _)
        {
            WindowsService.Show<ShopWindow>();
        }
        
        private void OnClickShopCurrency(Unit _)
        {
            WindowsService.TryGetWindow<ShopWindow>(out var shopWindow);
            shopWindow.SetTargetCategoryId("soft_currency_category");
            WindowsService.Show<ShopWindow>();
        }
        
        private void OnClickSpinWheel(Unit _)
        {
            WindowsService.Show<SpinWheelWindow>();
        }
        
        private void OnLocalizationChanged(string _)
        {
            InitializeFields();
        }
        
        private void OnLevelChanged(Unit _)
        {
            InitializeFields();
        }

        private void InitializeFields()
        {
            var levelNumber = _levelProgressData.GetCurrentLevel();
            _mainMenuWindow.LevelNumberText.text = levelNumber.ToString();
        }

        private void OnPlayGameClick(Unit _)
        {
            _gameStateMachine.Enter<LoadGameState>();
        }
        
        private void OnClickSettings(Unit _)
        {
            WindowsService.Show<SettingsWindow>();
        }
        
        private void OnClickAchievements(Unit _)
        {
            WindowsService.Show<AchievementsWindow>();
        }
        
        private void OnClearSave(Unit _)
        {
            _storageService.Clear();
        }
    }
}