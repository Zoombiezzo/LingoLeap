using _Client.Scripts.GameLoop.Components.Boosters;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Common;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using AssetKits.ParticleImage;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectWord
{
    public class BoosterSelectWordWindow : Window
    {
        [SerializeField] private BoosterCharsContainer _boosterCharsContainer;
        [SerializeField] private AnimationButton _hideButton;
        [SerializeField] private AnimationButton _openButton;
        [SerializeField] private UIToggleElement _openToggleElement;
        [SerializeField] private float _maxDelay = 0.25f;
        [SerializeField] private CounterField _boosterCounterField;
        [SerializeField] private ParticleImage _particleImageButtonPlay;

        public BoosterCharsContainer BoosterCharsContainer => _boosterCharsContainer;
        public AnimationButton HideButton => _hideButton;
        public AnimationButton OpenButton => _openButton;
        public UIToggleElement OpenToggleElement => _openToggleElement;
        public float MaxDelay => _maxDelay;
        public CounterField BoosterCounterField => _boosterCounterField;
        public ParticleImage ParticleImageButtonPlay => _particleImageButtonPlay;
    }
}