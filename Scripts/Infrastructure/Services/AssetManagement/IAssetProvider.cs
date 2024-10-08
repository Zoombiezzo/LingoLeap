using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.AssetManagement
{
    public interface IAssetProvider : IService
    {
        GameObject Load(string path);
        Task<T> Load<T>(string path) where T : Object;
        
        Task<List<T>> LoadAll<T>(string path) where T : Object;
    }
}