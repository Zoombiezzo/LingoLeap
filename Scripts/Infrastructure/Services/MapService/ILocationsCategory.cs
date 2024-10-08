using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public interface ILocationsCategory
    {
        string Id { get; }
        string NameId { get; }
        Color Color { get; }
        LocationCategoryPreview CategoryPrefab { get; }
        IReadOnlyList<ILocationConfig> Locations { get; }
    }
}