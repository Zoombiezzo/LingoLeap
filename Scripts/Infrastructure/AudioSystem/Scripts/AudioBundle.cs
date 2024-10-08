using System.Collections.Generic;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    [CreateAssetMenu(menuName = "Audio Service/Create Audio Bundle", fileName = "Audio Bundle", order = 0)]
    [System.Serializable]
    public class AudioBundle : ScriptableObject
    {
        [SerializeField]
        private AudioInfo[] _infos;
        
        public IReadOnlyList<AudioInfo> Infos => _infos;
    }
}