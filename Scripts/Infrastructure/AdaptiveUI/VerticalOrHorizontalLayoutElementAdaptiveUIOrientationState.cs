using System;
using _Client.Scripts.Tools.Layout;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [Serializable]
    public class VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState : AdaptiveUIOrientationState<AdaptiveUIVerticalOrHorizontalLayoutGroup>
    {
        [SerializeField] [ReadOnly]
        private AdaptiveUIVerticalOrHorizontalLayoutGroup _element;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private RectOffset padding;

        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private float spacing;

        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private TextAnchor childAlignment;

        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool reverseArrangement;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childControlHeight;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childControlWidth;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childForceExpandHeight;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childForceExpandWidth;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childScaleHeight;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private bool childScaleWidth;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private LayoutDirection layoutDirection;
        
        public VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState(AdaptiveUIVerticalOrHorizontalLayoutGroup element, AdaptiveUIOrientation orientation)
        {
            _orientation = orientation;
            _element = element;
            
#if UNITY_EDITOR
            SaveCurrentParameters();
#endif
        }
        
        public override void Apply(AdaptiveUIVerticalOrHorizontalLayoutGroup element)
        {
            if(_isEnabled == false)
                return;
            
            if (element == null)
            {
                Debug.LogWarning("Element is not assigned!");
                return;
            }
            
            var sourceLayoutGroup = element.LayoutGroup;
            
            if (sourceLayoutGroup == null)
            {
                Debug.LogWarning("Source LayoutGroup is not assigned!");
                return;
            }

            sourceLayoutGroup.padding = padding;
            sourceLayoutGroup.spacing = spacing;
            sourceLayoutGroup.childAlignment = childAlignment;
            sourceLayoutGroup.reverseArrangement = reverseArrangement;
            sourceLayoutGroup.childControlHeight = childControlHeight;
            sourceLayoutGroup.childControlWidth = childControlWidth;
            sourceLayoutGroup.childForceExpandHeight = childForceExpandHeight;
            sourceLayoutGroup.childForceExpandWidth = childForceExpandWidth;
            sourceLayoutGroup.childScaleHeight = childScaleHeight;
            sourceLayoutGroup.childScaleWidth = childScaleWidth;
            sourceLayoutGroup.direction = layoutDirection;
            
            var sourceRectTransform = element.RectTransform;

            if (sourceRectTransform == null)
            {
                Debug.LogWarning("Source RectTransform is not assigned!");
                return;
            }
            
            LayoutRebuilder.MarkLayoutForRebuild(element.RectTransform);
            sourceRectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(sourceRectTransform);
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
            
            var sourceLayoutGroup = _element.LayoutGroup;
            
            if (sourceLayoutGroup == null)
            {
                Debug.LogWarning("Source LayoutGroup is not assigned!");
                return;
            }
            
            // Copy all parameters from the source LayoutGroup
            var paddingSource = sourceLayoutGroup.padding;
            padding = new RectOffset(paddingSource.left, paddingSource.right, paddingSource.top, paddingSource.bottom);
            spacing = sourceLayoutGroup.spacing;
            childAlignment = sourceLayoutGroup.childAlignment;
            reverseArrangement = sourceLayoutGroup.reverseArrangement;
            childControlHeight = sourceLayoutGroup.childControlHeight;
            childControlWidth = sourceLayoutGroup.childControlWidth;
            childForceExpandHeight = sourceLayoutGroup.childForceExpandHeight;
            childForceExpandWidth = sourceLayoutGroup.childForceExpandWidth;
            childScaleHeight = sourceLayoutGroup.childScaleHeight;
            childScaleWidth = sourceLayoutGroup.childScaleWidth;
            layoutDirection = sourceLayoutGroup.direction;
        }

        [Button]
        private void ApplyParameters()
        {
            Apply(_element);
        }
#endif

    }
}