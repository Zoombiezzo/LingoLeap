using System;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Background
{
    public class BackgroundPresenter : IStartable, IDisposable
    {
        private BackgroundWindow _window;
        private IBackgroundChanger _changer;
        
        private readonly IMapService _mapService;
        
        private ILocationConfig _currentConfig;
        
        private IDisposable _disposable;

        public BackgroundPresenter(IMapService mapService)
        {
            _mapService = mapService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);

            _changer = new BackgroundChanger(this, _window);
            
            var builder = Disposable.CreateBuilder();

            Observable.FromEvent<string>(h => _mapService.LocationSelected += h, h => _mapService.LocationSelected -= h)
                .Subscribe(OnLocationSelected).AddTo(ref builder);
            
            _disposable = builder.Build();

            CreateBackground();
        }
        
        private void OnLocationSelected(string location)
        {
            _changer.Change(location);
        }

        public async Task ChangeBackground(string location)
        {
            _mapService.TryGetLocationConfig(location, out var newLocationConfig);
            
            if(_currentConfig == newLocationConfig)
                return;
            
            var background = await _mapService.CreatePicture(_window.Container, newLocationConfig);

            var previousBackground = _window.Picture;
            
            _window.SetCurrentBackground(background);
            
            await _window.Picture.Show(true);
            await previousBackground.Hide(false);
            
            _mapService.Release(_currentConfig);
            
            _currentConfig = newLocationConfig;
        }
        
        private async void CreateBackground()
        {
            _mapService.TryGetLocationConfig(_mapService.CurrentSelectedLocationId, out _currentConfig);
            var background = await _mapService.CreatePicture(_window.Container, _currentConfig);
            _window.SetCurrentBackground(background);
            
            await _window.Picture.Show(false);
        }
        
        public void Dispose()
        {
            _mapService.Release(_currentConfig);
            
            _disposable.Dispose();
        }
    }
}