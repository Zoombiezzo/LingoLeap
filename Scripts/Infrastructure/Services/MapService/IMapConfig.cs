using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public interface IMapConfig
    {
        string Id { get; }
        IReadOnlyList<LocationsCategoryConfig> Categories { get; }
        LocationConfig Base { get; }
    }
}