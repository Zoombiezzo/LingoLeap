using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using Newtonsoft.Json;

namespace _Client.Scripts.Infrastructure.Services.LimitationService.Data
{
    [Serializable]
    public class LimitationStorageData : ILimitationStorageData
    {
       [JsonProperty] private List<LimitationRecord> _limitationRecords = new();
        
        [JsonIgnore] public List<LimitationRecord> LimitationRecords => _limitationRecords;
        
        [JsonIgnore] private JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            var deserializeObject = JsonConvert.DeserializeObject<LimitationStorageData>(data, _settings);
            return deserializeObject;
        }

        public string ToData(IStorable data)
        {
            var limitationService = (LimitationService)data;
            _limitationRecords = limitationService.LimitationRecordsSavable;
            return JsonConvert.SerializeObject(this, _settings);
        }
    }

    public interface ILimitationStorageData : IStorage
    {
        List<LimitationRecord> LimitationRecords { get; }
    }
}