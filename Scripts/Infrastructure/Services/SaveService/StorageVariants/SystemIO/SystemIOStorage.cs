using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.SystemIO
{
    public class SystemIOStorage : IStorageVariantService
    {
        private string DataPath => Application.persistentDataPath;
        
        public Task<bool> Save(string key, byte[] value)
        {
            File.WriteAllBytes(GetPath(key),value);
            return Task.FromResult(true);
        }

        public Task<(bool, byte[])> Load(string key)
        {
            if (HasKey(key) == false) return Task.FromResult<(bool, byte[])>((false, null));
            
            var value = File.ReadAllBytes(GetPath(key));
            

            return Task.FromResult((true, value));
        }

        public void Remove(string key)
        {
            File.Delete(GetPath(key));
        }

        public void Clear()
        {
            
        }

        private string GetPath(string key)
        {
            return Path.Combine(DataPath, $"{key}.data");
        }
        
        private bool HasKey(string key)
        {
            return File.Exists(GetPath(key));
        }
    }
}