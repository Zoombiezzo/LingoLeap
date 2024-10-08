using System;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService.Statistics.LevelCompleteStatistic
{
    public class LevelCompleteStatisticRecord : StatisticRecord
    {
        [JsonProperty] private int _count;
        
        [JsonIgnore] public int Count => _count;
        
        public void Add(int count)
        {
            if (count <= 0)
            {
                throw new Exception("Count must be > 0");
            }
            
            _count += count;
        }
    }
}