using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.ComponentToggler
{
    public class ParticleImageToggleComponent : ToggleComponent
    {
        [SerializeField] private List<ParticleImage> _particles;

        public override async Task Enable()
        {
            await base.Enable();

            foreach (var particle in _particles)
            {
                particle.enabled = true;
                particle.Play();

                await Task.Yield();
            }
        }

        public override async Task Disable()
        {
            await base.Disable();

            foreach (var particle in _particles)
            {
                particle.Stop();
                particle.Clear();

                particle.enabled = false;
                
                await Task.Yield();
            }
        }

        [Button]
        private void CollectAllInChildren()
        {
            _particles = new List<ParticleImage>(GetComponentsInChildren<ParticleImage>(false));
        }

        private void Reset()
        {
            CollectAllInChildren();
        }
    }
}