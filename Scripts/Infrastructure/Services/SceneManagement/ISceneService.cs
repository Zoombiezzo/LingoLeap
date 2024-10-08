using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.ConfigData;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    public interface ISceneService : IConfigData
    {
        float Progress { get; }
        void Initialize();
        bool TryGetScene(string sceneName, out Scene scene);
        bool TryGetScenesFromPreset(string presetId, ref List<Scene> scenes);
        Task LoadScenesFromPreset(string presetId, Action<string, float> progress = null);
        Task ChangeScenesFromPreset(string presetId, Action<string, float> progress = null);
        Task UnloadScenesFromPreset(string presetId, Action<string, float> progress = null, List<string> dontUnloadScenesFromPresets = null, bool immediately = false);
        Task LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, Action<float> progress = null);
        Task UnloadScene(string sceneName, UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action<float> progress = null);
        bool TryGetLoadedScene(string sceneName, out Scene scene);
        void SetActiveScene(string sceneName);
    }
}