using UnityEngine;

namespace _Client.Scripts.Infrastructure.AudioSystem.Scripts
{
    [System.Serializable]
    public class AudioInfo
    {
        [SerializeField] 
        private string _id;
        [SerializeField]
        private AudioClip _clip;
        
        public string Id => _id;
        public AudioClip Clip => _clip;
    }
}