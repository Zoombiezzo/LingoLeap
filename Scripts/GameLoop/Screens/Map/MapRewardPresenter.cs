using System;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.StateMachine;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using VContainer.Unity;

namespace _Client.Scripts.GameLoop.Screens.Map
{
    public class MapRewardPresenter : IStartable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMapService _mapService;
        
        private MapRewardWindow _window;
        private IDisposable _disposable;

        private bool _isBlocked = false;
        private Sequence _showSequence;
        private Sequence _hideSequence;
        
        private ILocationConfig _unlockedLocation;

        public MapRewardPresenter(IGameStateMachine gameStateMachine, IMapService mapService)
        {
            _gameStateMachine = gameStateMachine;
            _mapService = mapService;
        }
        
        public void Start()
        {
            WindowsService.TryGetWindow(out _window);
            
            var disposableBuilder = Disposable.CreateBuilder();
            
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnCloseClick).AddTo(ref disposableBuilder);
            _window.ApplyButton.OnClick.AsObservable().Subscribe(OnSelectClick).AddTo(ref disposableBuilder);
            
            Observable.FromEvent<ILocationConfig>(h => _mapService.OnShowScreenRewardLocation += h, h => _mapService.OnShowScreenRewardLocation -= h).Subscribe(OnShowScreenRewardLocation)
                .AddTo(ref disposableBuilder);
            
            _disposable = disposableBuilder.Build();
        }

        private void OnShowScreenRewardLocation(ILocationConfig locationConfig)
        {
            _unlockedLocation = locationConfig;
            _window.Icon.sprite = locationConfig.Icon;
            ShowScreen();
        }
        
        private void OnCloseClick(Unit _)
        {
            if(_isBlocked)
                return;
            
            HideScreen();
        }

        private void OnSelectClick(Unit _)
        {
            if(_isBlocked)
                return;
            
            _mapService.TrySelectLocation(_unlockedLocation.Id);
            
            HideScreen();
        }
        
        private async void ShowScreen()
        {
            _isBlocked = true;
            
            _window.AnimationElement.Hide();
            _window.Show();

            _window.Particles.Play();

            await UniTask.WaitUntil(() => _window.IsShow());
            
            _showSequence?.Kill();
            _hideSequence?.Kill();
            _showSequence = DOTween.Sequence();

            _showSequence.AppendCallback(() => _window.AnimationElement.Show(true));
            _showSequence.AppendInterval(_window.AnimationElement.DurationShow);

            await _showSequence.Play().AsyncWaitForCompletion().AsUniTask();
            
            _isBlocked = false;
        }

        private async void HideScreen()
        {
            _isBlocked = true;
            
            _showSequence?.Kill();
            _hideSequence?.Kill();
            _hideSequence = DOTween.Sequence();
            
            _hideSequence.AppendCallback(() => _window.AnimationElement.Hide(true));
            _hideSequence.AppendInterval(_window.AnimationElement.DurationHide);
            
            _hideSequence.AppendCallback(() => _window.Hide());
            
            await _hideSequence.Play().AsyncWaitForCompletion().AsUniTask();
            
            await UniTask.WaitUntil(() => _window.IsShow() == false);
            
            var particles = _window.Particles;
            particles.Stop();
            particles.Clear();
            
            _isBlocked = false;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}