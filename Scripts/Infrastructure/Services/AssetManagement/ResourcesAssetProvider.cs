using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AssetManagement
{
    public class ResourcesAssetProvider : IAssetProvider
    {
        public GameObject Load(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        public Task<T> Load<T>(string path) where T : Object
        {
            return Task.FromResult(Resources.Load<T>(path));
        }

        public Task<List<T>> LoadAll<T>(string path) where T : Object
        {
            return Task.FromResult(new List<T>(Resources.LoadAll<T>(path)));
        }
    }
}