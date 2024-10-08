using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.TutorialService
{
    public abstract class MonoTutorialStep : MonoBehaviour, ITutorialStep
    {
        [SerializeField] private string _id;

        public virtual string Id => _id;
        public virtual bool IsComplete { get; protected set; }
        public virtual bool IsPossibleStart { get; protected set; }
        
        public virtual Task Initialize(IInitializeData data = null) => Task.CompletedTask;

        public virtual Task Create() => Task.CompletedTask;

        public virtual Task Show() => Task.CompletedTask;

        public virtual Task Hide() => Task.CompletedTask;
        public void Delete() => Destroy(gameObject);

        public virtual void Dispose() {}
    }
}