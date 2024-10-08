using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement;
using _Client.Scripts.Infrastructure.Services.LocalizationService.Data;
using _Client.Scripts.Infrastructure.Services.SaveService;
using GameSDK.Localization;
using TMPro;

namespace _Client.Scripts.Infrastructure.Services.LocalizationService
{
    public class LocalizationService : ILocalizationService
    {
        private const string AssetPath = "Localization";

        public event Action OnChanged;
        
        private Dictionary<string, LocalizationInfo> _localizationInfos = new(5);
        private List<LocalizationInfo> _localizationInfosList = new(5);

        private readonly IStorageService _storage;
        private readonly IAssetProvider _assetProvider;

        private string _currentLanguageCode;
        private bool _userChanged = false;
        private readonly ILocalizationProvider _provider;

        public string CurrentLanguageCode => _currentLanguageCode;
        public bool UserChanged => _userChanged;
        public event Action<string> OnLanguageChanged;
        public IReadOnlyList<LocalizationInfo> Localizations => _localizationInfosList;

        public LocalizationService(IAssetProvider assetProvider, IStorageService storage, ILocalizationProvider provider)
        {
            _assetProvider = assetProvider;
            _storage = storage;
            _provider = provider;

            _storage.Register<ILocalizationService>(
                new StorableData<ILocalizationService>(this, new LocalizationStorageData()));
        }

        public bool SetCurrentLanguage(string languageCode, bool save = true)
        {
            if (string.IsNullOrEmpty(languageCode)) return false;

            if (_localizationInfos.ContainsKey(languageCode) == false)
            {
                return false;
            }

            if (_currentLanguageCode == languageCode)
            {
                return false;
            }

            Localization.ChangeLanguage(languageCode);
            _currentLanguageCode = languageCode;
            OnLanguageChanged?.Invoke(_currentLanguageCode);

            if (save)
            {
                _userChanged = true;
                _storage.Save<ILocalizationService>();
            }
            
            return true;
        }

        public LocalizationInfo GetCurrentLocalization()
        {
            return _localizationInfos.GetValueOrDefault(_currentLanguageCode);
        }

        public string GetValue(string key) => string.IsNullOrEmpty(key) ? key : Localization.GetValue(key);
        public void RegisterTMPText(string id, TMP_Text text)
        {
            Localization.AddTMPText(id, text);
        }

        public void UnregisterTMPText(string id, TMP_Text text)
        {
            Localization.RemoveTMPText(id, text);
        }

        public async Task LoadData()
        {
            var assets = await _assetProvider.LoadAll<LocalizationAsset>(AssetPath);

            if(assets.Count == 0)
                return;
            
            var asset = assets[0];
            
            foreach (var localization in asset.Localizations)
            {
                _localizationInfosList.Add(localization);
                _localizationInfos.Add(localization.LanguageCode, localization);
            }

            var bundles = await _assetProvider.LoadAll<LocalizationDatabase>(AssetPath);
            foreach (var bundle in bundles)
            {
                Localization.AddDatabase(bundle);
            }

            if (_provider != null)
            {
                await _provider.Initialize();
                
                SetCurrentLanguage(_provider.CurrentLanguageCode, false);
            }
            else
            {
                SetCurrentLanguage(asset.BaseLocalizationCode, false);
            }
        }

        public async void Load(IStorage data)
        {
            var localizationStorageData = (LocalizationStorageData)data;

            _userChanged = localizationStorageData.UserChanged;
            
            if (_userChanged == false && _provider != null)
            {
                await _provider.Initialize();
                
                SetCurrentLanguage(_provider.CurrentLanguageCode, false);
            }
            else
            {
                SetCurrentLanguage(localizationStorageData.CurrentLocalizationCode, false);
            }
            
            OnChanged?.Invoke();
        }

        public string ToStorage()
        {
            var storableData = _storage.Get<ILocalizationService>();
            return storableData.Storage.ToData(this);
        }
    }
}