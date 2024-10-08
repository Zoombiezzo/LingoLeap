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
    public class GridLayoutElementAdaptiveUIOrientationState : AdaptiveUIOrientationState<AdaptiveUIGridLayoutGroup>
    {
        [SerializeField] [ReadOnly]
        private AdaptiveUIGridLayoutGroup _element;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private RectOffset padding;

        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 spacing;

        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private TextAnchor childAlignment;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 cellSize;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private GridLayoutGroup.Corner startCorner;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private GridLayoutGroup.Axis startAxis;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private GridLayoutGroup.Constraint constraint;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private int constraintCount;
        
        public GridLayoutElementAdaptiveUIOrientationState(AdaptiveUIGridLayoutGroup element, AdaptiveUIOrientation orientation)
        {
            _orientation = orientation;
            _element = element;
            
#if UNITY_EDITOR
            SaveCurrentParameters();
#endif
        }
        
        public override void Apply(AdaptiveUIGridLayoutGroup element)
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
            sourceLayoutGroup.cellSize = cellSize;
            sourceLayoutGroup.spacing = spacing;
            sourceLayoutGroup.childAlignment = childAlignment;
            sourceLayoutGroup.startCorner = startCorner;
            sourceLayoutGroup.startAxis = startAxis;
            sourceLayoutGroup.constraint = constraint;
            sourceLayoutGroup.constraintCount = constraintCount;
            
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
            cellSize = sourceLayoutGroup.cellSize;
            spacing = sourceLayoutGroup.spacing;
            startCorner = sourceLayoutGroup.startCorner;
            startAxis = sourceLayoutGroup.startAxis;
            childAlignment = sourceLayoutGroup.childAlignment;
            constraint = sourceLayoutGroup.constraint;
            constraintCount = sourceLayoutGroup.constraintCount;
        }

        [Button]
        private void ApplyParameters()
        {
            Apply(_element);
        }
#endif

    }
}