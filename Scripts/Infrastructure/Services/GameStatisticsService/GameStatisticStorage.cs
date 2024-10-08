using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.GameStatisticsService
{
    [Serializable]
    public class GameStatisticStorage : IStorage
    {
        [JsonProperty] public List<IStatisticRecord> Statistics = new();
        
        [JsonIgnore] private JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            var deserializeObject = JsonConvert.DeserializeObject<GameStatisticStorage>(data, _settings);
            Statistics = deserializeObject.Statistics;
            return this;
        }

        public string ToData(IStorable data) => JsonConvert.SerializeObject(this, _settings);
    }
}