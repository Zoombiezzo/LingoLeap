using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Helpers
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerAdapter : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            if (_canvasScaler == null)
            {
                _canvasScaler = GetComponent<CanvasScaler>();
            }
            
            UpdateCanvasScale();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateCanvasScale();
        }

        private void UpdateCanvasScale()
        {
            if(_canvasScaler == null) return;
            
            _canvasScaler.matchWidthOrHeight = Screen.width * 1f / Screen.height;
        }

        private void OnValidate()
        {
            if (_canvasScaler == null)
            {
                _canvasScaler = GetComponent<CanvasScaler>();
            }
        }

        private void Reset()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
        }
    }
}