using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public abstract class LocationPicture : MonoBehaviour
    {
        public virtual Task Show(bool animate = false)
        {
            return Task.CompletedTask;
        }
        
        public virtual Task Hide(bool animate = false)
        {
            return Task.CompletedTask;
        }
    }
}