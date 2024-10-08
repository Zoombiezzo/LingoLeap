using System;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.InputService
{
    public interface IInputService : IService
    {
        Vector2 MousePosition { get; }

        event Action OnMousePressed;
        event Action OnMouseReleased;
        event Action OnUpdated;
        event Action OnDragged;
        event Action<Vector2> OnSwipe;
        event Action<Vector2> OnTap;
        event Action<KeyCode> OnKeyDown;
        event Action<KeyCode> OnKeyUp;
        event Action<KeyCode> OnKeyHold;
    }
}
