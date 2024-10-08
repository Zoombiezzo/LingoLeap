using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.LevelProgress
{
    [Serializable]
    public class LevelProgressStorage : IStorage
    {
        [SerializeField] public List<LanguageLevelRecord> LanguageLevelRecords = new();
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}