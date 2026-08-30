using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;

public class SettingsManager : IInitializable, IDisposable
{
    private const string SOUNDS_SETTING_KEY = "SoundsEnabled";
    private const string THEME_SETTING_KEY = "ThemeChanged";
    private const string LANGUAGE_SETTING_KEY = "LanguageIndex";

    private readonly List<string> _settingsKeys = new List<string>
    {
        SOUNDS_SETTING_KEY,
        THEME_SETTING_KEY,
        LANGUAGE_SETTING_KEY
    };

    private readonly AudioController _audioController;
    private readonly Board _board;
    private readonly SettingsPanel _settingsPanel;
    private readonly ThemeController _themeController;
    private readonly PlayerStatistic _playerStatistic;
    private readonly WordsController _wordsController;

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
        int defaultLanguageIndex = LocalizationSettings.AvailableLocales.Locales
            .IndexOf(LocalizationSettings.SelectedLocale);

        PlatformBridge.Service.LoadData(_settingsKeys, (success, values) =>
        {
            bool soundsEnabled = true;
            bool themeChanged = false;
            int languageIndex = defaultLanguageIndex;

            if (success && values != null && values.Count >= _settingsKeys.Count)
            {
                // if sound key exists
                if (values[0] != null)
                {
                    soundsEnabled = DataParser.ParseBoolFromInt(values[0], true);
                    _audioController.ToggleSound(soundsEnabled);
                }

                // if theme key exists
                if (values[1] != null)
                {
                    themeChanged = DataParser.ParseBoolFromInt(values[1], false);
                    if (themeChanged)
                        _themeController.ChangeColors();
                }

                // if language key exists
                if (values[2] != null)
                {
                    languageIndex = DataParser.ParseInt(values[2], defaultLanguageIndex);
                    if (languageIndex >= 0 && languageIndex < LocalizationSettings.AvailableLocales.Locales.Count)
                    {
                        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
                    }
                }
            }

            _settingsPanel.Setup(soundsEnabled, themeChanged, languageIndex);
        });
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
        PlatformBridge.Service.SaveData(SOUNDS_SETTING_KEY, isOn ? 1 : 0);
    }

    private void ThemeToggledHandler(bool isOn)
    {
        _themeController.ChangeColors();
        PlatformBridge.Service.SaveData(THEME_SETTING_KEY, isOn ? 0 : 1);
    }

    private void LanguageChangedHandler(int localeIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        PlatformBridge.Service.SaveData(LANGUAGE_SETTING_KEY, localeIndex);

        Debug.Log($"Language changed to: {LocalizationSettings.SelectedLocale.LocaleName}");
    }
}
