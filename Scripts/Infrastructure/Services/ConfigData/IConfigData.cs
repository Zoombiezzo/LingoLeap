using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.Services.ConfigData
{
    public interface IConfigData : IService
    {
        Task LoadData();
    }
}