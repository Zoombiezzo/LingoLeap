using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.AchievementsSystem
{
    [Serializable]
    public class AchievementStorage : IStorage
    {
        [JsonProperty] public List<AchievementRecord> Achievements = new();
        
        [JsonIgnore] private JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Error = (sender, args) =>
            {
                // Игнорировать ошибки, связанные с типами
                if (args.ErrorContext.Error is JsonSerializationException)
                {
                    args.ErrorContext.Handled = true; // Пометить ошибку как обработанную
                }
            }
        };
        
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            var deserializeObject = JsonConvert.DeserializeObject<AchievementStorage>(data, _settings);
            Achievements = deserializeObject.Achievements;
            return this;
        }

        public string ToData(IStorable data) => JsonConvert.SerializeObject(this, _settings);
    }
}