using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants.LocalStorage
{
    public class LocalStorageWebGL : IStorageVariantService
    {
        public Task<bool> Save(string key, byte[] value)
        {

            if(value == null)
                return Task.FromResult(false);
            
            var base64Str = Convert.ToBase64String(value);


            SetItemLocalStorage(key, base64Str);
            return Task.FromResult(true);
        }

        public Task<(bool, byte[])> Load(string key)
        {
            if (HasKey(key) == false) return Task.FromResult((false, Array.Empty<byte>()));
            
            string value = GetItemLocalStorage(key);
            
            
            if (string.IsNullOrEmpty(value))
                return Task.FromResult((false, Array.Empty<byte>()));
            
            var bytes = Convert.FromBase64String(value);
            
            
            return Task.FromResult((true, bytes));
        }

        public void Remove(string key)
        {
            if(HasKey(key))
                RemoveItemLocalStorage(key);
        }

        public void Clear()
        {
            ClearLocalStorage();
        }

        private bool HasKey(string key)
        {
            return HasKeyLocalStorage(key) == 1;
        }
        
        [DllImport("__Internal")]
        private static extern void SetItemLocalStorage(string key, string value);
        [DllImport("__Internal")]
        private static extern string GetItemLocalStorage(string key);
        [DllImport("__Internal")]
        private static extern int HasKeyLocalStorage(string key);
        [DllImport("__Internal")]
        private static extern void RemoveItemLocalStorage(string key);
        [DllImport("__Internal")]
        private static extern void ClearLocalStorage();
    }
}