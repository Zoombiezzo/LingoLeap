using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public interface ILocationPictureFactory
    {
        Task<LocationPicture> Create(Transform parent, ILocationConfig config);
        void Release(ILocationConfig config);
    }
}