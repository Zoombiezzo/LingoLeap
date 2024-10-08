using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService
{
    public class AddressablesAssetProvider : IAssetProvider
    {
        private readonly IAddressablesService _addressablesService;

        public AddressablesAssetProvider(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }
        
        public GameObject Load(string path)
        {
            return null;
        }

        public Task<T> Load<T>(string path) where T : Object
        {
            return _addressablesService.Load<T>(path);
        }

        public Task<List<T>> LoadAll<T>(string path) where T : Object
        {
            try
            {
                var data = _addressablesService.LoadAll<T>(path);
                return data;
            }
            catch(Exception e)
            {
                Debugger.LogError(e.Message);
                return Task.FromResult(new List<T>());
            }
        }
    }
}