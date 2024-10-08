using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    public class SceneService : ISceneService
    {
        private const string ConfigPath = "ScenePreset";
        
        public float Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                _onProgressChanged?.Invoke(_progress);
            }
        }

        private float _progress;

        private string _currentPreset;
        
        private Dictionary<string,ScenePreset> _presets;
        private Dictionary<string, Scene> _loadedScenes;
        private Action<float> _onProgressChanged;
        private readonly IAssetProvider _assetProvider;

        public SceneService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public void Initialize()
        {
            RegisterCurrentLoadedScenes();
        }

        public ScenePreset GetPreset(string id) => _presets.TryGetValue(id, out ScenePreset preset) ? preset : null;

        public bool TryGetScene(string sceneName, out Scene scene) => _loadedScenes.TryGetValue(sceneName, out scene);

        public bool TryGetScenesFromPreset(string presetId, ref List<Scene> scenes)
        {
            scenes ??= new List<Scene>();

            var preset = GetPreset(presetId);

            if (preset == null) return false;

            foreach (var scene in preset.Scenes)
            {
                if (TryGetScene(scene.Scene, out var loadedScene))
                    scenes.Add(loadedScene);
            }

            return scenes.Count == preset.Scenes.Count();
        }

        public async Task LoadScenesFromPreset(string presetId, Action<string, float> progressCallback = null)
        {
            var preset = GetPreset(presetId);

            if (preset == null) return;

            _currentPreset = presetId;
            
            foreach (var scene in preset.Scenes)
                await LoadScene(scene.Scene, scene.LoadSceneMode,
                    progress => progressCallback?.Invoke(scene.Scene, progress));
        }

        public async Task ChangeScenesFromPreset(string presetId, Action<string, float> progressCallback = null)
        {
            var preset = GetPreset(presetId);
            
            if (preset == null) return;

            var listLoadedScenes = _loadedScenes.Keys.ToHashSet();
            
            foreach (var scene in preset.Scenes)
            {
                var sceneName = scene.Scene;
                await LoadScene(sceneName, scene.LoadSceneMode,
                    progress => progressCallback?.Invoke(scene.Scene, progress));

                if (_loadedScenes.ContainsKey(sceneName))
                {
                    listLoadedScenes.Remove(sceneName);
                }
            }

            foreach (var scene in listLoadedScenes)
            {
                await UnloadScene(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects,
                    progress => progressCallback?.Invoke(scene, progress));
            }
        }

        public async Task UnloadScenesFromPreset(string presetId, Action<string, float> progressCallback = null, List<string> dontUnloadScenesFromPresets = null, bool immediately = false)
        {
            var preset = GetPreset(presetId);

            if (preset == null) return;

            var scenesForRemove = new HashSet<string>(10);
            
            foreach (var scene in preset.Scenes)
            {
                scenesForRemove.Add(scene.Scene);
            }
            
            if (string.IsNullOrEmpty(_currentPreset) == false && immediately == false)
            {
                if (_currentPreset != presetId)
                {
                    var currentPreset = GetPreset(_currentPreset);

                    if (currentPreset != null)
                    {
                        foreach (var scene in currentPreset.Scenes)
                        {
                            scenesForRemove.Remove(scene.Scene);
                        }
                    }
                }
            }
            
            if(dontUnloadScenesFromPresets != null && dontUnloadScenesFromPresets.Count > 0)
            {
                foreach (var presetIdDontUnload in dontUnloadScenesFromPresets)
                {
                    var presetDontUnload = GetPreset(presetIdDontUnload);


                    foreach (var scene in presetDontUnload.Scenes)
                    {
                        scenesForRemove.Remove(scene.Scene);
                    }
                }
            }
            
            foreach (var scene in scenesForRemove)
                await UnloadScene(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects,
                    progress => progressCallback?.Invoke(scene, progress));
        }

        public async Task LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, Action<float> progressCallback = null)
        {
            Progress = 0;
            progressCallback?.Invoke(Progress);

            if (_loadedScenes.ContainsKey(sceneName))
            {
                Progress = 1f;
                progressCallback?.Invoke(Progress);

                return;
            }

            var sceneRef = SceneManager.GetSceneByName(sceneName);
            if (sceneRef.isLoaded)
            {
                Progress = 1f;
                _loadedScenes.Add(sceneName, sceneRef);
                progressCallback?.Invoke(Progress);

                return;
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, mode);

            while (operation.isDone == false)
            {
                Progress = operation.progress;
                progressCallback?.Invoke(Progress);
                
                await Task.Yield();
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            
            if (scene.isLoaded)
            {
                _loadedScenes.Add(sceneName, scene);
            }
            
            Progress = 1f;
            progressCallback?.Invoke(Progress);
            await Task.Yield();
        }

        public async Task UnloadScene(string sceneName, UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, Action<float> progressCallback = null)
        {
            Progress = 0;
            progressCallback?.Invoke(Progress);
            
            if (_loadedScenes.TryGetValue(sceneName, out var scene) == false)
            {
                var sceneRef = SceneManager.GetSceneByName(sceneName);
                if (sceneRef.isLoaded == false)
                {
                    Progress = 1;
                    progressCallback?.Invoke(Progress);
                    return;
                }
                _loadedScenes.Add(sceneName, sceneRef);
            }

            var operation = SceneManager.UnloadSceneAsync(scene, unloadSceneOptions);

            while (operation.isDone == false)
            {
                Progress = operation.progress;
                progressCallback?.Invoke(Progress);
                await Task.Yield();
            }
            
            _loadedScenes.Remove(sceneName);
            Progress = 1f;
            progressCallback?.Invoke(Progress);
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();
        }

        public bool TryGetLoadedScene(string sceneName, out Scene scene) => _loadedScenes.TryGetValue(sceneName, out scene);

        public void SetActiveScene(string sceneName)
        {
            if (TryGetLoadedScene(sceneName, out var scene))
            {
                SceneManager.SetActiveScene(scene);
            }
        }
        private void RegisterCurrentLoadedScenes()
        {
            _loadedScenes = new Dictionary<string, Scene>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.isLoaded)
                    _loadedScenes.Add(scene.name, scene);
            }
        }

        public async Task LoadData()
        {
            _presets = (await _assetProvider.LoadAll<ScenePreset>(ConfigPath))
                .ToDictionary(x => x.Id, x => x);
        }
    }
}