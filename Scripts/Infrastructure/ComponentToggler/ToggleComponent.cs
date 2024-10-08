using System.Threading.Tasks;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.ComponentToggler
{
    public abstract class ToggleComponent : MonoBehaviour
    {
        [SerializeField] private ComponentToggler _rootToggler;
        
        protected bool _isEnable = false;
        
        public bool IsEnable => _isEnable;
        
        private void Awake()
        {
            _rootToggler = GetComponentInParent<ComponentToggler>();
            
            if(_rootToggler == null || _rootToggler == this)
                return;
            
            _rootToggler.TryRegister(this);
        }

        public virtual Task Enable()
        {
            _isEnable = true;
            return Task.CompletedTask;
        }

        public virtual Task Disable()
        {
            _isEnable = false;
            return Task.CompletedTask;
        }

        private void OnDestroy()
        {
            if (_rootToggler != null) 
                _rootToggler.TryUnregister(this);
        }
    }
}