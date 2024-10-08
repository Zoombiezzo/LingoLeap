using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/Map/MapConfig", order = 0)]
    public class MapConfig : ScriptableObject, IMapConfig
    {
        [SerializeField] private string _id;
        [SerializeField] private List<LocationsCategoryConfig>_categories;
        [SerializeField] private LocationConfig _base;
        
        public string Id => _id;
        public IReadOnlyList<LocationsCategoryConfig> Categories => _categories;
        public LocationConfig Base => _base;
    }
}