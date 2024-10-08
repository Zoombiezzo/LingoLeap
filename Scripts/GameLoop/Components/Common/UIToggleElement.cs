using _Client.Scripts.Tools.Animation;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Common
{
    public class UIToggleElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _target;
        [SerializeField] private UiAnimation _onAnimation;
        [SerializeField] private UiAnimation _offAnimation;
        [SerializeField] private bool _isOn = false;

        public void Set(bool value, bool animate = false)
        {
            if(_isOn == value)
                return;
            
            _isOn = value;

            _target.alpha = _isOn ? 1f : 0f;

            if (animate)
            {
                if (_isOn)
                {
                    _onAnimation?.Play();
                }
                else
                {
                    _offAnimation?.Play();
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Set(_isOn, false);
        }
#endif
    }
}