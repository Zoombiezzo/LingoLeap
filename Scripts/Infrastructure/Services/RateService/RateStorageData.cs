using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using Newtonsoft.Json;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.RateService
{
    [Serializable]
    public class RateStorageData : IStorage
    {
        [SerializeField] public bool Rated;
        [SerializeField] public bool RatedShowed;
        public int Version => 0;
        
        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}