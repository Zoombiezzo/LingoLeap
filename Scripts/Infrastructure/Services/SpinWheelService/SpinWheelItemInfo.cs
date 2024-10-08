using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.Services.RewardsManagement;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    [Serializable]
    public class SpinWheelItemInfo : ISpinWheelItem
    {
        [SerializeField] [ValueDropdown("SpritesDropdown")]
        private string _spriteId;
        [SerializeField] private string _text;
        [SerializeField] private RewardInfo _reward;
        [SerializeField] [Range(0f, 1f)] 
        private float _chance;
        
        public string SpriteId => _spriteId;
        public string Text => _text;
        public IRewardInfo Reward => _reward;
        public float Chance => _chance;
      
#if UNITY_EDITOR
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>(){""};
            var configs = AssetHelper<SpritesPreset>.GetAsset("Assets");
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