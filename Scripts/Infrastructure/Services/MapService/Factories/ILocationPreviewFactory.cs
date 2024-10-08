using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public interface ILocationPreviewFactory
    {
        LocationPreview Create(Transform parent, LocationPreview prefab, ILocationConfig item);
        LocationCategoryPreview Create(Transform parent, LocationCategoryPreview prefab,
            ILocationsCategory category);
    }
}