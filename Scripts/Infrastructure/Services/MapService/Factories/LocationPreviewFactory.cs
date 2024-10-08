using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.MapService.Factories.PreviewFactories;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public class LocationPreviewFactory : ILocationPreviewFactory
    {
        private Dictionary<Type, ILocationPreviewItemFactory> _itemsFactories;
        private Dictionary<Type, ILocationCategoryFactory> _categoriesFactories;
        
        private readonly ILocalizationService _localizationService;
        private readonly IMapService _mapService;

        public LocationPreviewFactory(IMapService mapService, ILocalizationService localizationService)
        {
            _mapService = mapService;
            _localizationService = localizationService;

            _itemsFactories = new Dictionary<Type, ILocationPreviewItemFactory>()
            {
                { typeof(LocationPreviewImage), new LocationPreviewImageFactory(_mapService, _localizationService) },
            };

            _categoriesFactories = new Dictionary<Type, ILocationCategoryFactory>()
            {
                { typeof(LocationCategoryPreview), new LocationCategoryPreviewFactory(_localizationService) },
            };
        }

        public LocationPreview Create(Transform parent, LocationPreview prefab, ILocationConfig item)
        {
            if (_itemsFactories.TryGetValue(prefab.GetType(), out var factory) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[LocationPreviewFactory]:  Item factory not found for type {prefab.GetType()}");
#endif

                return null;
            }

            return factory.Create(parent, prefab, item);
        }

        public LocationCategoryPreview Create(Transform parent, LocationCategoryPreview prefab, ILocationsCategory category)
        {
            
            if (_categoriesFactories.TryGetValue(prefab.GetType(), out var factory) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[LocationPreviewFactory]:  Category factory not found for type {prefab.GetType()}");
#endif

                return null;
            }
            
            return factory.Create(parent, prefab, category);
        }
    }
}