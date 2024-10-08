using System;
using _Client.Scripts.Infrastructure.Services.SaveService;
using R3;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.PlayerProgress
{
    [Serializable]
    public class PlayerProgressStorage : IStorage
    {
        public SerializableReactiveProperty<int> Soft = new(0);
        public SerializableReactiveProperty<int> BoosterSelectChar = new(0);
        public SerializableReactiveProperty<int> BoosterSelectWord = new(0);
        
        public SerializableReactiveProperty<int> MindScore = new(0);
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}