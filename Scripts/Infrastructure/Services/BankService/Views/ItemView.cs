using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Services.BankService.Views
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        
        public Image Image => _image;
        public TMP_Text Text => _text;
    }
}