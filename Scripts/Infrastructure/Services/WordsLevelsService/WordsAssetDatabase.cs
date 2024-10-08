using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Client.Scripts.Infrastructure.Services.WordsLevelsService
{
    [Serializable]
    public class WordsLevelsAssetDatabase : AssetReferenceT<TextAsset>
    {
        public WordsLevelsAssetDatabase(string guid) : base(guid)
        {
        }
    }
}