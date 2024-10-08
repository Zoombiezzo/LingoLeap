using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    [Serializable]
    public class PictureReference : AssetReferenceT<GameObject>
    {
        public PictureReference(string guid) : base(guid) { }
    }
}