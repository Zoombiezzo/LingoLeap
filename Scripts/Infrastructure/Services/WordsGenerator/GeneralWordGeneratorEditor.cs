#if UNITY_EDITOR
using System.Collections.Generic;
using System.Data.Common;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.WordsGenerator
{
    [System.Serializable]
    public class GeneralWordGeneratorEditor
    {
        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [SerializeField] [LabelText("Минимальное количество символов")]
        private int _minLength;

        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [SerializeField] [LabelText("Максимальное количество символов")]
        private int _maxLength;

        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [SerializeField] [LabelText("Содержит символы")]
        private string _containsChars;

        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [SerializeField] [LabelText("Подходящие слова")]
        private List<string> _suitableWords;
        
        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [SerializeField] [LabelText("Выбранное слово")]
        private string _selectedWord;
        
        private int _currentIndex = 0;

        private WordsGeneratorEditor _wordsGeneratorEditor;
        
        public string SelectedWord => _selectedWord;

        public GeneralWordGeneratorEditor(WordsGeneratorEditor wordsGeneratorEditor)
        {
            _wordsGeneratorEditor = wordsGeneratorEditor;
        }
        
        public void SetWordsGeneratorEditor(WordsGeneratorEditor wordsGeneratorEditor)
        {
            _wordsGeneratorEditor = wordsGeneratorEditor;
        }
        
        [HorizontalGroup("ШАГ 1 (Генерация основного слова)/Select")] [Button("Предыдущее")] [ShowIf("SuitableWordsContains")]
        private void SelectPrevious()
        {
            if(_suitableWords.Count == 0)
                return;
            
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _suitableWords.Count - 1;
            
            _selectedWord = _suitableWords[_currentIndex];
        }
        
        [HorizontalGroup("ШАГ 1 (Генерация основного слова)/Select")] [Button("Следующее")] [ShowIf("SuitableWordsContains")]
        private void SelectNext()
        {
            if(_suitableWords.Count == 0)
                return;
            
            _currentIndex++;
            if (_currentIndex >= _suitableWords.Count)
                _currentIndex = 0;
            
            _selectedWord = _suitableWords[_currentIndex];
        }

        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [Button("Генерировать")]
        private void Generate()
        {
            var query = new WordQueryBuilder().Clear()
                .WithLanguage(_wordsGeneratorEditor.Language)
                .WithMinLength(_minLength)
                .WithMaxLength(_maxLength)
                .WithContainsChars(_containsChars);

            var wordSelector = new WordsDatabaseService(_wordsGeneratorEditor.DBPath, _wordsGeneratorEditor.Language);

            try
            {
                _suitableWords = wordSelector.QueryScalars<string>(query);

                if (_suitableWords.Count > 0)
                {
                    _currentIndex = Random.Range(0, _suitableWords.Count);
                    _selectedWord = _suitableWords[_currentIndex];
                }
            }
            catch (DbException e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        [FoldoutGroup("ШАГ 1 (Генерация основного слова)")] [Button("Очистить слова")]
        public void Clear()
        {
            _suitableWords.Clear();
            _currentIndex = 0;
            _selectedWord = string.Empty;
        }

        private bool SuitableWordsContains() => _suitableWords.Count > 0;
    }
}
#endif