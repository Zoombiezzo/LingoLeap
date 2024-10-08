using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Tools.Animation;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.ComponentToggler
{
    public class UIAnimationToggleComponent : ToggleComponent
    {
        [SerializeField] private List<UiAnimation> _animations;

        public override async Task Enable()
        {
            await base.Enable();
            
            foreach (var uiAnimation in _animations)
            {
                uiAnimation.Play();
                uiAnimation.enabled = true;

                await Task.Yield();
            }
        }

        public override async Task Disable()
        {
            await base.Disable();
            
            foreach (var uiAnimation in _animations)
            {
                uiAnimation.Stop();
                uiAnimation.enabled = false;

                await Task.Yield();
            }
        }
    }
}