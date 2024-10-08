using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using _Client.Scripts.Helpers;
using _Client.Scripts.Infrastructure.Services.MapService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using Debug = UnityEngine.Debug;

namespace _Client.Scripts.GameLoop.Screens.Map
{
    public class MapContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private ScrollRect _scrollRect;

        private readonly Dictionary<string, LocationPreview> _locationViews = new(16);
        private readonly List<LocationPreview> _locations = new(16);
        
        private readonly Dictionary<string, LocationCategoryPreview> _categoryViews = new(16);
        private readonly List<LocationCategoryPreview> _categories = new(16);
        
        private IMapService _mapService;
        
        private LocationPreview _selectedLocation;
        private int _lastOpened;
        
        private readonly CompositeDisposable _disposable = new();
        private readonly CompositeDisposable _disposableUI = new();
        
        private readonly Vector3[] _cornersViewport = new Vector3[4];
        private readonly Vector3[] _cornersElement = new Vector3[4];
        
        private Coroutine _coroutineScrollToLastOpened;

        public ScrollRect ScrollRect => _scrollRect;
        public IReadOnlyList<LocationPreview> Locations => _locations;
        
        public event Action<string> SelectLocation;
        public event Action<Vector2> OnScroll;
        public event Action OnBeginDrag;

        [Inject]
        public void Construct(IMapService mapService)
        {
            _mapService = mapService;
        }
        
        public async Task Create(IMapConfig config)
        {
            if(config == null)
                return;
            
            if(config.Categories == null)
                return;
            
            foreach (var category in config.Categories)
            {
                await CreateCategory(_container, category);
            }

            if (_locationViews.TryGetValue(_mapService.CurrentSelectedLocationId, out var locationView))
            {
                _selectedLocation = locationView;
            }
        }

        public void ScrollToLastMap()
        {
            if (_coroutineScrollToLastOpened != null)
            {
                StopCoroutine(_coroutineScrollToLastOpened);
                _coroutineScrollToLastOpened = null;
            }
            
            _coroutineScrollToLastOpened = StartCoroutine(ScrollToLastOpenedCoroutine());
        }

        public LocationPreview GetCurrentLocation() =>
            _mapService.TryGetCurrentLocationConfig(out _) == false ?
                _selectedLocation : GetLastOpenedLocation();

        public LocationPreview GetLastOpenedLocation() => _locations[Mathf.Clamp(_lastOpened, 0, _locations.Count - 1)];

        private IEnumerator ScrollToLastOpenedCoroutine()
        {
            yield return new WaitForEndOfFrame();
            
            _scrollRect.velocity = Vector2.zero;
            var location = GetCurrentLocation();
            
            _scrollRect.ScrollToElement(location.RectTransform, new Vector2(0.5f, 0.5f));
        }

        private async Task CreateCategory(Transform root, ILocationsCategory category)
        {
            var categoryView = await _mapService.CreateCategoryPreview(root, category);
            
            if (categoryView == null)
                return;
            
            _categoryViews.TryAdd(category.Id, categoryView);
            _categories.Add(categoryView);
            
            var locations = category.Locations;
            
            if (locations != null)
            {
                foreach (var location in locations)
                {
                    var locationView = await _mapService.CreatePreview(categoryView.Content, location);

                    if (locationView == null)
                        continue;

                    _locationViews.TryAdd(location.Id, locationView);
                    _locations.Add(locationView);
                    
                    Subscribe(locationView);
                }
            }

            await UniTask.NextFrame();
        }
        
        private void Subscribe(LocationPreview locationView)
        {
            Observable.FromEvent<LocationPreview>(h => locationView.OnSelectClick += h,
                h => locationView.OnSelectClick -= h).Subscribe(OnSelect).AddTo(_disposable);
        }
        
        private void OnSelect(LocationPreview location)
        {
            SelectLocation?.Invoke(location.Id);
        }

        private void OnEnable()
        {
            _scrollRect.onValueChanged.AsObservable().Subscribe(OnScrollValueChanged).AddTo(_disposableUI);
            _scrollRect.OnBeginDragAsObservable().Subscribe(OnBeginDragHandler).AddTo(_disposableUI);
        }

        private void OnDisable()
        {
            _disposableUI?.Dispose();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
        
        private void OnBeginDragHandler(PointerEventData _)
        {
            OnBeginDrag?.Invoke();
        }

        public void SetLocationSelected(string location)
        {
            if (_locationViews.TryGetValue(location, out var locationView))
            {
                SetLocationSelected(locationView, true);
            }

            if (_selectedLocation != null)
            {
                SetLocationSelected(_selectedLocation, false);
            }
            
            _selectedLocation = locationView;
        }

        public void CloseAll()
        {
            foreach (var location in _locations)
            {
                SetLocationClosed(location);
            }
        }

        public void SetOpenedState(int openedIndex)
        {
            for(int i = _lastOpened; i < openedIndex; i++)
            {
                if(_locations.Count < i)
                    break;
                
                var location = _locations[i];
                
                SetLocationOpened(location);
            }

            if (_locations.Count > openedIndex)
            {
                if (_mapService.TryGetCurrentLocationConfig(out var config))
                {
                    SetLocationProgress(_locations[openedIndex]);
                }
            }

            _lastOpened = openedIndex;
        }

        public void SetProgress(int index, int count, int maxCount)
        {
            if(_locations.Count <= index)
                return;
            
            
            ChangeLocationProgress(_locations[index], count, maxCount);
        }

        private void SetLocationOpened(LocationPreview preview)
        {
            preview.SetOpenedState();
        }

        private void SetLocationClosed(LocationPreview preview)
        {
            preview.SetClosedState();
        }

        private void SetLocationProgress(LocationPreview preview)
        {
            preview.SetProgressState();
        }

        private void ChangeLocationProgress(LocationPreview preview, int count, int maxCount)
        {
            preview.SetProgress(count, maxCount);
        }

        private void SetLocationSelected(LocationPreview preview, bool select)
        {
            preview.SetButtonStateSelected(select);
        }

        private void OnScrollValueChanged(Vector2 position)
        {
            UpdateVisibleElements();
            
            OnScroll?.Invoke(position);
        }

        private void UpdateVisibleElements()
        {
            foreach (var location in _locations)
            {
                var result = IsRectTransformVisibleInScrollRect(location.RectTransform);
                location.SetVisible(result);
            }

            for (var index = 0; index < _categories.Count; index++)
            {
                var category = _categories[index];
                category.TryUpdateHeader();
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