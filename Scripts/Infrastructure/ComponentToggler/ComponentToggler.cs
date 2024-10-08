using System.Collections.Generic;
using System.Threading.Tasks;

namespace _Client.Scripts.Infrastructure.ComponentToggler
{
    public class ComponentToggler : ToggleComponent
    {
        private readonly LinkedList<ToggleComponent> _togglesList = new();
        private readonly Dictionary<ToggleComponent, LinkedListNode<ToggleComponent>> _togglesMap = new(16);
        
        public override async Task Enable()
        {
            await base.Enable();
            
            foreach (var toggleComponent in _togglesList)
            {
                await toggleComponent.Enable();
            }
        }

        public override async Task Disable()
        {
            await base.Disable();
            
            foreach (var toggleComponent in _togglesList)
            {
                await toggleComponent.Disable();
            }
        }

        internal void TryRegister(ToggleComponent toggleComponent)
        {
            if(_togglesMap.ContainsKey(toggleComponent))
                return;
            
            var node = _togglesList.AddLast(toggleComponent);
            _togglesMap.Add(toggleComponent, node);

            if (IsEnable == toggleComponent.IsEnable)
                return;
            
            if(IsEnable)
                toggleComponent.Enable();
            else
                toggleComponent.Disable();
        }
        
        internal void TryUnregister(ToggleComponent toggleComponent)
        {
            if(_togglesMap.TryGetValue(toggleComponent, out var node) == false)
                return;

            toggleComponent.Disable();
            
            _togglesList.Remove(node);
            _togglesMap.Remove(toggleComponent);
        }
    }
}