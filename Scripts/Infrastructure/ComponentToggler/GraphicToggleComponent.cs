using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.ComponentToggler
{
    public class GraphicToggleComponent : ToggleComponent
    {
        [SerializeField] private List<Graphic> _renderer;

        public override async Task Enable()
        {
            await base.Enable();

            foreach (var render in _renderer)
            {
                render.enabled = true;
                await Task.Yield();
            }
        }

        public override async Task Disable()
        {
            await base.Disable();
            
            foreach (var render in _renderer)
            {
                render.enabled = false;
                await Task.Yield();
            }
        }
    }
}