using _Client.Scripts.Infrastructure.Services.MapService;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.Background
{
    public class BackgroundWindow : Window
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private LocationPicture _picture;

        public RectTransform Container => _container;
        public LocationPicture Picture => _picture;
        
        public void SetCurrentBackground(LocationPicture picture)
        {
            _picture = picture;
        }
    }
    
    
}