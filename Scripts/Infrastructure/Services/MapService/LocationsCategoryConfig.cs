using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    [CreateAssetMenu(fileName = "LocationsCategoryConfig", menuName = "Configs/Map/LocationsCategoryConfig", order = 0)]
    public class LocationsCategoryConfig : ScriptableObject, ILocationsCategory
    {
        [SerializeField] private string _id;
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _nameId;
        [SerializeField] private Color _color;
        [SerializeField] private LocationCategoryPreview _categoryPrefab;
        [SerializeField] private List<LocationConfig> _locations;
        
        public string Id => _id;
        public string NameId => _nameId;
        public Color Color => _color;
        public LocationCategoryPreview CategoryPrefab => _categoryPrefab;
        public IReadOnlyList<ILocationConfig> Locations => _locations;
    }
}