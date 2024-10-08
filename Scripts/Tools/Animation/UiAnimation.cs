using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.Tools.Animation
{
    public abstract class UiAnimation : MonoBehaviour
    {
        public bool IsPlaying { get; protected set; }
        public bool IsFinished { get; protected set; }

        [Button]
        public virtual Sequence Play(Action onComplete = null)
        {
            return DOTween.Sequence();
        }

        [Button]
        public virtual void Stop()
        {
            
        }
    }
}