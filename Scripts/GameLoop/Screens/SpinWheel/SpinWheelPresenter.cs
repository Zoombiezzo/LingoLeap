using System;
using _Client.Scripts.GameLoop.Screens.PendingScreen;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.Services.BankService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpinWheelService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using DG.Tweening;
using R3;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class SpinWheelPresenter : IStartable, IDisposable
    {
        private const string NotAvailableKey = "UNAVAILABLE";
        private const string FreeKey = "FREE";
        private const string SpinKey = "SPIN";
        
        private SpinWheelWindow _spinWheelWindow;
        private SpinWheelHandle _handle;

        private IDisposable _disposable;
        private readonly ISpinWheelService _spinWheelService;
        private readonly ISpriteDatabaseService _spriteDatabaseService;
        private readonly IRewardService _rewardService;
        private readonly ILocalizationService _localizationService;

        private bool _isSpin = false;
        private IDisposable _updateWheelHandle;

        public SpinWheelPresenter(ISpinWheelService spinWheelService, ISpriteDatabaseService spriteDatabaseService, IRewardService rewardService, ILocalizationService localizationService)
        {
            _spinWheelService = spinWheelService;
            _spriteDatabaseService = spriteDatabaseService;
            _rewardService = rewardService;
            _localizationService = localizationService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _spinWheelWindow);
            _handle = _spinWheelWindow.View.Handle;
            
            var builder = Disposable.CreateBuilder();
            
            _spinWheelWindow.SpinButton.OnClick.AsObservable().Where(IsNotSpin).Subscribe(OnSpinClick).AddTo(ref builder);
            _spinWheelWindow.CloseButton.OnClick.AsObservable().Where(IsNotSpin).Subscribe(OnCloseClick).AddTo(ref builder);

            Observable.FromEvent<string>(h => _localizationService.OnLanguageChanged += h,
                h => _localizationService.OnLanguageChanged -= h).Subscribe(OnLocalizationChanged).AddTo(ref builder);
            
            Observable.Interval(TimeSpan.FromSeconds(1f), UnityTimeProvider.UpdateIgnoreTimeScale).Subscribe(OnUpdateWheelHandle).AddTo(ref builder);
            
            Observable.FromEvent(h => _spinWheelWindow.OnBeforeShow += h, h => _spinWheelWindow.OnBeforeShow -= h).Subscribe(OnBeforeShow).AddTo(ref builder);

            _disposable = builder.Build();

            InitializeItems();
            InitializeFields();
        }
        
        private void OnCloseClick(Unit _)
        {
            _spinWheelWindow.Hide();
        }
        
        private void OnBeforeShow(Unit _)
        {
            InitializeFields();
        }
        
        private void OnUpdateWheelHandle(Unit _)
        {
            if (_spinWheelService.CanUpdate())
            {
                _spinWheelService.UpdateSpins();
                InitializeItems();
                InitializeFields();
                return;
            }
            
            if(_spinWheelWindow.IsShow() == false)
                return;
            
            UpdateTimeLeft();
        }
        
        private void OnLocalizationChanged(string language)
        {
            UpdateCurrencyValue();
        }

        private void InitializeItems()
        {
            var currentItems = _spinWheelService.CurrentSpinItems;
            var items = _spinWheelWindow.View.Items;
            
            for(var i = 0; i < currentItems.Count; i++)
            {
                if (items.Count > i)
                {
                    InitializeItem(items[i], currentItems[i]);
                }
            }
        }

        private void InitializeFields()
        {
            UpdateCurrencyValue();
            UpdateTimeLeft();
            UpdateSpinLeft();
        }

        private void UpdateCurrencyValue()
        {
            if (_spinWheelService.IsPossibleSpin() == false)
            {
                _spinWheelWindow.EnableSpinButton(false);
                _spinWheelWindow.SetCurrencySpin(null, _localizationService.GetValue(NotAvailableKey));
                return;
            }
            
            if (_spinWheelService.TryGetCurrentSpinSetting(out var spinSetting) == false)
            {
                return;
            }

            _spinWheelWindow.EnableSpinButton(true);

            var currency = spinSetting.Currency;

            if (currency == CurrencyType.Free)
            {
                _spinWheelWindow.SetCurrencySpin(null, _localizationService.GetValue(SpinKey));
                return;
            }
            
            var sprite = _spriteDatabaseService.GetCurrencySprite(currency);

            
            if (currency == CurrencyType.Ads)
            {
                _spinWheelWindow.SetCurrencySpin(sprite, _localizationService.GetValue(SpinKey));
                return;
            }
            
            _spinWheelWindow.SetCurrencySpin(sprite, spinSetting.Price.ToString());
        }

        private void UpdateSpinLeft()
        {
            if (_spinWheelService.IsPossibleSpin() == false)
            {
                _spinWheelWindow.EnableSpinButton(false);
                _spinWheelWindow.SetCurrencySpin(null, _localizationService.GetValue(NotAvailableKey));
                return;
            }
            _spinWheelWindow.SetSpinLeft(_spinWheelService.GetSpinLeft(), _spinWheelService.GetMaxSpins());
        }

        private void UpdateTimeLeft()
        {
            _spinWheelWindow.SetLeftTime(_spinWheelService.GetTimeLeftToUpdate());
        }

        private void InitializeItem(SpinWheelItemView itemView, ISpinWheelItem spinWheelItemInfo)
        {
            var sprite = _spriteDatabaseService.GetSprite(spinWheelItemInfo.SpriteId);
            itemView.Image.sprite = sprite;
            itemView.ImageShadow.sprite = sprite;

            var text = spinWheelItemInfo.Text;
            itemView.Text.text = text;
            itemView.TextCanvasGroup.alpha = string.IsNullOrEmpty(text) ? 0f : 1f;
        }

        private void OnSpinClick(Unit _)
        {
            OnBuyItemHandler();
        }

        private bool IsNotSpin(Unit _) => _isSpin == false;

        private void OnBuyItemHandler()
        {
            if(_spinWheelService.IsPossibleSpin() == false)
                return;
            
            WindowsService.Show<PendingWindow>();

            _spinWheelService.Purchase(OnPurchaseHandler);
        }

        private void OnPurchaseHandler(bool isSuccess)
        {
            WindowsService.Hide<PendingWindow>();

            if (isSuccess == false)
                return;

            Spin();
        }

        private async void Spin()
        {
            var targetItemIndex = _spinWheelService.GetRandomIndex();
            
            if (_spinWheelService.TryGetReward(targetItemIndex, out var reward) == false)
                return;
         
            _isSpin = true;
            _updateWheelHandle = Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate).Subscribe(UpdateWheelHandle);
            
            var itemView = _spinWheelWindow.View.Items[targetItemIndex];
            var spinWheelItemInfo = _spinWheelService.CurrentSpinItems[targetItemIndex];

            AudioService.Play(AudioType.Sound, _spinWheelWindow.SpinStartSound.Audio, AudioSettings.Default);
            await _spinWheelWindow.View.Spin(targetItemIndex).AsyncWaitForCompletion();
            
            AudioService.Play(AudioType.Sound, _spinWheelWindow.SpinRewardSound.Audio, AudioSettings.Default);
            await itemView.PlayCollectAnimation().AsyncWaitForCompletion();
            
            InitializeItem(itemView, spinWheelItemInfo);
            
            await itemView.PlayShowAnimation().AsyncWaitForCompletion();

            _rewardService.SetAvailableMultipleReward(2);
            _rewardService.ShowScreenReward(reward);
            
            InitializeFields();
            
            _updateWheelHandle?.Dispose();
            _isSpin = false;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void UpdateWheelHandle(Unit _)
        {
            _handle.UpdatePosition();
        }
    }
}