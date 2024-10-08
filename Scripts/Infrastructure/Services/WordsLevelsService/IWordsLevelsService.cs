using _Client.Scripts.Infrastructure.Services.ConfigData;

namespace _Client.Scripts.Infrastructure.Services.WordsLevelsService
{
    public interface IWordsLevelsService : IConfigData
    {
        bool TryGetLevel(int level, out WordsLevel wordsLevel);
        int GetCountLevels();
    }
}