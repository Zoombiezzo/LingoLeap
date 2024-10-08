using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using SQLite;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.WordsLevelsService
{
    public class WordsLevelsService : IWordsLevelsService
    {
        private const string ConfigPath = "WordsLevels";
        private const string Language = "ru";
        
        private readonly IAssetProvider _assetProvider;
        private readonly IAddressablesService _addressablesService;
        private readonly ILocalizationService _localeService;
        private readonly Dictionary<string, int> _countLevels;
        private readonly string _path;
        private readonly string _filePath;
        private readonly string _extension;

        public WordsLevelsService(IAssetProvider assetProvider, IAddressablesService addressablesService,
            ILocalizationService languageService)
        {
            _assetProvider = assetProvider;
            _addressablesService = addressablesService;
            _localeService = languageService;
            _path = Path.Combine(Application.persistentDataPath, "WordsDatabase");
            _extension = "bytes";
            _filePath = Path.Combine(_path, $"{ConfigPath}.{_extension}");
            _countLevels = new Dictionary<string, int>();
        }
        
        public async Task LoadData()
        {
            var settings = await _assetProvider.Load<WordsLevelsSettings>(ConfigPath);

            var result = await _addressablesService.Load<TextAsset>(settings.Asset);

            if (Directory.Exists(_path) == false)
                Directory.CreateDirectory(_path);

            File.WriteAllBytes(_filePath, result.bytes);

            _addressablesService.Release(settings.Asset);
        }
        
        public bool TryGetLevel(int level, out WordsLevel wordsLevel)
        {
            wordsLevel = default;
            
            using var db = new SQLiteConnection(_filePath);
            var localization = _localeService.CurrentLanguageCode;
            
            var result = db.Query<WordsLevelsRecord>($"SELECT * FROM {localization} WHERE level_number = {level}");

            if (result.Count == 0)
            {
                var maxLevel = GetCountLevels();
                var currentLevel = level % maxLevel;

                result = db.Query<WordsLevelsRecord>($"SELECT * FROM {localization} WHERE level_number = {currentLevel}");

                if (result.Count == 0)
                    return false;
            }

            var levelResult = result[0];
            wordsLevel = new WordsLevel(level, levelResult.Chars, levelResult.Words);
            
            return true;
        }

        public int GetCountLevels()
        {
            var localization = _localeService.CurrentLanguageCode;
            if (_countLevels.TryGetValue(localization, out var count))
                return count;
            
            
            using var db = new SQLiteConnection(_filePath);
            var result = db.QueryScalars<int>($"SELECT COUNT(*) FROM {localization}");

            if (result is not { Count: > 0 })
                return 0;
                
            count = result[0];
            _countLevels.Add(localization, count);

            return count;
        }

        private record WordsLevelsRecord
        {
            public int LevelNumber { get; set; }
            public string Chars { get; set; }
            public string Words { get; set; }
        }
    }
}