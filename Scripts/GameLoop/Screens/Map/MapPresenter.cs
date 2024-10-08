using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using _Client.Scripts.Helpers;
using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using VContainer.Unity;
using Vector2 = UnityEngine.Vector2;

namespace _Client.Scripts.GameLoop.Screens.Map
{
    public class MapPresenter : IAsyncStartable, IDisposable
    {
        private const float InteractButtonOffset = 0f; 

        private readonly IMapService _mapService;
        
        private MapWindow _window;
        private MapContainer _container;

        private CompositeDisposable _compositeDisposable = new();
        
        private Sequence _scrollSequence;
        

        public MapPresenter(IMapService mapService)
        {
            _mapService = mapService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            WindowsService.TryGetWindow(out _window);
            
            
            _window.CloseButton.OnClick.AsObservable().Subscribe(OnCloseClick).AddTo(_compositeDisposable);
            _window.ButtonNext.OnClick.AsObservable().Subscribe(OnButtonNextClick).AddTo(_compositeDisposable);
            _window.ButtonPrevious.OnClick.AsObservable().Subscribe(OnButtonPreviousClick).AddTo(_compositeDisposable);
            
            _container = _window.Container;
            
            await CreateMap().AsUniTask();

            InitializeButtonsScroll();
            
            Observable.FromEvent<string>(h => _container.SelectLocation += h, h => _container.SelectLocation -= h)
                .Subscribe(OnSelectLocation).AddTo(_compositeDisposable);
            
            Observable.FromEvent<Vector2>(h => _container.OnScroll += h, h => _container.OnScroll -= h)
                .Subscribe(OnScrollContainer).AddTo(_compositeDisposable);
            
            Observable.FromEvent(h => _container.OnBeginDrag += h, h => _container.OnBeginDrag -= h)
                .Subscribe(OnBeginDrag).AddTo(_compositeDisposable);

            Observable.FromEvent<string>(h => _mapService.LocationSelected += h, h => _mapService.LocationSelected -= h)
                .Subscribe(OnLocationSelected).AddTo(_compositeDisposable);
            
            Observable.FromEvent<int>(h => _mapService.LocationOpened += h, h => _mapService.LocationOpened -= h).
                Subscribe(OnLocationOpened).AddTo(_compositeDisposable);
            
            Observable.FromEvent<int>(h => _mapService.ProgressChanged += h, h => _mapService.ProgressChanged -= h).
                Subscribe(OnProgressChanged).AddTo(_compositeDisposable);

            Observable.FromEvent(h => _window.OnBeforeShow += h, h => _window.OnBeforeShow -= h).Subscribe(OnBeforeShow)
                .AddTo(_compositeDisposable);
        }
        
        private void OnBeginDrag(Unit _)
        {
            if (_scrollSequence != null && _scrollSequence.IsPlaying())
            {
                _scrollSequence.Kill();
                _scrollSequence = null;
            }
        }

        private void OnScrollContainer(Vector2 _)
        {
            InitializeButtonsScroll();
        }

        private void InitializeButtonsScroll()
        {
            var scrollRect = _container.ScrollRect;
            var normalizedPosition = scrollRect.normalizedPosition;

            if (scrollRect.horizontal)
            {
                if (normalizedPosition.x > 0f)
                {
                    _window.ButtonPrevious.Show();
                }
                else
                {
                    _window.ButtonPrevious.Hide();
                }
            }
            
            if (scrollRect.vertical)
            {
                if (normalizedPosition.y > 0f)
                {
                    _window.ButtonPrevious.Show();
                }
                else
                {
                    _window.ButtonPrevious.Hide();
                }
            }

            UpdateButtonSprite();
        }

        private void UpdateButtonSprite()
        {
            var currentLocation = _container.GetCurrentLocation();
            var scrollRect = _container.ScrollRect;
            var anchoredPosition =
                scrollRect.GetAnchoredPosition(currentLocation.RectTransform, new Vector2(0f, 1f));

            var content = scrollRect.content;
            var diff = anchoredPosition - content.anchoredPosition;
            diff.x = (int)diff.x;
            diff.y = (int)diff.y;
            
            if (diff.x > InteractButtonOffset || diff.y > InteractButtonOffset)
            {
                _window.ButtonPreviousImage.sprite = _window.ArrowElement;
            }
            else
            {
                _window.ButtonPreviousImage.sprite = _window.ArrowBase;
            }


            anchoredPosition =
                scrollRect.GetAnchoredPosition(currentLocation.RectTransform, new Vector2(1f, 0f));
            diff = anchoredPosition - content.anchoredPosition;
            diff.x = (int)diff.x;
            diff.y = (int)diff.y;
            
            if ((scrollRect.horizontal && diff.x >= InteractButtonOffset) || (scrollRect.vertical && diff.y >= InteractButtonOffset))
            {
                _window.ButtonNextImage.sprite = _window.ArrowBase;
                
                var lastOpened = _container.GetLastOpenedLocation();
                
                anchoredPosition =
                    scrollRect.GetAnchoredPosition(lastOpened.RectTransform, new Vector2(1f, 0f));
                diff = anchoredPosition - content.anchoredPosition;
                
                if ((scrollRect.horizontal && diff.x >= InteractButtonOffset) || (scrollRect.vertical && diff.y >= InteractButtonOffset))
                {
                    _window.ButtonNext.Hide();
                }
                else
                {
                    _window.ButtonNext.Show();
                }
            }
            else
            {
                _window.ButtonNextImage.sprite = _window.ArrowElement;
                _window.ButtonNext.Show();
            }

        }

