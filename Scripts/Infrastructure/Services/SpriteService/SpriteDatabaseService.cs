using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    public class SpriteDatabaseService : ISpriteDatabaseService
    {
        private const string SpritesPresetPath = "SpritesAsset";
        private readonly IAssetProvider _assetProvider;

        private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public SpriteDatabaseService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public Sprite GetSprite(string id)
        {
            _sprites.TryGetValue(id, out var sprite);
            return sprite;
        }

        public Sprite GetCurrencySprite(CurrencyType currencyType)
        {
            var currency = currencyType.ToString().ToLower();
            _sprites.TryGetValue($"currency:{currency}", out var sprite);
            return sprite;
        }

        public async Task LoadData()
        {
            var presets = await _assetProvider.LoadAll<SpritesPreset>(SpritesPresetPath);

            _sprites ??= new Dictionary<string, Sprite>();
            _sprites.Clear();
            
            foreach (var preset in presets)
            {
                foreach (var sprite in preset.SpritePresets)
                {
                    var id = preset.GetIdSprite(sprite.Id);
                    if (_sprites.ContainsKey(id))
                    {
#if UNITY_EDITOR
                        Debugger.Log($"[SpriteDatabaseService]: Sprite with id: {id} contains!");        
#endif
                        continue;
                    }
                    
                    _sprites.Add(id, sprite.Sprite);
                }
            }
        }
    }
}