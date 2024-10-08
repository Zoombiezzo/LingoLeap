using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Helpers;
using GameSDK.Localization;
using System.Linq;

namespace _Client.Scripts.Helpers
{
#if UNITY_EDITOR
    public static class AssetsSelector
    {
        private static List<string> _cachedNames = new(128);
        public static IEnumerable<string> GetLocalizationKeys()
        {
            _cachedNames.Clear();
            _cachedNames.Add("");
            var assets = AssetHelper<LocalizationDatabase>
                .GetAsset();

            var languages = assets.Select(el => el.Languages.First());
            var languagesKeys = languages.SelectMany(el => el.Text.Select(el => el.Key));
            _cachedNames.AddRange(languagesKeys);
            
            return _cachedNames;
        }
    }
#endif
}