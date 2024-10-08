using System;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.GameLoop.Screens.Shop;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.CurrencyHeader
{
    public class CurrencyHeaderPresenter : IStartable, IDisposable
    {
        private CurrencyHeaderWindow _window;
        private ShopWindow _shopWindow;
        
        private IDisposable _disposable;
        private readonly IPlayerProgressData _playerProgressData;

        public CurrencyHeaderPresenter(IPlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            WindowsService.TryGetWindow(out _shopWindow);
            
            var builder = Disposable.CreateBuilder();
            
            _window.SoftCounterField.SetValue(_playerProgressData.Soft.CurrentValue);
            
            _window.SoftButton.OnClick.AsObservable().Subscribe(OnClickShop).AddTo(ref builder);
            _playerProgressData.Soft.Subscribe(OnSoftChanged).AddTo(ref builder);

            Observable.FromEvent(h => _shopWindow.OnBeforeShow += h, h => _shopWindow.OnBeforeShow -= h)
                .Subscribe(OnShopBeforeShow).AddTo(ref builder);
            
            Observable.FromEvent(h => _shopWindow.OnBeforeHide += h, h => _shopWindow.OnBeforeHide -= h)
                .Subscribe(OnShopBeforeHide).AddTo(ref builder);
            
            _disposable = builder.Build();
        }
        
        private void OnClickShop(Unit _)
        {
            WindowsService.Show<ShopWindow>();
        }

        private void OnShopBeforeShow(Unit _)
        {
            _window.Show();
        } 
        private void OnShopBeforeHide(Unit _)
        {
            _window.Hide();
        }
        
        private void OnSoftChanged(int newValue)
        {
            _window.SoftCounterField.SetValue(newValue, true);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}