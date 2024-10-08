using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.WordsLevelsService
{
    public class WordsLevel
    {
        private int _level;
        private string _chars;
        private string[] _words;
        
        private readonly HashSet<string> _wordsSet = new(8);
        
        public int Level => _level;
        public string Chars => _chars;
        public string[] Words => _words;
        
        public WordsLevel(int level, string chars, string words)
        {
            _level = level;
            _chars = chars;
            var wordsArray = words.Split(",");
            _words = new string[wordsArray.Length];
            
            for (var i = 0; i < wordsArray.Length; i++)
            {
                var word = wordsArray[i].Trim();
                _words[i] = word;
                _wordsSet.Add(word);
            }
        }
        
        public bool IsWordContains(string word) => _wordsSet.Contains(word);
    }
}