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
    public class ContentSizeFitterAdaptiveUIOrientationState : AdaptiveUIOrientationState<AdaptiveUIContentSizeFitter>
    {
        [SerializeField] [ReadOnly]
        private AdaptiveUIContentSizeFitter _element;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private ContentSizeFitter.FitMode horizontalFitMode;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private ContentSizeFitter.FitMode verticalFitMode;
        
        public ContentSizeFitterAdaptiveUIOrientationState(AdaptiveUIContentSizeFitter element, AdaptiveUIOrientation orientation)
        {
            _orientation = orientation;
            _element = element;
            
#if UNITY_EDITOR
            SaveCurrentParameters();
#endif
        }
        
        public override void Apply(AdaptiveUIContentSizeFitter element)
        {
            if(_isEnabled == false)
                return;
            
            if (element == null)
            {
                Debug.LogWarning("Element is not assigned!");
                return;
            }
            
            var source = element.ContentSizeFitter;
            
            if (source == null)
            {
                Debug.LogWarning("Source ContentSizeFitter is not assigned!");
                return;
            }
            
            source.horizontalFit = horizontalFitMode;
            source.verticalFit = verticalFitMode;
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
            
            var source = _element.ContentSizeFitter;
            
            if (source == null)
            {
                Debug.LogWarning("Source ContentSizeFitter is not assigned!");
                return;
            }
            
            // Copy all parameters from the source ContentSizeFitter
            horizontalFitMode = source.horizontalFit;
            verticalFitMode = source.verticalFit;
        }

        [Button]
        private void ApplyParameters()
        {
            Apply(_element);
        }
#endif

    }
}