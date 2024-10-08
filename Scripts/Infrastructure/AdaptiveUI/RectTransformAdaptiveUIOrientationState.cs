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
    public class RectTransformAdaptiveUIOrientationState : AdaptiveUIOrientationState<AdaptiveUIRectTransform>
    {
        [SerializeField] [ReadOnly]
        private AdaptiveUIRectTransform _element;
        
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 anchoredPosition;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector3 localPosition;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 anchorMin;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 anchorMax;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 pivot;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector2 sizeDelta;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Vector3 localScale;
        [SerializeField] [ReadOnly]
        [FoldoutGroup("Parameters")]
        private Quaternion localRotation;
        
        public RectTransformAdaptiveUIOrientationState(AdaptiveUIRectTransform element, AdaptiveUIOrientation orientation)
        {
            _orientation = orientation;
            _element = element;
            
#if UNITY_EDITOR
            SaveCurrentParameters();
#endif
        }
        
        public override void Apply(AdaptiveUIRectTransform element)
        {
            if(_isEnabled == false)
                return;
            
            if (element == null)
            {
                Debug.LogWarning("Element is not assigned!");
                return;
            }
            
            var sourceRectTransform = element.RectTransform;
            
            if (sourceRectTransform == null)
            {
                Debug.LogWarning("Source RectTransform is not assigned!");
                return;
            }

            sourceRectTransform.anchorMin = anchorMin;
            sourceRectTransform.anchorMax = anchorMax;
            sourceRectTransform.anchoredPosition = anchoredPosition;
            sourceRectTransform.sizeDelta = sizeDelta;
            sourceRectTransform.pivot = pivot;
            sourceRectTransform.localScale = localScale;
            sourceRectTransform.localRotation = localRotation;
            
            LayoutRebuilder.MarkLayoutForRebuild(sourceRectTransform);
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
            
            var sourceRectTransform = _element.RectTransform;
            
            if (sourceRectTransform == null)
            {
                Debug.LogWarning("Source RectTransform is not assigned!");
                return;
            }
            
            // Copy all parameters from the source RectTransform
            anchoredPosition = sourceRectTransform.anchoredPosition;
            localPosition = sourceRectTransform.localPosition;
            anchorMin = sourceRectTransform.anchorMin;
            anchorMax = sourceRectTransform.anchorMax;
            pivot = sourceRectTransform.pivot;
            sizeDelta = sourceRectTransform.sizeDelta;
            localScale = sourceRectTransform.localScale;
            localRotation = sourceRectTransform.localRotation;
        }

        [Button]
        private void ApplyParameters()
        {
            Apply(_element);
        }
#endif

    }
}