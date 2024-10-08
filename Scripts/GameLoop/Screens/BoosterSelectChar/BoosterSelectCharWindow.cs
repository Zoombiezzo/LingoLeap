using _Client.Scripts.GameLoop.Components.Boosters;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Common;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.BoosterSelectChar
{
    public class BoosterSelectCharWindow : Window
    {
        [SerializeField] private BoosterCharsContainer _boosterCharsContainer;
        [SerializeField] private AnimationButton _hideButton;
        [SerializeField] private AnimationButton _openButton;
        [SerializeField] private UIToggleElement _openToggleElement;
        [SerializeField] private GameObject _countObjectButton;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private CounterField _counterBoosterField;
        [SerializeField] private ParticleImage _particleImageButtonPlay;
        
        public BoosterCharsContainer BoosterCharsContainer => _boosterCharsContainer;
        public AnimationButton HideButton => _hideButton;
        public AnimationButton OpenButton => _openButton;
        public UIToggleElement OpenToggleElement => _openToggleElement;
        public GameObject CountObjectButton => _countObjectButton;
        public TMP_Text CountText => _countText;
        public CounterField CounterBoosterField => _counterBoosterField;
        public ParticleImage ParticleImageButtonPlay => _particleImageButtonPlay;
    }
}