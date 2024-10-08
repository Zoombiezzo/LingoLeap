using UnityEngine;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [RequireComponent(typeof(RectTransform))]
    public class AdaptiveUIRectTransform : AdaptiveUIElement
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransformAdaptiveUIOrientationState _horizontalState;
        [SerializeField] private RectTransformAdaptiveUIOrientationState _verticalState;
        
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
            
            _horizontalState = new RectTransformAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Horizontal);
            _verticalState = new RectTransformAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Vertical);
            
            base.Reset();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if(_rectTransform == null) 
                TryGetComponent(out _rectTransform);
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