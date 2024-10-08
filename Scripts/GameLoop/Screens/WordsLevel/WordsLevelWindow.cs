using _Client.Scripts.GameLoop.Components.Boosters;
using _Client.Scripts.GameLoop.Components.Buttons;
using _Client.Scripts.GameLoop.Components.Common;
using _Client.Scripts.GameLoop.Components.WordsContainer;
using _Client.Scripts.GameLoop.Components.WordSelector;
using _Client.Scripts.GameLoop.Components.WordViewer;
using _Client.Scripts.GameLoop.Screens.AdditionalWords;
using _Client.Scripts.GameLoop.Screens.BoosterSelectChar;
using _Client.Scripts.GameLoop.Screens.BoosterSelectWord;
using _Client.Scripts.Infrastructure.AdaptiveUI;
using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using _Client.Scripts.Tools;
using _Client.Scripts.Tools.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.GameLoop.Screens.WordsLevel
{
    public class WordsLevelWindow : Window
    {
        [SerializeField] private WordSelector _wordSelector;
        [SerializeField] private WordViewer _wordViewer;
        [SerializeField] private WordsContainer _wordsContainer;
        [SerializeField] private TMP_Text _containsText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Button _buttonNextLevel;
        [SerializeField] private Button _buttonOpenWord;
        [SerializeField] private AnimationButton _buttonHome;
        [SerializeField] private AnimationButton _buttonCoins;
        [SerializeField] private CounterField _coinsCounter;
        [SerializeField] private Button _buttonReset;
        [SerializeField] private AdditionalWordProgressView _additionalWordProgress;
        [SerializeField] private AdaptiveUIGroup _adaptiveUIGroup;
        [SerializeField] private AdditionalWordAnimator _additionalWordAnimator;
        [SerializeField] private CanvasGroup _blockerLevel;
        
        [SerializeField] private BoosterButtonView _boosterSelectCharButton;
        [SerializeField] private BoosterButtonView _boosterSelectWordButton;
        
        [SerializeField] private UiAnimation _completeAnimation;

        [SerializeField] private UiElement _wordsUiElement;
        [SerializeField] private UiElement _wordsSelectPanelUiElement;
        
        [SerializeField] private BoosterCharAnimator _boosterCharAnimator;
        [SerializeField] private BoosterWordAnimator _boosterWordAnimator;
        
        public WordSelector WordSelector => _wordSelector;
        public WordViewer WordViewer => _wordViewer;
        public WordsContainer WordsContainer => _wordsContainer;
        public TMP_Text ContainsText => _containsText;
        public Button ButtonNextLevel => _buttonNextLevel;
        public Button ButtonOpenWord => _buttonOpenWord;
        public AnimationButton ButtonHome => _buttonHome;
        public AnimationButton ButtonCoins => _buttonCoins;
        public CounterField CoinsCounter => _coinsCounter;
        public Button ButtonReset => _buttonReset;
        public AdditionalWordProgressView AdditionalWordProgress => _additionalWordProgress;
        public AdaptiveUIGroup AdaptiveUIGroup => _adaptiveUIGroup;
        public TMP_Text LevelText => _levelText;
        public AdditionalWordAnimator AdditionalWordAnimator => _additionalWordAnimator;
        public CanvasGroup BlockerLevel => _blockerLevel;
        public BoosterButtonView BoosterSelectCharButton => _boosterSelectCharButton;
        public BoosterButtonView BoosterSelectWordButton => _boosterSelectWordButton;
        public UiAnimation CompleteAnimation => _completeAnimation;
        public UiElement WordsUiElement => _wordsUiElement;
        public UiElement WordsSelectPanelUiElement => _wordsSelectPanelUiElement;
        public BoosterCharAnimator BoosterCharAnimator => _boosterCharAnimator;
        public BoosterWordAnimator BoosterWordAnimator => _boosterWordAnimator;
    }
}