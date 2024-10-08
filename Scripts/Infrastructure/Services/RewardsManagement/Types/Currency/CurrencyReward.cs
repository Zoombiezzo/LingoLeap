using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.SpriteService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.RewardsManagement.Types.Currency
{
    [System.Serializable]
    public class CurrencyReward : IReward
    {
        [SerializeField] [LabelText("Валюта")]
        private CurrencyType _type;
        [SerializeField] [LabelText("Количество")]
        private int _value;

        [SerializeField] [LabelText("Иконка")] [ValueDropdown("SpritesDropdown")]
        private string _iconId = string.Empty;
        
        public RewardType Type => RewardType.Currency;
        public CurrencyType CurrencyType => _type;
        public int Count => _value;
        
        public string IconId { get; }

#if UNITY_EDITOR
        public string TypeName => "Валюта";
        
        private IEnumerable<string> SpritesDropdown()
        {
            var spritesIds = new List<string>() { string.Empty };
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