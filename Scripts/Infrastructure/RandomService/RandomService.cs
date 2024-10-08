using System;

namespace _Client.Scripts.Infrastructure.RandomService
{
    public class RandomService : IRandomService
    {
        private readonly Random _random;

        public RandomService()
        {
            _random = new System.Random();
        }
        
        public int Range(int min, int max)
        {
            return _random.Next(min, max);
        }
        
        public float Range(float min, float max)
        {
            return (float)(_random.NextDouble() * (max - min) + min);
        }
    }
}