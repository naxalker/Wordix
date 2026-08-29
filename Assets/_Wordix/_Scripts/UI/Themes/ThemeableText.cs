using TMPro;
using UnityEngine;

public class ThemeableText : ThemeableObject
{
    [Header("Text")]
    [SerializeField] private Color _darkThemeTextColor = Color.black;
    [SerializeField] private Color _lightThemeTextColor = Color.black;
    [SerializeField] private TMP_Text _text;

    public override void ApplyTheme(Theme theme)
    {
        if (CanChangeColor == false) { return; }

        if (theme == Theme.Dark)
        {
            _text.color = _darkThemeTextColor;
        }
        else if (theme == Theme.Light)
        {
            _text.color = _lightThemeTextColor;
        }
    }
}
