using System;
using System.Collections.Generic;
using System.Linq;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Helpers
{
    [Serializable]
    public class SpriteSelector
    {
        [SerializeField] [ValueDropdown("SpritesDropdown")] [HideLabel]
        private string _spriteId;
        
        public string SpriteId => _spriteId;
        
#if UNITY_EDITOR
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>();
            var configs = ConfigsHelper<SpritesPreset>.GetConfigs();
            foreach (var spritesPreset in configs)
            {
                foreach (var sprites in spritesPreset.SpritePresets)
                {
                    spritesIds.Add(spritesPreset.GetIdSprite(sprites.Id));
                }
            }

            return spritesIds;
        }
#endif
    }
}