using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpinWheelService
{
    [Serializable]
    public class SpinWheelStorage : IStorage
    {
        public int Version => 0;
        [SerializeField] public int CurrentSpin = 0;
        [SerializeField] public long LastTimeUpdateTicks = 0;
        [SerializeField] public List<RegionRecord> Regions = new(8);

        private Dictionary<int, RegionRecord> _regionsMap = new(8);

        public void AddRegion(int index, int stage)
        {
            if(_regionsMap.ContainsKey(index))
                return;
            
            var region = new RegionRecord
            {
                Index = index,
                Stage = stage
            };
            
            Regions.Add(region);
            _regionsMap.Add(index, region);
        }

        public bool TryGetRegion(int index, out RegionRecord record) => _regionsMap.TryGetValue(index, out record);

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            
            foreach (var region in Regions)
            {
                _regionsMap.TryAdd(region.Index, region);
            }
            
            return this;
        }
        
        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}