using System;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    public interface ILevelProgressData : IStorable
    {
        event Action OnCurrentLevelChanged;
        event Action<int> OnLevelCompleted;
        event Action<string> OnWordOpened;
        void SetCurrentLevel(int levelNumber);  
        void CompleteLevel();  
        int GetCurrentLevel();  
        void ResetLevel();  
        ILevelRecord GetLevelRecord();
        void OpenCharIndex(string word, int charIndex);
        void OpenWord(string word);
        bool IsCharOpened(string word, int charIndex);
        bool IsWordOpened(string word);
    }
}