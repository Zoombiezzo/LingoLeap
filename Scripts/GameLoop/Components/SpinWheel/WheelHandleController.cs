using System.Collections.Generic;
using _Client.Scripts.GameLoop.Screens.SpinWheel;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Client.Scripts.GameLoop.Components.SpinWheel
{
    public class WheelHandleController : MonoBehaviour
    {
        [FormerlySerializedAs("_wheelHandle")] [SerializeField] private SpinWheelHandle spinWheelHandle;
        [SerializeField] private List<PointOnWheel> _points;
        
        
    }
}