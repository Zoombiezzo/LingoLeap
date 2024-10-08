using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    [Serializable]
    public class TutorialData : ITutorialData
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private bool _isCompleted = false;
        [SerializeField] private List<string> _completedSteps = new(4);
        private HashSet<string> _completedStepsSet => new(4);
        
        public string Id => _id;
        public bool IsCompleted => _isCompleted;

        public TutorialData()
        {
            _completedSteps.Clear();
            _completedStepsSet.Clear();

            foreach (var completedStep in _completedSteps) 
                _completedStepsSet.Add(completedStep);
        }

        public TutorialData(string id) : base()
        {
            _id = id;
            _isCompleted = false;
        }
        
        public void CompleteStep(string id)
        {
            if(_completedStepsSet.Add(id) == false)
                return;

            _completedSteps.Add(id);
        }

        public void CompleteTutorial() => _isCompleted = true;

        public bool IsStepCompleted(string id) => _completedStepsSet.Contains(id);

        public void Reset()
        {
            _isCompleted = false;
            _completedSteps.Clear();
            _completedStepsSet.Clear();
        }
    }
}