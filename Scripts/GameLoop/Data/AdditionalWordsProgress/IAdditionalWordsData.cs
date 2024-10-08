using System;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.GameLoop.Data.AdditionalWordsProgress
{
    public interface IAdditionalWordsData : IStorable
    {
        event Action<int> OnWordsChanged;
        event Action<string> OnWordOpened;
        public int GetCurrentProgressLevel();
        public int GetCurrentProgressWords();
        public void InitializeLevel();
        public ILanguageAdditionalWordsRecord GetLevelRecord();
        public void OpenWord(string word);
        public bool IsWordOpened(string word);
        public void SetCurrentLevel(int level, int wordsCount);
        public void ClearWords();
    }
}