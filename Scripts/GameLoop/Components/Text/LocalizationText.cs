using GameSDK.Localization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Components.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField] [ValueDropdown("@AssetsSelector.GetLocalizationKeys()")]
        private string _key;
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            Localization.AddTMPText(_key, _text);
        }

        private void OnDestroy()
        {
            Localization.RemoveTMPText(_key, _text);
        }

        private void OnValidate()
        {
            if (_text != null) return;

            _text = GetComponent<TMP_Text>();
        }
    }
}