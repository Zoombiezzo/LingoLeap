using System;
using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    [Serializable]
    public class TutorialStorageData : IStorage
    {
        public int Version => 0;
        [SerializeField] public List<TutorialData> Tutorials = new(8);
        private readonly Dictionary<string, TutorialData> _tutorialMap = new(8);
        
        public bool ContainsTutorial(string id) => _tutorialMap.ContainsKey(id);
        
        public bool TryGetTutorial(string id, out TutorialData tutorialData) => _tutorialMap.TryGetValue(id, out tutorialData);

        public bool TryAddTutorial(TutorialData tutorialData)
        {
            if(_tutorialMap.TryAdd(tutorialData.Id, tutorialData) == false)
                return false;
            
            Tutorials.Add(tutorialData);

            return true;
        }

        public IStorage ToStorage(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
            
            foreach (var tutorialData in Tutorials)
            {
                _tutorialMap.TryAdd(tutorialData.Id, tutorialData);
            }
            
            return this;
        }

        public string ToData(IStorable data) => JsonUtility.ToJson(this);
    }
}