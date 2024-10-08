using _Client.Scripts.Tools.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [RequireComponent(typeof(RectTransform), typeof(VerticalOrHorizontalLayoutGroup))]
    public class AdaptiveUIVerticalOrHorizontalLayoutGroup : AdaptiveUIElement
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private VerticalOrHorizontalLayoutGroup _layoutGroup;
        [SerializeField] private VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState _horizontalState;
        [SerializeField] private VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState _verticalState;
        internal VerticalOrHorizontalLayoutGroup LayoutGroup => _layoutGroup;
        internal RectTransform RectTransform => _rectTransform;

        public override void ChangeOrientation(AdaptiveUIOrientation orientation, bool force = false)
        {
            if (_currentOrientation == orientation && force == false)
                return;
            
            if (orientation == AdaptiveUIOrientation.Horizontal)
                _horizontalState.Apply(this);
            else
                _verticalState.Apply(this);
            
            base.ChangeOrientation(orientation, force);
        }
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            if (_rectTransform == null)
                TryGetComponent(out _rectTransform);
            
            if(_layoutGroup == null)
                TryGetComponent(out _layoutGroup);
            
            _horizontalState = new VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Horizontal);
            _verticalState = new VerticalOrHorizontalLayoutElementAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Vertical);
            
            base.Reset();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if(_rectTransform == null) 
                TryGetComponent(out _rectTransform);
            
            if(_layoutGroup == null)
                TryGetComponent(out _layoutGroup);
        }
        
        internal override void TryEdit(AdaptiveUIOrientation orientation)
        {
            if(_editMode == false)
                return;
            
            if (orientation == AdaptiveUIOrientation.Horizontal)
                _horizontalState.SaveCurrentParameters();
            if (orientation == AdaptiveUIOrientation.Vertical)
                _verticalState.SaveCurrentParameters();
        }
        
        internal override void SavePreset(AdaptiveUIOrientation orientation)
        {
            base.SavePreset(orientation);

            if (orientation == AdaptiveUIOrientation.Horizontal)
                _horizontalState.SaveCurrentParameters();
            if (orientation == AdaptiveUIOrientation.Vertical)
                _verticalState.SaveCurrentParameters();
        }
#endif
    }
}