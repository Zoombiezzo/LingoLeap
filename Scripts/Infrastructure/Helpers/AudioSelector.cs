using System;
using System.Collections.Generic;
using System.Linq;
using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Helpers
{
    [Serializable]
    public class AudioSelector
    {
        [SerializeField] [ValueDropdown("AudioDropdown")] [HideLabel]
        private string _audio;
        
        public string Audio => _audio;
        
#if UNITY_EDITOR
        private IEnumerable<string> AudioDropdown()
        {
            return AssetHelper<AudioBundle>.GetAsset("Assets").Select(el => el.Infos).SelectMany(el => el)
                .Select(el => el.Id);
        }
#endif
    }
}