using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public interface ILocationPreviewItemFactory
    {
        LocationPreview Create(Transform parent, LocationPreview prefab, ILocationConfig item);
    }
}