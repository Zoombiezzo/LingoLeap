using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    [Serializable]
    public class SpritePreset : ISpritePreset
    {
        [HorizontalGroup("main")] [LabelWidth(50)] [SerializeField] 
        private string _id;
        
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [HorizontalGroup("main", 50)]
        [HideLabel]
        [SerializeField]
        private Sprite _sprite;

        public string Id => _id;
        public Sprite Sprite => _sprite;
        
        public SpritePreset(string id, Sprite sprite)
        {
            _id = id;
            _sprite = sprite;
        }

    }
}