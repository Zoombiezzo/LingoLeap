using System.Collections.Generic;
using System.Threading.Tasks;
using _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.MapService.Factories
{
    public class LocationPictureFactory : ILocationPictureFactory
    {
        private readonly IAddressablesService _addressablesService;
        
        private List<PictureReference> _referencePictures = new(2);
        private Dictionary<PictureReference, LocationPicture> _pictures = new(2);

        public LocationPictureFactory(IAddressablesService addressablesService)
        {
            _addressablesService = addressablesService;
        }
        
        public async Task<LocationPicture> Create(Transform parent, ILocationConfig config)
        {
            var reference = config.PictureReference;

            if (_pictures.TryGetValue(reference, out var locationPicture))
                return locationPicture;
            
            var prefab = await _addressablesService.Load<GameObject>(reference);
            
            _referencePictures.Add(config.PictureReference);
            
            var picture = Object.Instantiate(prefab, parent);

            if (picture.TryGetComponent(out locationPicture) == true)
            {
                _pictures.TryAdd(reference, locationPicture);
            }
            
            return locationPicture;
        }

        public void Release(ILocationConfig config)
        {
            if(config == null)
                return;
            
            var reference = config.PictureReference;

            _referencePictures.Remove(reference);
            
            if (_pictures.TryGetValue(reference, out var locationPicture))
            {
                if (locationPicture != null)
                {
                    if (locationPicture.gameObject != null)
                    {
                        Object.Destroy(locationPicture.gameObject);
                    }
                }

                _pictures.Remove(reference);
            }
            
            _addressablesService.Release(reference);
        }
    }
}