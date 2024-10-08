using System;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [Serializable]
    public abstract class AdaptiveUIOrientationState<T> where T : AdaptiveUIElement
    {
        [SerializeField] protected bool _isEnabled = true;
        [SerializeField] protected AdaptiveUIOrientation _orientation;
        
        public AdaptiveUIOrientation Orientation => _orientation;
        public abstract void Apply(T element);
    }
}