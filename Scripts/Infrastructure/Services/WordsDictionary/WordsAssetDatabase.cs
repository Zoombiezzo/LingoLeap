using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Client.Scripts.Infrastructure.Services.WordsDictionary
{
    [Serializable]
    public class WordsAssetDatabase : AssetReferenceT<TextAsset>
    {
        public WordsAssetDatabase(string guid) : base(guid)
        {
        }
    }
}