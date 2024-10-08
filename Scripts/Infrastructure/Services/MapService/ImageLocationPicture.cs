using System;
using System.Threading.Tasks;
using _Client.Scripts.Tools.Animation;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public class ImageLocationPicture : LocationPicture
    {
        [SerializeField] private Image _image;
        [SerializeField] private UiAnimation _animationShow;
        [SerializeField] private UiAnimation _animationHide;
        
        public override Task Show(bool animate = false)
        {
            StopAllAnimations();
            
            return animate == false ? base.Show(false) : _animationShow.Play().AsyncWaitForCompletion();
        }
        
        public override Task Hide(bool animate = false)
        {
            StopAllAnimations();

            return animate == false ? base.Hide(false) : _animationHide.Play().AsyncWaitForCompletion();
        }

        private void StopAllAnimations()
        {
            _animationShow.Stop();
            _animationHide.Stop();
        }

        private void OnDestroy()
        {
            StopAllAnimations();
        }
    }
}