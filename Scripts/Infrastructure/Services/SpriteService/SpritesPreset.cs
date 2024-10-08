using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    [CreateAssetMenu(fileName = "Sprites Preset", menuName = "Configs/Sprite/Sprites Preset", order = 0)]
    public class SpritesPreset : ScriptableObject, ISpritesPreset
    {
        [SerializeField] private string _id;
        [SerializeField] private List<SpritePreset> _spritePresets = new List<SpritePreset>();

#if UNITY_EDITOR
        [BoxGroup("Settings")] [BoxGroup("Settings/Path")] [FolderPath] [SerializeField]
        private List<string> _folders = new List<string>();
#endif
        
        public string Id => _id;
        public IReadOnlyList<SpritePreset> SpritePresets => _spritePresets;

        public string GetIdSprite(string idSprite) => _id + ":" + idSprite;

#if UNITY_EDITOR
        [BoxGroup("Settings")]
        [BoxGroup("Settings/Path")]
        [Button("Initialize Sprites From Path")]
        private void InitializeSpritesFromPath()
        {
            _spritePresets ??= new List<SpritePreset>();

            List<SpritePreset> newPresets = new List<SpritePreset>();
            foreach (var folder in _folders)
            {
                var listSprites = Helpers.AssetHelper<Sprite>.LoadAllAssetRepresentationsAtPath(folder);

                foreach (var sprite in listSprites)
                {
                    var pieces = sprite.name.ToLower().Split(" ");
                    var id = string.Join("_", pieces);

                    var item = _spritePresets.FirstOrDefault(el => el.Id == id);
                    if (item != null)
                    {
                        Debug.LogWarning($"[SpritesPreset] Sprite with id {id} contains!");
                        newPresets.Add(item);
                        continue;
                    }
                    
                    item =  _spritePresets.FirstOrDefault(el => el.Sprite == sprite);
                    if (item != null)
                    {
                        Debug.LogWarning($"[SpritesPreset] Sprite with sprite {sprite.name} contains!");
                        newPresets.Add(item);
                        continue;
                    }

                    newPresets.Add(new SpritePreset(id, sprite));
                }
            }

            _spritePresets = newPresets;
        }
        
        [BoxGroup("Settings")]
        [BoxGroup("Settings/Path")]
        [Button("Clear")]
        private void Clear()
        {
            _id = string.Empty;
            _spritePresets ??= new List<SpritePreset>();
            _spritePresets.Clear();
            _folders.Clear();
        }
#endif
    }
}