using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class PointOnWheel : MonoBehaviour
    {
        [SerializeField] private RectTransform _point;

        public Vector2 Position => _point.anchoredPosition;
    }
}