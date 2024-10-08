using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public interface ILocationCategoryFactory
    {
        public LocationCategoryPreview Create(Transform parent, LocationCategoryPreview prefab,
            ILocationsCategory category);

    }
}