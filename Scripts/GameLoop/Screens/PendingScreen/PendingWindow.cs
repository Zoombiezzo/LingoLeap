using System.Collections.Generic;
using _Client.Scripts.Infrastructure.ComponentToggler;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools.Animation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.PendingScreen
{
    public class PendingWindow : Window
    {
        [SerializeField] private List<UiAnimation> _animationOnShowedWindow;
        [SerializeField] private ToggleComponent _toggleComponent;

        protected override void OnBeforeShown()
        {
            base.OnBeforeShown();
            ShowAnimation();
        }
        
        protected override void OnHidden()
        {
            base.OnHidden();
            HideAnimation();
        }

        private void ShowAnimation()
        {
            _toggleComponent.Enable();
            
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying == false)
                    anim.Play();
            }   
        }
        
        private void HideAnimation()
        {
            _toggleComponent.Disable();
            
            foreach (var anim in _animationOnShowedWindow)
            {
                if (anim.IsPlaying)
                    anim.Stop();
            }
        }
        
#if UNITY_EDITOR

        [Button]
        private void ShowScreen()
        {
            Show();
        }

        [Button]
        private void HideScreen()
        {
            Hide();
        }
#endif
    }
}