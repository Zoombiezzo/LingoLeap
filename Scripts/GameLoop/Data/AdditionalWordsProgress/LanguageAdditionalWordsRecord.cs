using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.AdditionalWordsProgress
{
    [Serializable]
    public class LanguageAdditionalWordsRecord : ILanguageAdditionalWordsRecord
    {
        [SerializeField] private string _language;
        [SerializeField] private List<string> _openedWords = new();
        
        public string Language => _language;
        public List<string> OpenedWords => _openedWords;
        
        public LanguageAdditionalWordsRecord(string language)
        {
            _language = language;
            _openedWords = new List<string>();
        }
    }
}