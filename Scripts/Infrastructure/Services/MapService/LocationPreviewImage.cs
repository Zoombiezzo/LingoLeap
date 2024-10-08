using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.MapService
{
    public class LocationPreviewImage : LocationPreview
    {
        [SerializeField] private Image _image;

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
        
        public override void ShowClosedPanel(bool show)
        {
            base.ShowClosedPanel(show);
            _image.enabled = show == false;
        }
    }
}