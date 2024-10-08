using _Client.Scripts.Infrastructure.Services.ConfigData;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    public interface IWordsDictionaryService : IConfigData
    {
        bool Contains(string word);
    }
}