using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public event Action OnResetButtonClicked;
    public event Action<bool> OnSoundsToggled;
    public event Action<bool> OnThemeToggled;
    public event Action<int> OnLanguageChanged;

    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _resetButton;

    [SerializeField] private ToggleSwitch _toggleSounds;
    [SerializeField] private ToggleSwitch _toggleTheme;

    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Button _changeLanguageButton;
    [SerializeField] private GameObject _changelLanguageWarningPanel;

    private int _languageIndex;

    private void Awake()
    {
        _exitButton.onClick.AddListener(
            () => gameObject.SetActive(false));
        _resetButton.onClick.AddListener(
            () => OnResetButtonClicked?.Invoke());

        _toggleSounds.OnToggled += ToggledSoundsHandler;
        _toggleTheme.OnToggled += ToggledThemeHandler;

        _languageDropdown.onValueChanged.AddListener(
            index =>
            {
                _languageDropdown.SetValueWithoutNotify(1 - index);

                _languageIndex = index;

                _changelLanguageWarningPanel.SetActive(true);
            }
        );

        _changeLanguageButton.onClick.AddListener(
            () =>
            {
                OnLanguageChanged?.Invoke(_languageIndex);

                _languageDropdown.SetValueWithoutNotify(_languageIndex);
                _languageDropdown.RefreshShownValue();

                _changelLanguageWarningPanel.SetActive(false);
                gameObject.SetActive(false);
            }
        );
    }

    private void OnDestroy()
    {
        _toggleSounds.OnToggled -= ToggledSoundsHandler;
        _toggleTheme.OnToggled -= ToggledThemeHandler;
    }

    public void Setup(bool soundsEnabled, bool themeChanged, int languageIndex)
    {
        _toggleSounds.Setup(soundsEnabled ? 1f : 0f);
        _toggleTheme.Setup(themeChanged ? 0f : 1f);

        _languageDropdown.SetValueWithoutNotify(languageIndex);
        _languageDropdown.RefreshShownValue();

        _changelLanguageWarningPanel.SetActive(false);
    }

    private void ToggledSoundsHandler(bool isOn)
    {
        OnSoundsToggled?.Invoke(isOn);
    }

    private void ToggledThemeHandler(bool isOn)
    {
        OnThemeToggled?.Invoke(isOn);
    }
}
