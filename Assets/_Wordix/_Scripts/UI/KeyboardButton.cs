using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ThemeableImage))]
[RequireComponent(typeof(Image))]
public class KeyboardButton : MonoBehaviour
{
    private bool _colorHasChanged;
    private ThemeableImage _themeableImage;
    private Image _background;

    public bool ColorHasChanged => _colorHasChanged;

    private void Awake()
    {
        _themeableImage = GetComponent<ThemeableImage>();
        _background = GetComponent<Image>();
    }

    public void ChangeColor(Color color)
    {
        _colorHasChanged = true;

        _background.color = color;

        _themeableImage.Lock();
    }

    public void ResetButton()
    {
        _colorHasChanged = false;

        _themeableImage.Unlock();
    }
}
