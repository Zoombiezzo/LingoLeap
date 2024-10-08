using _Client.Scripts.Infrastructure.Services.ConfigData;

namespace _Client.Scripts.Infrastructure.Services.AdditionalWordsService
{
    public interface IAdditionalWordsService : IConfigData
    {
        bool TryGetLevelInfo(int level, out IAdditionalWordsLevelInfo levelInfo);
    }
}