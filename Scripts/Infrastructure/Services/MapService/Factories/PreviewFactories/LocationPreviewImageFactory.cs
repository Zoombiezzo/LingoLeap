using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories.PreviewFactories
{
    public class LocationPreviewImageFactory : ILocationPreviewItemFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly IMapService _mapService;

        public LocationPreviewImageFactory(IMapService mapService, ILocalizationService localizationService)
        {
            _mapService = mapService;
            _localizationService = localizationService;
        }

        public LocationPreview Create(Transform parent, LocationPreview prefab, ILocationConfig item)
        {
            if (prefab is not LocationPreviewImage locationPreviewImagePrefab)
                return null;
            
            var view = Object.Instantiate(locationPreviewImagePrefab, parent);
            
            view.SetSprite(item.Icon);
            view.SetBlockedColor(item.BlockedColor);
            
            var isSelected = item.Id == _mapService.CurrentSelectedLocationId;
            var isAvailableToSelect = _mapService.IsLocationAvailableToSelect(item.Id);
            
            view.SetId(item.Id);
            view.RegisterLocalization(_localizationService);
            view.ShowProgressbar(isAvailableToSelect == false);
            view.ShowButtonSelect(isAvailableToSelect);
            view.SetButtonStateSelected(isSelected);
            
            return view;
        }
    }
}