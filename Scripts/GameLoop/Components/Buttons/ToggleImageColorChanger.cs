using UnityEngine;
using UnityEngine.UI;

public class ToggleImageColorChanger : ToggleValueChanger
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;
    
    public override void ChangeValue(float value)
    {
        _image.color = Color.Lerp(_offColor, _onColor, value);
    }
}