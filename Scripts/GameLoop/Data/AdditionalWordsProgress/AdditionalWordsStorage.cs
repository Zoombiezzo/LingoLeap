using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.AdditionalWordsProgress
{
    [Serializable]
    public class AdditionalWordsStorage : IStorage
    {
        public int ProgressLevel = 0;
        public int ProgressWordsCount = 0;
        public List<LanguageAdditionalWordsRecord> LanguageAdditionalWordsRecords = new();
        public int Version => 0;

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);

    }
}