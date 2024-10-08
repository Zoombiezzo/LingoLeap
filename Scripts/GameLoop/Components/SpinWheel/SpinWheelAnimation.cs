using DG.Tweening;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.SpinWheel
{
    public class SpinWheelAnimation : MonoBehaviour
    {
        public RectTransform Wheel;
        public AnimationCurve AnimationCurve;
        public float Duration = 0.5f;
        public float Rotation = 360f;
        
        private Sequence _sequence;

        public void Spin()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
            }
            
            _sequence = DOTween.Sequence()
                .Append(Wheel.DORotate(new Vector3(0f, 0f, Wheel.localRotation.z + Rotation), Duration, RotateMode.LocalAxisAdd).SetEase(AnimationCurve));
        }
    }
}