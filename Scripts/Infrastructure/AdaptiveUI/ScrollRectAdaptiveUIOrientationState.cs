using System;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [Serializable]
    public class ScrollRectAdaptiveUIOrientationState : AdaptiveUIOrientationState<AdaptiveUIScrollRect>
    {
        [SerializeField] [ReadOnly]
        private AdaptiveUIScrollRect _element;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool horizontal;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool vertical;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private ScrollRect.MovementType movementType;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private float elasticity;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool inertia;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private float decelerationRate;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private float scrollSensitivity;
        
        
        public ScrollRectAdaptiveUIOrientationState(AdaptiveUIScrollRect element, AdaptiveUIOrientation orientation)
        {
            _orientation = orientation;
            _element = element;
            
#if UNITY_EDITOR
            SaveCurrentParameters();
#endif
        }
        
        public override void Apply(AdaptiveUIScrollRect element)
        {
            if(_isEnabled == false)
                return;
            
            if (element == null)
            {
                Debug.LogWarning("Element is not assigned!");
                return;
            }
            
            var source = element.ScrollRect;
            
            if (source == null)
            {
                Debug.LogWarning("Source ScrollRect is not assigned!");
                return;
            }
            
            source.horizontal = horizontal;
            source.vertical = vertical;
            source.movementType = movementType;
            source.elasticity = elasticity;
            source.inertia = inertia;
            source.decelerationRate = decelerationRate;
            source.scrollSensitivity = scrollSensitivity;
            
            source.velocity = Vector2.zero;
        }

#if UNITY_EDITOR
        [Button]
        internal void SaveCurrentParameters()
        {
            if (_element == null)
            {
                Debug.LogWarning("Element is not assigned!");
                return;
            }
            
            var source = _element.ScrollRect;
            
            if (source == null)
            {
                Debug.LogWarning("Source ScrollRect is not assigned!");
                return;
            }
            
            // Copy all parameters from the source ScrollRect
            horizontal = source.horizontal;
            vertical = source.vertical;
            movementType = source.movementType;
            elasticity = source.elasticity;
            inertia = source.inertia;
            decelerationRate = source.decelerationRate;
            scrollSensitivity = source.scrollSensitivity;
        }

        [Button]
        private void ApplyParameters()
        {
            Apply(_element);
        }
#endif

    }
}