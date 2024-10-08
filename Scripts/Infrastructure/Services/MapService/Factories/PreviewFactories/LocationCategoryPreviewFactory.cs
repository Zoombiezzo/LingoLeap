using _Client.Scripts.Infrastructure.Services.LocalizationService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories.PreviewFactories
{
    public class LocationCategoryPreviewFactory : ILocationCategoryFactory
    {
        private readonly ILocalizationService _localizationService;

        public LocationCategoryPreviewFactory(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        
        public LocationCategoryPreview Create(Transform parent, LocationCategoryPreview prefab, ILocationsCategory category)
        {
            var view = Object.Instantiate(prefab, parent);
            
            view.RegisterLocalization(_localizationService);
            view.SetNameLocalizationKey(category.NameId);
            view.SetColor(category.Color);
            
            return view;
        }
    }
}