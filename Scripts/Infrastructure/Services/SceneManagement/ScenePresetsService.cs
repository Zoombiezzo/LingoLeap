using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    public class ScenePresetsService : IScenePresetsService
    {
        private Dictionary<string,ScenePreset> _presets;

        public void LoadPresets()
        {
            _presets = Resources.LoadAll<ScenePreset>($"Configs/ScenePresets/")
                .ToDictionary(x => x.Id, x => x);
        }
        
        public ScenePreset GetPreset(string id) => _presets.TryGetValue(id, out ScenePreset preset) ? preset : null;
    }
}