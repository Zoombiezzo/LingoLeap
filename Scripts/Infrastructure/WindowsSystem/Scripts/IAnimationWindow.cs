using UnityEngine;

namespace _Client.Scripts.Infrastructure.WindowsSystem.Scripts
{
    public abstract class AnimationWindow: MonoBehaviour
    {
        public bool IsPlayed { get; set; }
        public bool IsFinished { get; set; }

        public virtual void Play()
        {
            
        }

        public virtual void Stop()
        {
            
        }
    }
}