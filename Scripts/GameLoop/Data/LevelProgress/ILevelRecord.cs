using System.Collections.Generic;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    public interface ILevelRecord
    {
        int LevelNumber { get; }
        string Language { get; }
        List<OpenedChar> OpenedChars { get; }
        List<string> OpenedWords { get; }
        public void AddOpenedChar(string word, int index);
        void AddOpenedWord(string word);
        bool IsOpenedAllChars(string word);
        bool IsWordOpened(string word);
    }
}