using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService
{
    public interface IAddressablesService : IService
    {
        IEnumerator Initialize(Action<float> onProgress = null);
        
        IEnumerator CheckAndUpdateBundles(Action<float> onProgress = null);

        Task<List<T>> LoadAll<T>(string label);
        Task<T> Load<T>(string path);
        
        Task<T> Load<T>(Object reference);
        void Release(Object reference);
    }
}