using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.SaveService.StorageVariants
{
    public interface IStorageVariantService : IService
    {
        Task<bool> Save(string key, byte[] value);
        Task<(bool, byte[])> Load(string key);
        void Remove(string key);
        void Clear();
    }
}