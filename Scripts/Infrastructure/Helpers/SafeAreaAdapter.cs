using System;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Helpers
{
    [RequireComponent( typeof( RectTransform ) ), DisallowMultipleComponent]
    public sealed class SafeAreaAdapter : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private RectTransform _rectTransform;

        [SerializeField]
        private bool _enable = true;

        [SerializeField]
        private SafeAreaPosition _excludeSafeAreaPosition;

        private Rect _lastArea = Rect.zero;
        
        private void Refresh()
        {
            var currArea = Screen.safeArea;
            var currScreen = new Vector2(Screen.width, Screen.height);

            if (_enable && (currArea != _lastArea))
            {
                ChangeArea(currArea, currScreen);
            }
        }

        private void Awake()
        {
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            Refresh();
        }

        private void ChangeArea(Rect rect, Vector2 screen)
        {
            _lastArea = rect;
            
            Vector2 anchorMin = rect.min;
            Vector2 anchorMax = rect.max;

            if (_excludeSafeAreaPosition.HasFlag(SafeAreaPosition.All))
            {
                anchorMin = Vector2.zero;
                anchorMax = screen;
            }
            else
            {
                anchorMin.x = _excludeSafeAreaPosition.HasFlag(SafeAreaPosition.Left) ? 0 : Mathf.Clamp(anchorMin.x, 0, screen.x);
                anchorMin.y = _excludeSafeAreaPosition.HasFlag(SafeAreaPosition.Bottom) ? 0 : Mathf.Clamp(anchorMin.y, 0, screen.y);
                anchorMax.x = _excludeSafeAreaPosition.HasFlag(SafeAreaPosition.Right) ? screen.x : Mathf.Clamp(anchorMax.x, 0, screen.x);
                anchorMax.y = _excludeSafeAreaPosition.HasFlag(SafeAreaPosition.Top) ? screen.y : Mathf.Clamp(anchorMax.y, 0, screen.y);
            }
            
            anchorMin /= screen;
            anchorMax /= screen;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }

        private void Reset()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        [Flags]
        private enum SafeAreaPosition
        {
            None = 1,
            All = None << 1,
            Top = All << 1,
            Bottom = Top << 1,
            Left = Bottom << 1,
            Right = Left << 1,
        }
    }
}