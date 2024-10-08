using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.GameLoop.Data.AdditionalWordsProgress
{
    public class AdditionalWordsData : IAdditionalWordsData
    {
        private AdditionalWordsStorage _storage;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;
        private readonly Dictionary<string, LanguageAdditionalWordsRecord> _levelRecords;
        private readonly HashSet<string> _openedWords = new HashSet<string>();
        private ILanguageAdditionalWordsRecord _currentLevelRecord;

        public event Action<int> OnWordsChanged;
        public event Action<string> OnWordOpened;

        public AdditionalWordsData(IStorageService storageService, ILocalizationService localizationService)
        {
            _storageService = storageService;
            _localizationService = localizationService;
            _levelRecords = new Dictionary<string, LanguageAdditionalWordsRecord>();
            _storage = new AdditionalWordsStorage();
            _storageService.Register<IAdditionalWordsData>(new StorableData<IAdditionalWordsData>(this, _storage));
        }

        public int GetCurrentProgressLevel() => _storage.ProgressLevel;
        public int GetCurrentProgressWords() => _storage.ProgressWordsCount;
        
        public void InitializeLevel()
        {
            _currentLevelRecord = GetLevelRecord();
        }

        public ILanguageAdditionalWordsRecord GetLevelRecord()
        {
            var language = _localizationService.CurrentLanguageCode;
            _openedWords.Clear();
            
            if (_levelRecords.TryGetValue(language, out var levelRecord) == false)
            {
                levelRecord = new LanguageAdditionalWordsRecord(language);
                _storage.LanguageAdditionalWordsRecords.Add(levelRecord);
                _levelRecords.TryAdd(language, levelRecord);
                _storageService.Save<IAdditionalWordsData>();
            }

            foreach (var openedWord in levelRecord.OpenedWords)
            {
                _openedWords.Add(openedWord);
            }

            _currentLevelRecord = levelRecord;
            
            return levelRecord;
        }
        
        public bool IsWordOpened(string word) => _openedWords.Contains(word);
        public void SetCurrentLevel(int level, int wordsCount)
        {
            _storage.ProgressLevel = level;
            _storage.ProgressWordsCount = wordsCount;
            
            _storageService.Save<IAdditionalWordsData>();
            
            OnWordsChanged?.Invoke(_storage.ProgressWordsCount);
        }

        public void ClearWords()
        {
            _openedWords.Clear();
            var currentLevelRecord = GetLevelRecord();
            currentLevelRecord.OpenedWords.Clear();
            
            _storageService.Save<IAdditionalWordsData>();
        }

        public void OpenWord(string word)
        {
            if(_openedWords.Contains(word))
                return;
            
            _currentLevelRecord ??= GetLevelRecord();
            
            _storage.ProgressWordsCount++;
            _currentLevelRecord.OpenedWords.Add(word);
            _openedWords.Add(word);
            
            _storageService.Save<IAdditionalWordsData>();
            
            OnWordsChanged?.Invoke(_storage.ProgressWordsCount);
            OnWordOpened?.Invoke(word);
        }
        public void Load(IStorage data)
        {
            foreach (var levelRecord in _storage.LanguageAdditionalWordsRecords) 
                _levelRecords.TryAdd(levelRecord.Language, levelRecord);
        }

        public string ToStorage() => _storage.ToData(this);
    }
}