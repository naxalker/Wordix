using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class SettingsManager : IInitializable, IDisposable
{
    private const string SOUNDS_SETTING_KEY = "SoundsEnabled";
    private const string THEME_SETTING_KEY = "ThemeChanged";
    private const string LANGUAGE_SETTING_KEY = "LanguageIndex";

    private AudioController _audioController;
    private Board _board;
    private SettingsPanel _settingsPanel;

    private ThemeController _themeController;
    private PlayerStatistic _playerStatistic;
    private WordsController _wordsController;

    public SettingsManager(
        AudioController audioController,
        Board board,
        SettingsPanel settingsPanel,
        ThemeController themeController,
        PlayerStatistic playerStatistic,
        WordsController wordsController
    )
    {
        _audioController = audioController;
        _board = board;
        _settingsPanel = settingsPanel;
        _themeController = themeController;
        _playerStatistic = playerStatistic;
        _wordsController = wordsController;
    }

    public void Initialize()
    {
        _settingsPanel.OnResetButtonClicked += ResetButtonHandler;
        _settingsPanel.OnSoundsToggled += SoundsToggledHandler;
        _settingsPanel.OnThemeToggled += ThemeToggledHandler;
        _settingsPanel.OnLanguageChanged += LanguageChangedHandler;

        LoadInitialSettings();
    }

    public void Dispose()
    {
        _settingsPanel.OnResetButtonClicked -= ResetButtonHandler;
        _settingsPanel.OnSoundsToggled -= SoundsToggledHandler;
        _settingsPanel.OnThemeToggled -= ThemeToggledHandler;
        _settingsPanel.OnLanguageChanged -= LanguageChangedHandler;
    }

    private void LoadInitialSettings()
    {
        bool soundsEnabled = true;
        bool themeChanged = false;
        int languageIndex =
            LocalizationSettings.AvailableLocales.Locales
                .IndexOf(LocalizationSettings.SelectedLocale);

        Debug.Log($"Current language index: {languageIndex}");

        if (PlayerPrefs.HasKey(SOUNDS_SETTING_KEY))
        {
            soundsEnabled =
                PlayerPrefs.GetInt(SOUNDS_SETTING_KEY) == 1;

            _audioController.ToggleSound(soundsEnabled);
        }

        if (PlayerPrefs.HasKey(THEME_SETTING_KEY))
        {
            themeChanged =
                PlayerPrefs.GetInt(THEME_SETTING_KEY) == 1;

            if (themeChanged)
                _themeController.ChangeColors();
        }

        if (PlayerPrefs.HasKey(LANGUAGE_SETTING_KEY))
        {
            languageIndex = PlayerPrefs.GetInt(LANGUAGE_SETTING_KEY);

            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[languageIndex];
        }

        _settingsPanel.Setup(soundsEnabled, themeChanged, languageIndex);
    }

    private void ResetButtonHandler()
    {
        _playerStatistic.ResetProgress();
        _wordsController.ResetProgress();
        _board.StartNewGame();
    }

    private void SoundsToggledHandler(bool isOn)
    {
        _audioController.ToggleSound(isOn);

        PlayerPrefs.SetInt(SOUNDS_SETTING_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ThemeToggledHandler(bool isOn)
    {
        _themeController.ChangeColors();

        PlayerPrefs.SetInt(THEME_SETTING_KEY, isOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void LanguageChangedHandler(int localeIndex)
    {
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[localeIndex];

        PlayerPrefs.SetInt(LANGUAGE_SETTING_KEY, localeIndex);
        PlayerPrefs.Save();

        Debug.Log($"Language changed to: {LocalizationSettings.SelectedLocale.LocaleName}");
    }
}
