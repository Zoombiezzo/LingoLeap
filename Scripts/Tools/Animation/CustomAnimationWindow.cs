using UnityEngine;
using AnimationWindow = _Client.Scripts.Infrastructure.WindowsSystem.Scripts.AnimationWindow;

namespace _Client.Scripts.Tools.Animation
{
    public class CustomAnimationWindow : AnimationWindow
    {
        [SerializeField] private UiAnimation _animation;
        
        public override void Play()
        {
            IsPlayed = true;
            IsFinished = false;
            
            _animation.Play(OnCompleted);
        }

        public override void Stop()
        {
            IsPlayed = true;
            _animation.Stop();
        }
        
        private void OnCompleted()
        {
            IsPlayed = false;
            IsFinished = true;
        }
    }
}