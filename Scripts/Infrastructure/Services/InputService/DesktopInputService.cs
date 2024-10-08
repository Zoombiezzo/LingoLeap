using System;
using _Client.Scripts.Helpers;
using UnityEngine;
using VContainer.Unity;

namespace _Client.Scripts.Infrastructure.Services.InputService
{
    public class DesktopInputService : IInputService, ITickable, IDisposable
    {
        public Vector2 MousePosition => Input.mousePosition;
        public event Action OnMousePressed;
        public event Action OnMouseReleased;
        public event Action OnUpdated;
        public event Action OnDragged;
        public event Action<Vector2> OnSwipe;
        public event Action<Vector2> OnTap;
        public event Action<KeyCode> OnKeyDown;
        public event Action<KeyCode> OnKeyUp;
        public event Action<KeyCode> OnKeyHold;

        private float SwipeThreshold = 100f;
        private float SwipeThresholdTime = 0.5f;
        
        private float TapThreshold = 5f;
        private float TapThresholdTime = 0.2f;
        
        private bool _isMousePressed;
        private float _mouseDownTime;
        private float _mouseUpTime;
        private bool _isSwiped;
        private Vector2 _lastPositionMouse;
        private Vector2 _mousePositionDown;
        private Vector2 _mousePositionUp;
        private Vector2 _screenResolution;
        private int _minValueResolution;
        private readonly KeyCode[] _keyCodes;
        
        public DesktopInputService()
        {
            _screenResolution = new Vector2(Screen.width, Screen.height);
            _minValueResolution = (int)Mathf.Min(_screenResolution.x, _screenResolution.y);
            SwipeThreshold = _minValueResolution * 0.05f;
            TapThreshold = _minValueResolution * 0.05f;
            
            _keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        }

        public void Tick()
        {
            SendMouseButtonDown();
            SendDrag();
            CheckSwipe();
            SendMouseButtonUp();
            UpdateKeys();
            
            OnUpdated?.Invoke();
        }

        private void UpdateKeys()
        {
            foreach (var key in _keyCodes)
            {
                if(Input.GetKeyDown(key)) OnKeyDown?.Invoke(key);
                if(Input.GetKey(key)) OnKeyHold?.Invoke(key);
                if(Input.GetKeyUp(key)) OnKeyUp?.Invoke(key);
            }
        }

        private void CheckSwipe()
        {
            if(_isSwiped) return;

            if (Time.time - _mouseDownTime > SwipeThresholdTime) return;

            var mouseDirection = (_mousePositionDown - _lastPositionMouse).Abs();
            if (mouseDirection.x > SwipeThreshold || mouseDirection.y > SwipeThreshold)
            {
                if (mouseDirection.x > mouseDirection.y)
                {
                    if (_mousePositionDown.x < _lastPositionMouse.x)
                    {
                        OnSwipeRight();
                    }
                    else
                    {
                        OnSwipeLeft();
                    }
                }
                else
                {
                    if (_mousePositionDown.y < _lastPositionMouse.y)
                    {
                        OnSwipeUp();
                    }
                    else
                    {
                        OnSwipeDown();
                    }
                }

                _isSwiped = true;
            }
            
            void OnSwipeDown()
            {
                OnSwipe?.Invoke(Vector2.down);
            }

            void OnSwipeUp()
            {
                OnSwipe?.Invoke(Vector2.up);
            }

            void OnSwipeLeft()
            {
                OnSwipe?.Invoke(Vector2.left);
            }

            void OnSwipeRight()
            {
                OnSwipe?.Invoke(Vector2.right);
            }
        }
        
        private void CheckTap()
        {
            if(_mouseUpTime - _mouseDownTime > TapThresholdTime) return;
            
            if (Mathf.Abs(_mousePositionDown.x - _mousePositionUp.x) > TapThreshold || Mathf.Abs(_mousePositionDown.y - _mousePositionUp.y) > TapThreshold)
            {
                return;
            }
            
            OnTap?.Invoke(_mousePositionUp);
        }

        private void SendMouseButtonDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDownTime = Time.time;
                OnMousePressed?.Invoke();
                _isMousePressed = true;
                _isSwiped = false;
                _lastPositionMouse = MousePosition;
                _mousePositionDown = MousePosition;
            }
        }
        
        private void SendMouseButtonUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseReleased?.Invoke();
                _isMousePressed = false;
                _mousePositionUp = MousePosition;
                _mouseUpTime = Time.time;

                CheckTap();
            }
        }

        private void SendDrag()
        {
            var delta = (MousePosition - _lastPositionMouse).sqrMagnitude;
            
            if (_isMousePressed && delta > 0)
            {
                OnDragged?.Invoke();
                _lastPositionMouse = MousePosition;
            }
        }

        public void Dispose()
        {
           
        }
    }
}