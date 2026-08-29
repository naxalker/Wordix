using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ThemeableImage : ThemeableObject
{
    [Header("Background")]
    [SerializeField]
    private Color _darkThemeBackgroundColor = Color.black;
    [SerializeField]
    private Color _lightThemeBackgroundColor = Color.black;
    [SerializeField] private Image _background;

    [Header("Outline")]
    [SerializeField] private bool _hasOutline = false;
    [ShowIf("_hasOutline")]
    [SerializeField] private Color _darkThemeOutlineColor = Color.black;
    [ShowIf("_hasOutline")]
    [SerializeField] private Color _lightThemeOutlineColor = Color.black;
    [ShowIf("_hasOutline")]
    [SerializeField] private Outline _outline;

    public override void ApplyTheme(Theme theme)
    {
        if (CanChangeColor == false) { return; }

        if (theme == Theme.Dark)
        {
            _background.color = _darkThemeBackgroundColor;
        }
        else if (theme == Theme.Light)
        {
            _background.color = _lightThemeBackgroundColor;
        }

        if (_outline != null)
        {
            if (theme == Theme.Dark)
            {
                _outline.effectColor = _darkThemeOutlineColor;
            }
            else if (theme == Theme.Light)
            {
                _outline.effectColor = _lightThemeOutlineColor;
            }
        }
    }
}
