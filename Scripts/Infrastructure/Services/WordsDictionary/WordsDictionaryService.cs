using System.IO;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using SQLite;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    public class WordsDictionaryService : IWordsDictionaryService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IAddressablesService _addressablesService;
        private readonly ILocalizationService _localizationService;
        private readonly string _path;
        private readonly string _extension;
        private readonly string _filePath;
        private const string ConfigPath = "WordsDictionary";

        public WordsDictionaryService(IAssetProvider assetProvider, ILocalizationService localizationService, IAddressablesService addressablesService)
        {
            _assetProvider = assetProvider;
            _addressablesService = addressablesService;
            _localizationService = localizationService;
            
            _path = Path.Combine(Application.persistentDataPath, "WordsDatabase");
            _extension = "bytes";
            _filePath = Path.Combine(_path, $"{ConfigPath}.{_extension}");
        }

        public async Task LoadData()
        {
            var settings = await _assetProvider.Load<WordsDictionarySettings>(ConfigPath);

            var result = await _addressablesService.Load<TextAsset>(settings.WordsAssetDatabase);

            if (Directory.Exists(_path) == false)
                Directory.CreateDirectory(_path);

            File.WriteAllBytes(_filePath, result.bytes);

            _addressablesService.Release(settings.WordsAssetDatabase);
        }

        public bool Contains(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;
            
            using var db = new SQLiteConnection(_filePath);
            var result = db.QueryScalars<int>($"SELECT COUNT(*) FROM {_localizationService.CurrentLanguageCode} WHERE Value = ?", word.ToLower());

            if(result.Count == 0)
                return false;
            
            return result[0] > 0;
        }
    }
}