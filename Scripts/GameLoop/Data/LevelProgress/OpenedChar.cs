using System;
using System.Collections.Generic;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    [Serializable]
    public class OpenedChar
    {
        public string Word;
        public List<int> Indexes;
        
        public OpenedChar() { }
        
        public OpenedChar(string word)
        {
            Word = word;
            Indexes = new List<int>(word.Length);
        }

        public void AddCharIndex(int index)
        {
            if (Indexes.Contains(index) == false)
                Indexes.Add(index);
        }

        public bool IsOpenAllChars() => Indexes.Count == Word.Length;
    }
}