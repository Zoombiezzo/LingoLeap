using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.AdaptiveUI
{
    [RequireComponent(typeof(ContentSizeFitter))]
    public class AdaptiveUIContentSizeFitter : AdaptiveUIElement
    {
        [SerializeField] private ContentSizeFitter _contentSizeFitter;
        [SerializeField] private ContentSizeFitterAdaptiveUIOrientationState _horizontalState;
        [SerializeField] private ContentSizeFitterAdaptiveUIOrientationState _verticalState;
        
        internal ContentSizeFitter ContentSizeFitter => _contentSizeFitter;

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
            if (_contentSizeFitter == null)
                TryGetComponent(out _contentSizeFitter);
            
            _horizontalState = new ContentSizeFitterAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Horizontal);
            _verticalState = new ContentSizeFitterAdaptiveUIOrientationState(this, AdaptiveUIOrientation.Vertical);
            
            base.Reset();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if(_contentSizeFitter == null) 
                TryGetComponent(out _contentSizeFitter);
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