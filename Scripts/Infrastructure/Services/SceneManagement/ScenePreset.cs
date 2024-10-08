using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Service/Create ScenePreset", fileName = "ScenePreset", order = 0)]
    [System.Serializable]
    public class ScenePreset : ScriptableObject
    {
        [SerializeField]
        private string _id;
        
        [SerializeField]
        private List<SceneReference> _scenes;
        public string Id => _id;
        public IEnumerable<SceneReference> Scenes => _scenes;
        
    }
}