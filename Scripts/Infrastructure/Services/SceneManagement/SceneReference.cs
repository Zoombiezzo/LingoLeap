using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    [System.Serializable]
    public class SceneReference
    {
        [SerializeField] [ValueDropdown("GetSceneNames")] 
        private string _scene;
        [SerializeField]
        private LoadSceneMode _loadSceneMode;

        public string Scene => _scene;
        public LoadSceneMode LoadSceneMode => _loadSceneMode;

#if UNITY_EDITOR
        private List<string> GetSceneNames()
        {
            var sceneNames = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scene = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                sceneNames.Add(scene);
            }
            
            return sceneNames;
        }
#endif
    }
}