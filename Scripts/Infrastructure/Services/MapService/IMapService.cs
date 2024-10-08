using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public interface IMapService : IConfigData, IStorable
    {
        event Action<string> LocationSelected;
        event Action<int> LocationOpened;
        event Action<int> ProgressChanged;
        event Action<ILocationConfig> OnShowScreenRewardLocation;
        IReadOnlyList<IMapConfig> MapConfigs { get; }
        string CurrentSelectedLocationId { get; }
        int ProgressCounter { get; }
        int CurrentIndex { get; }
        void ShowRewardWindow(ILocationConfig config);
        bool TryGetCurrentLocationConfig(out ILocationConfig config);
        bool TryGetLocationByIndexConfig(int index, out ILocationConfig config);
        bool TryGetLocationConfig(string id, out ILocationConfig config);
        bool IsLocationAvailableToSelect(string locationId);
        Task<LocationPicture> CreatePicture(Transform parent, ILocationConfig config);
        Task<LocationPreview> CreatePreview(Transform parent, ILocationConfig config);
        Task<LocationCategoryPreview> CreateCategoryPreview(Transform parent, ILocationsCategory category);
        void Release(ILocationConfig config);
        bool TrySelectLocation(string location);
        void AddProgress(int count);
        bool TryCollectReward(out IReadOnlyList<IRewardInfo> rewards);
    }
}