using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.SpinWheel
{
  #if UNITY_EDITOR
    public class WheelItemsPlacer : MonoBehaviour
    {
      [SerializeField] private List<RectTransform> _items;
      [SerializeField] [OnValueChanged("Place")] private float _radius;
      [SerializeField] [OnValueChanged("Place")] private float _startAngle;
      
      [Button]
      private void Place()
      {
        for (int i = 0; i < _items.Count; i++)
        {
          var angle = _startAngle - i * 360f / _items.Count;
          var x = _radius * Mathf.Cos(angle * Mathf.Deg2Rad);
          var y = _radius * Mathf.Sin(angle * Mathf.Deg2Rad);
          _items[i].anchoredPosition = new Vector2(x, y);
          _items[i].eulerAngles = new Vector3(0f, 0f, angle - _startAngle);
        }
      }
    }
  #endif
}