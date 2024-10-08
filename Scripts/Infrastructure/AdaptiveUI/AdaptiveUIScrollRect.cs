using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [RequireComponent(typeof(ScrollRect))]
    public class AdaptiveUIScrollRect : AdaptiveUIElement
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ScrollRectAdaptiveUIOrientationState _horizontalState;
        [SerializeField] private ScrollRectAdaptiveUIOrientationState _verticalState;
        
        internal ScrollRect ScrollRect => _scrollRect;

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
            if (_scrollRect == null)
                TryGetComponent(out _scrollRect);
            
            _horizontalState = new ScrollRectAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Horizontal);
            _verticalState = new ScrollRectAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Vertical);
            
            base.Reset();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if(_scrollRect == null) 
                TryGetComponent(out _scrollRect);
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