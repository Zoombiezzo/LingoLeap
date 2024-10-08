using _Client.Scripts.Infrastructure.AudioSystem.Scripts;
using _Client.Scripts.Infrastructure.Helpers;
using AssetKits.ParticleImage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using AudioSettings = _Client.Scripts.Infrastructure.AudioSystem.Scripts.AudioSettings;
using AudioType = _Client.Scripts.Infrastructure.AudioSystem.Scripts.AudioType;

namespace _Client.Scripts.GameLoop.Screens.SpinWheel
{
    public class SpinWheelHandle : MonoBehaviour
    {
        [SerializeField] private RectTransform _handle;
        [SerializeField] private Vector2 _fixedAnchoredPosition;
        [SerializeField] private ParticleImage _particleImagePrefab;
        [SerializeField] private ParticleImage _particleImage;
        [SerializeField] private bool _usePooledParticleImage;
        [SerializeField] private AudioSelector _spinTickSound;
        
        private ObjectPool<ParticleImage> _pooledParticleImage;

        private void Awake()
        {
            _pooledParticleImage = new ObjectPool<ParticleImage>(CreateParticle, x => x.gameObject.SetActive(true),
                x => x.gameObject.SetActive(false), x => Destroy(x.gameObject), true, 20, 100);
        }

        public void UpdatePosition()
        {
            _handle.anchoredPosition = _fixedAnchoredPosition;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            AudioService.Play(AudioType.Sound, _spinTickSound.Audio, AudioSettings.Default);

            if (other.relativeVelocity.magnitude > 100) return;

            ParticleImage particleImage = null;
            if (_usePooledParticleImage)
            {
                particleImage = _pooledParticleImage.Get();
            }
            else
            {
                particleImage = _particleImage;
            }

            if (other.contactCount > 0)
            {
                var contact = other.GetContact(0);
                particleImage.rectTransform.position = contact.point;
            }
            
            PlayParticle(particleImage);
        }

        private async void PlayParticle(ParticleImage particleImage)
        {
            particleImage.Play();

            if (_usePooledParticleImage)
            {
                await UniTask.Delay(3000);
                _pooledParticleImage.Release(particleImage);
            }
        }

        private ParticleImage CreateParticle() => Instantiate(_particleImagePrefab, transform);
    }
}