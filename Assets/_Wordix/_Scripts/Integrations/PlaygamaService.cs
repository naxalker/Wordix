using Playgama;
using Playgama.Modules.Advertisement;
using Playgama.Modules.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class PlaygamaService : IPlatformService
{
    private Action _onRewarded;

    public bool IsInterstitialSupported => Bridge.advertisement.isInterstitialSupported;

    public bool IsRewardedSupported => Bridge.advertisement.isRewardedSupported;

    public void Initialize()
    {
        Bridge.advertisement.rewardedStateChanged += RewardedStateChangedHandler;
        Bridge.platform.audioStateChanged += AudioStateChangedHandler;
        Bridge.platform.pauseStateChanged += PauseStateChangedHandler;
    }

    public void GameReady()
        => Bridge.platform.SendMessage(PlatformMessage.GameReady);

    public void GameLoadingStarted()
        => Bridge.platform.SendMessage(PlatformMessage.InGameLoadingStarted);

    public void GameLoadingStopped()
        => Bridge.platform.SendMessage(PlatformMessage.InGameLoadingStopped);

    public void LevelStarted(string level = null)
    {
        var options = level != null ? new Dictionary<string, object> { { "level", level } } : null;
        Bridge.platform.SendMessage(PlatformMessage.LevelStarted, options);
    }

    public void LevelCompleted(string level = null)
    {
        var options = level != null ? new Dictionary<string, object> { { "level", level } } : null;
        Bridge.platform.SendMessage(PlatformMessage.LevelCompleted, options);
    }

    public void LevelFailed(string level = null)
    {
        var options = level != null ? new Dictionary<string, object> { { "level", level } } : null;
        Bridge.platform.SendMessage(PlatformMessage.LevelFailed, options);
    }

    public void LevelPaused(string level = null)
    {
        var options = level != null ? new Dictionary<string, object> { { "level", level } } : null;
        Bridge.platform.SendMessage(PlatformMessage.LevelPaused, options);
    }

    public void LevelResumed(string level = null)
    {
        var options = level != null ? new Dictionary<string, object> { { "level", level } } : null;
        Bridge.platform.SendMessage(PlatformMessage.LevelResumed, options);
    }

    public void ShowInterstitial()
    {
        if (Bridge.advertisement.isInterstitialSupported)
            Bridge.advertisement.ShowInterstitial();
    }

    public void ShowRewarded(Action onRewarded)
    {
        _onRewarded = onRewarded;
        Bridge.advertisement.ShowRewarded("hint");
    }

    public string GetLanguage() => Bridge.platform.language;

    public void SaveData<T>(string key, T value, Action<bool> onComplete = null)
    {
        string stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);

        Bridge.storage.Set(key, stringValue, onComplete);
    }

    public void SaveData(List<string> keys, List<object> values, Action<bool> onComplete = null)
    {
        Bridge.storage.Set(keys, values, onComplete);
    }

    public void LoadData(string key, Action<bool, string> onComplete = null)
    {
        Bridge.storage.Get(key, onComplete);
    }

    public void LoadData(List<string> keys, Action<bool, List<string>> onComplete = null)
    {
        Bridge.storage.Get(keys, onComplete);
    }

    private void RewardedStateChangedHandler(RewardedState state)
    {
        if (state == RewardedState.Rewarded)
        {
            _onRewarded?.Invoke();
        }
    }

    private void AudioStateChangedHandler(bool isEnabled)
    {
        AudioListener.pause = !isEnabled;
    }

    private void PauseStateChangedHandler(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            EventSystem.current.enabled = false;
        }
        else
        {
            Time.timeScale = 1;
            EventSystem.current.enabled = true;
        }
    }
}