        private void OnButtonNextClick(Unit _)
        {
            var scrollRect = _container.ScrollRect;
            LocationPreview targetLocation = null;

            var currentLocation = _container.GetCurrentLocation();
            var anchoredPositionCurrent =
                scrollRect.GetAnchoredPosition(currentLocation.RectTransform, new Vector2(1f, 0f));

            var content = scrollRect.content;
            var diff = anchoredPositionCurrent - content.anchoredPosition;
            diff.x = (int)diff.x;
            diff.y = (int)diff.y;
            
            if (diff.x < InteractButtonOffset || diff.y < InteractButtonOffset)
            {
                targetLocation = currentLocation;
            }
            else
            {
                var lastOpened = _container.GetLastOpenedLocation();
                
                anchoredPositionCurrent =
                    scrollRect.GetAnchoredPosition(lastOpened.RectTransform, new Vector2(0.5f, 0.5f));
                diff = anchoredPositionCurrent - content.anchoredPosition;
                diff.x = (int)diff.x;
                diff.y = (int)diff.y;
                
                if (diff.x < InteractButtonOffset || diff.y < InteractButtonOffset)
                {
                    targetLocation = lastOpened;
                }
            }

            if (targetLocation == null)
                return;

            var anchoredPosition =
                scrollRect.GetAnchoredPosition(targetLocation.RectTransform, new Vector2(0.5f, 0.5f));

            if (_scrollSequence != null && _scrollSequence.IsPlaying())
            {
                _scrollSequence.Kill();
                _scrollSequence = null;
            }
            
            scrollRect.velocity = Vector2.zero;

            _scrollSequence = DOTween.Sequence();
            _scrollSequence.Append(scrollRect.content.DOAnchorPos(anchoredPosition, 0.5f));
        }

        private void OnButtonPreviousClick(Unit _)
        {
            var scrollRect = _container.ScrollRect;
            LocationPreview targetLocation = null;

            var currentLocation = _container.GetCurrentLocation();
            var anchoredPositionCurrent =
                scrollRect.GetAnchoredPosition(currentLocation.RectTransform, new Vector2(0f, 1f));

            var content = scrollRect.content;
            var diff = anchoredPositionCurrent - content.anchoredPosition;
            diff.x = (int)diff.x;
            diff.y = (int)diff.y;
            
            if (diff.x > 0 || diff.y > 0)
            {
                targetLocation = currentLocation;
            }
            else
            {
                targetLocation = _container.Locations[0];
            }

            if (targetLocation == null)
                return;

            var anchoredPosition =
                scrollRect.GetAnchoredPosition(targetLocation.RectTransform, new Vector2(0.5f, 0.5f));

            if (_scrollSequence != null && _scrollSequence.IsPlaying())
            {
                _scrollSequence.Kill();
                _scrollSequence = null;
            }
            
            scrollRect.velocity = Vector2.zero;

            _scrollSequence = DOTween.Sequence();
            _scrollSequence.Append(scrollRect.content.DOAnchorPos(anchoredPosition, 0.5f));
        }

        private void OnBeforeShow(Unit _)
        {
            _window.Container.ScrollToLastMap();
        }

        private void OnProgressChanged(int progress)
        {
            UpdateProgress();
        }
        
        private void OnLocationOpened(int index)
        {
            _container.SetOpenedState(index);
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_mapService.TryGetCurrentLocationConfig(out var config))
            {
                _container.SetProgress(_mapService.CurrentIndex, _mapService.ProgressCounter, config.RequiredCountLevels);
            }
        }
        
        private void OnLocationSelected(string location)
        {
            _container.SetLocationSelected(location);
            InitializeButtonsScroll();
        }
        
        private void OnSelectLocation(string location)
        {
            _mapService.TrySelectLocation(location);
        }

        private async Task CreateMap()
        {
            var maps = _mapService.MapConfigs;
            
            if(maps == null || maps.Count == 0)
                return;

            foreach (var map in maps)
            {
                await _container.Create(map);
            }
            
            _container.CloseAll();
            _container.SetOpenedState(_mapService.CurrentIndex);
            UpdateProgress();
        } 
        
        private void OnCloseClick(Unit _)
        {
            _window.Hide();
        }
        
        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}