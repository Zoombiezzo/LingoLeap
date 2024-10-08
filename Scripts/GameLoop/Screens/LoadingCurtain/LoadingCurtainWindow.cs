using _Client.Scripts.Infrastructure.WindowsSystem.Scripts;
using TMPro;
using UnityEngine;

namespace _Client.Scripts.GameLoop.Screens.LoadingCurtain
{
    public class LoadingCurtainWindow : Window
    {
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _progressValueText;
        
        public void SetStatus(string text)
        {
            _progressText.text = text;
        }
        
        public void SetProgress(float value)
        {
            ChangeValueInternal(value);
        }
        
        private void ChangeValueInternal(float value)
        {
            _progressValueText.text = $"{(int)(value * 100f)}%";
        }
    }
}