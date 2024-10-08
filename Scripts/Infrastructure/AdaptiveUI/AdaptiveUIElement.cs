using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    public abstract class AdaptiveUIElement : UIBehaviour
    {
        [SerializeField] [ReadOnly] protected AdaptiveUIOrientation _currentOrientation = AdaptiveUIOrientation.None;
        [SerializeField] [ReadOnly] protected Vector2 _currentSize = Vector2.zero;
        [SerializeField] protected bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;

        protected override void Awake()
        {
            base.Awake();
            _currentOrientation = AdaptiveUIOrientation.None;
            _currentSize = Vector2.zero;
            
            RegisterInGroup();
        }

        public virtual void ChangeOrientation(AdaptiveUIOrientation orientation, bool force = false)
        {
            _currentOrientation = orientation;
        }

        public virtual void ChangeSize(Vector2 size, bool force = false)
        {
            _currentSize = size;
        }
        
        private void RegisterInGroup()
        {
            var group = transform.GetComponentInParent<AdaptiveUIGroup>();
            group.TryRegister(this);
        }

#if UNITY_EDITOR
        
        [SerializeField] protected bool _editMode = true;

        protected override void Reset()
        {
            base.Reset();

            RegisterInGroup();
        }
        
        internal virtual void TryEdit(AdaptiveUIOrientation orientation)
        {
           return;
        }
        
        internal virtual void SavePreset(AdaptiveUIOrientation orientation)
        {
            return;
        }
#endif
    }
}