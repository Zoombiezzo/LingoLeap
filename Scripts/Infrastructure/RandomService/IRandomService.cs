using _Client.Scripts.Infrastructure.Services;

namespace _Client.Scripts.Infrastructure.RandomService
{
    public interface IRandomService : IService
    {
        int Range(int min, int max);

        float Range(float min, float max);
    }
}