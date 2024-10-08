#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Client.Scripts.Infrastructure.Services.WordsDictionary;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Client.Scripts.Infrastructure.Services.WordsGenerator
{
    [System.Serializable]
    public class SecondaryWordsGeneratorEditor
    {
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")] [SerializeField] [LabelText("Максимальное количество слов")]
        private int _maxCountWords;
        
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")] [SerializeField] [LabelText("Не пересоздавать предыдущую генерацию")]
        private bool _cachingLastGenerationWords;
        
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")] [SerializeField] [LabelText("Добавить основное слово в результат")]
        private bool _addMainWordInResult;
        
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")] [SerializeField] [LabelText("Настройки генерации слов")]
        private WordsSettings[] _settings;
        
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")] [SerializeField] [LabelText("Результат")] 
        [TextArea(1, 10)] [ShowIf("ResultWordsIsNotEmpty")]
        private string _resultWords;

        [SerializeField]
        [ReadOnly]
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")]
        [LabelText("Последняя генерированная строка")]
        private string _lastGenerationWord;
        
        private WordsGeneratorEditor _wordsGeneratorEditor;
        
        private HashSet<string> _resultWordsHash = new();
        private EditorCoroutine _editorCoroutine;

        private bool ResultWordsIsNotEmpty => _resultWordsHash is { Count: > 0 };
        
        

        public SecondaryWordsGeneratorEditor(WordsGeneratorEditor wordsGeneratorEditor)
        {
            _wordsGeneratorEditor = wordsGeneratorEditor;
        }
        
        public void SetWordsGeneratorEditor(WordsGeneratorEditor wordsGeneratorEditor)
        {
            _wordsGeneratorEditor = wordsGeneratorEditor;
        }

        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")]
        [Button("Нормализовать шансы")]
        public void Normalize()
        {
            var sum = 0f;
            foreach (var setting in _settings)
            {
                sum += setting.Percentage;
            }
            
            foreach (var setting in _settings)
            {
                var percentage = setting.Percentage / sum;
                setting.SetPercentage(percentage);
            }
        }
        
        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")]
        [Button("Уравнять шансы")]
        public void EqualPercentage()
        {
            var percentage = 1f / _settings.Length;
            
            foreach (var setting in _settings)
            {
                setting.SetPercentage(percentage);
            }
        }

        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")]
        [Button("Генерировать")]
        public void Generate()
        {
            if (_editorCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_editorCoroutine);
            }
            
            _editorCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(GenerateImpl());
        }

        private IEnumerator GenerateImpl()
        {
            var mainWord = _wordsGeneratorEditor.GeneralWordGenerator.SelectedWord;

            _resultWordsHash ??= new(_maxCountWords);
            _resultWordsHash.Clear();


            if (_addMainWordInResult)
            {
                _resultWordsHash.Add(mainWord);
            }

            if (_cachingLastGenerationWords && mainWord.Equals(_lastGenerationWord))
            {
                GenerateExistWords();
                yield break;
            }

            var query = new WordQueryBuilder().Clear();
            var wordSelector = new WordsDatabaseService(_wordsGeneratorEditor.DBPath, _wordsGeneratorEditor.Language);

            for (var index = 0; index < _settings.Length; index++)
            {
                var setting = _settings[index];
                var length = setting.Length;
                //var permutations = GetKCombinations(mainWord, length);
                var hashWords = new HashSet<string>();

                query.Clear()
                    .WithLanguage(_wordsGeneratorEditor.Language)
                    .WithMinLength(setting.Length)
                    .WithMaxLength(setting.Length)
                    .WithContainsChars(mainWord)
                    .OnlyContainsChars()
                    .OnlyContainsCharsCount()
                    .UseOrStatementChars();

                try
                {
                    var suitableWords = wordSelector.QueryScalars<string>(query);

                    foreach (var word in suitableWords)
                    {
                        //if (CheckWordsFit(mainWord, word))
                        hashWords.Add(word);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }

                setting.SetWords(hashWords.ToList());

                var maxWords = Mathf.Min(Mathf.CeilToInt(_maxCountWords * setting.Percentage),
                    Mathf.Max(_maxCountWords - _resultWordsHash.Count, 0));

                var words = setting.Words.ToList();

                var numberWord = 0;

                for (var i = 0; i < maxWords; i++)
                {
                    if (words.Count == 0)
                        break;

                    if (setting.IsRandom)
                    {
                        numberWord = Random.Range(0, words.Count);
                    }

                    var word = words[numberWord];

                    if (_resultWordsHash.Contains(word) == false)
                    {
                        _resultWordsHash.Add(words[numberWord]);
                    }
                    else
                    {
                        i--;
                    }

                    words.RemoveAt(numberWord);
                }
            }

            if (_resultWordsHash.Count < _maxCountWords)
            {
                var isAll = new List<bool>(_settings.Length);

                for (var i = 0; i < _settings.Length; i++)
                {
                    isAll.Add(false);
                }

                while (_resultWordsHash.Count < _maxCountWords)
                {
                    for (var i = 0; i < _settings.Length; i++)
                    {
                        if (isAll[i])
                            continue;

                        if (_resultWordsHash.Count == _maxCountWords)
                            break;

                        var isAdded = false;

                        foreach (var word in _settings[i].Words)
                        {
                            if (_resultWordsHash.Contains(word) == false)
                            {
                                isAdded = true;
                                _resultWordsHash.Add(word);
                                break;
                            }
                        }

                        if (isAdded == false)
                        {
                            isAll[i] = true;
                        }
                    }

                    if (isAll.All(x => x))
                        break;
                }
            }

            _resultWords = string.Join(",", _resultWordsHash);

            _lastGenerationWord = mainWord;

            yield return null;
        }

        private void GenerateExistWords()
        {
            foreach (var setting in _settings)
            {
                var maxWords = Mathf.Min(Mathf.CeilToInt(_maxCountWords * setting.Percentage),
                    Mathf.Max(_maxCountWords - _resultWordsHash.Count, 0));

                var words = setting.Words.ToList();

                var numberWord = 0;

                for (var i = 0; i < maxWords; i++)
                {
                    if (words.Count == 0)
                        break;

                    if (setting.IsRandom)
                    {
                        numberWord = Random.Range(0, words.Count);
                    }

                    var word = words[numberWord];

                    if (_resultWordsHash.Contains(word) == false)
                    {
                        _resultWordsHash.Add(words[numberWord]);
                    }
                    else
                    {
                        i--;
                    }

                    words.RemoveAt(numberWord);
                }
            }
            
            if (_resultWordsHash.Count < _maxCountWords)
            {
                var isAll = new List<bool>(_settings.Length);
                
                for (var i = 0; i < _settings.Length; i++)
                {
                    isAll.Add(false);
                }
                
                while (_resultWordsHash.Count < _maxCountWords)
                {
                    for (var i = 0; i < _settings.Length; i++)
                    {
                        if (isAll[i])
                            continue;
                        
                        if (_resultWordsHash.Count == _maxCountWords)
                            break;

                        var isAdded = false;
                        
                        foreach (var word in _settings[i].Words)
                        {
                            if (_resultWordsHash.Contains(word) == false)
                            {
                                isAdded = true;
                                _resultWordsHash.Add(word);
                                break;
                            }
                        }

                        if (isAdded == false)
                        {
                            isAll[i] = true;
                        }
                    }

                    if (isAll.All(x => x))
                        break;
                }
            }
            
            _resultWords = string.Join(",", _resultWordsHash);
        }

        private bool CheckWordsFit(string mainWord, string word)
        {
            foreach (var c in mainWord)
            {
                var index = word.IndexOf(c);

                if (index == -1)
                    continue;
                
                word = word.Remove(index, 1);

                if (word.Length == 0)
                    return true;
            }
            
            return false;
        }

        [FoldoutGroup("ШАГ 2 (Генерация дополнительных слов)")]
        [Button("Очистить")]
        public void Clear()
        {
            _resultWords = string.Empty;
            _resultWordsHash?.Clear();
            _lastGenerationWord = string.Empty;
        }
        
        public static List<string> GetKCombinations(string str, int k)
        {
            var allCombinations = new List<string>();
            Combine(str, 0, k, "", allCombinations);
            return allCombinations;
        }

        private static void Combine(string str, int start, int k, string current, List<string> allCombinations)
        {
            if (current.Length == k)
            {
                allCombinations.Add(current);
                return;
            }

            for (int i = start; i < str.Length; ++i)
            {
                current += str[i];
                Combine(str, i + 1, k, current, allCombinations);
                current = current.Substring(0, current.Length - 1);
            }
        }
    }
}
#endif