using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.RequirementService.Requirements.LevelCompletedRequirements
{
    [HideLabel]
    [Serializable]
    public class LevelNeedRequirement : IRequirement
    {
        [FoldoutGroup("@RequirementName")] [SerializeField]
        private int _level;
        
        [FoldoutGroup("@RequirementName")] [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _name;

        public int Level => _level;
        public string Name => _name;
        
#if UNITY_EDITOR
        public string RequirementName => "Ограничение по пройденным уровням";
#endif
    }
}