using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    [Serializable]
    public class LanguageLevelRecord : ILevelRecord
    {
        [SerializeField] private int _levelNumber;
        [SerializeField] private string _language;
        [SerializeField] private List<OpenedChar> _openedChars = new();
        [SerializeField] private List<string> _openedWords = new();

        private Dictionary<string, OpenedChar> _openedCharsMap = new(8);
        private HashSet<string> _openedWordsHashSet = new(8);
        
        public LanguageLevelRecord()
        {
            _openedChars = new(8);
            _openedWords = new(8);
            _language = string.Empty;
            _levelNumber = 0;
        }
        public LanguageLevelRecord(int levelNumber, string language)
        {
            _levelNumber = levelNumber;
            _language = language;
            _openedChars = new(8);
            _openedWords = new(8);
        }

        public int LevelNumber
        {
            get => _levelNumber;
            internal set => _levelNumber = value;
        }

        public string Language
        {
            get => _language;
            internal set => _language = value;
        }

        public List<OpenedChar> OpenedChars
        {
            get => _openedChars;
            internal set => _openedChars = value;
        }

        public List<string> OpenedWords
        {
            get => _openedWords;
            internal set => _openedWords = value;
        }

        public void AddOpenedWord(string word)
        {
            if (_openedWords.Contains(word))
                return;
            
            _openedWords.Add(word);
        }
        
        public bool IsWordOpened(string word) => _openedWordsHashSet.Contains(word);
        
        public void AddOpenedChar(string word, int index)
        {
            if (_openedCharsMap.TryGetValue(word, out var openedChar) == false)
            {
                foreach (var charsWord in _openedChars)
                {
                    if (charsWord.Word != word) continue;
                    openedChar = charsWord;
                    break;
                }
                
                if (openedChar == null)
                {
                    openedChar = new OpenedChar(word);
                    _openedChars.Add(openedChar);
                }

                _openedCharsMap.Add(word, openedChar);
            }
            
            openedChar.AddCharIndex(index);
        }

        public bool IsOpenedAllChars(string word)
        {
            if (_openedCharsMap.TryGetValue(word, out var openedChar) == false)
                return false;
            
            return openedChar.IsOpenAllChars();
        }
    }
}