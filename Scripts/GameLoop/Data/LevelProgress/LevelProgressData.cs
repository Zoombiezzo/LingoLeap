using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.LocalizationService;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    public class LevelProgressData : ILevelProgressData
    {
        private LevelProgressStorage _storage;
        private readonly IStorageService _storageService;
        private readonly ILocalizationService _localizationService;
        private readonly Dictionary<string, LanguageLevelRecord> _levelRecords;

        private readonly Dictionary<string, HashSet<int>> _openedChars = new(8);
        private HashSet<string> _openedWords = new(8);
        private ILevelRecord _currentLevelRecord;

        public event Action OnCurrentLevelChanged;
        public event Action<int> OnLevelCompleted;
        public event Action<string> OnWordOpened;

        public LevelProgressData(IStorageService storageService, ILocalizationService localizationService)
        {
            _storageService = storageService;
            _localizationService = localizationService;
            _storage = new LevelProgressStorage();
            _levelRecords = new Dictionary<string, LanguageLevelRecord>(8);
            _storageService.Register<ILevelProgressData>(new StorableData<ILevelProgressData>(this, _storage));
        }

        public int GetCurrentLevel()
        {
            var levelRecord = (LanguageLevelRecord)GetLevelRecord();
            return levelRecord.LevelNumber;
        }

        public void ResetLevel()
        {
          var levelRecord = (LanguageLevelRecord)GetLevelRecord();
          levelRecord.OpenedChars.Clear();
          levelRecord.OpenedWords.Clear();
          
          _openedChars.Clear();
          _openedWords.Clear();
          
          _storageService.Save<ILevelProgressData>();
        }

        public ILevelRecord GetLevelRecord()
        {
            var language = _localizationService.CurrentLanguageCode;
            
            if (_levelRecords.TryGetValue(language, out var levelRecord) == false)
            {
                levelRecord = new LanguageLevelRecord(0, language);
                _storage.LanguageLevelRecords.Add(levelRecord);
                _levelRecords.TryAdd(language, levelRecord);
                _storageService.Save<ILevelProgressData>();
            }

            if (_currentLevelRecord == levelRecord)
                return _currentLevelRecord;

            _openedChars.Clear();
            _openedWords.Clear();
            
            foreach (var openedChar in levelRecord.OpenedChars)
            {
                if (_openedChars.TryGetValue(openedChar.Word, out var openedChars) == false)
                {
                    openedChars = new HashSet<int>(8);
                    _openedChars.Add(openedChar.Word, openedChars);
                }
                
                foreach (var index in openedChar.Indexes)
                {
                    openedChars.Add(index);
                }
            }
            
            foreach (var openedWord in levelRecord.OpenedWords)
            {
                _openedWords.Add(openedWord);
            }

            _currentLevelRecord = levelRecord;
            
            return levelRecord;
        }

        public void OpenWord(string word)
        {
            if(_openedWords.Contains(word))
                return;
            
            if(_currentLevelRecord == null)
                return;
            
            _currentLevelRecord.AddOpenedWord(word);
            
            _openedWords.Add(word);
            
            _storageService.Save<ILevelProgressData>();
            
            OnWordOpened?.Invoke(word);
        }

        public void OpenCharIndex(string word, int charIndex)
        {
            if(_currentLevelRecord == null)
                return;
            
            if(_openedChars.TryGetValue(word, out var openedChars))
            {
                if(openedChars.Add(charIndex) == false)
                    return;
            }
            
            _currentLevelRecord.AddOpenedChar(word, charIndex);

            if (_currentLevelRecord.IsOpenedAllChars(word))
            {
                if (_currentLevelRecord.IsWordOpened(word) == false)
                {
                    OpenWord(word);
                    return;
                }
            }
            
            _storageService.Save<ILevelProgressData>();
        }

        public bool IsWordOpened(string word) => _openedWords.Contains(word);

        public bool IsCharOpened(string word, int charIndex) =>
            _openedChars.TryGetValue(word, out var openedChars) && openedChars.Contains(charIndex);


        public void SetCurrentLevel(int levelNumber)
        {
            var levelRecord = (LanguageLevelRecord)GetLevelRecord();
            var changed = levelRecord.LevelNumber != levelNumber;
            
            levelRecord.LevelNumber = levelNumber;
            levelRecord.OpenedChars.Clear();
            levelRecord.OpenedWords.Clear();
            
            _openedChars.Clear();
            _openedWords.Clear();
            
            _storageService.Save<ILevelProgressData>();
            
            if (changed)
                OnCurrentLevelChanged?.Invoke();
        }

        public void CompleteLevel()
        {
            var levelRecord = (LanguageLevelRecord)GetLevelRecord();
            var levelNumber = levelRecord.LevelNumber;
            
            levelRecord.LevelNumber += 1;
            levelRecord.OpenedChars.Clear();
            levelRecord.OpenedWords.Clear();
            
            _openedChars.Clear();
            _openedWords.Clear();
            
            _storageService.Save<ILevelProgressData>();
            OnCurrentLevelChanged?.Invoke();
            OnLevelCompleted?.Invoke(levelNumber);
        }

        public void Load(IStorage data)
        {
            foreach (var levelRecord in _storage.LanguageLevelRecords) 
                _levelRecords.TryAdd(levelRecord.Language, levelRecord);
        }

        public string ToStorage() => _storage.ToData(this);
    }
}