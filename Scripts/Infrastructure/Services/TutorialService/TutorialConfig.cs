using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Tutorial/Config", order = 0)]
    public class TutorialConfig : ScriptableObject, ITutorialConfig
    {
        [SerializeField]
        private string _id;
        
        [SerializeField]
        private string _tutorialExecutorId;
        
        [SerializeField]
        private List<MonoTutorialStep> _tutorialSteps;
        
        public string Id => _id;
        public string TutorialExecutorId => _tutorialExecutorId;
        public IReadOnlyList<ITutorialStep> Steps => _tutorialSteps;
    }
}