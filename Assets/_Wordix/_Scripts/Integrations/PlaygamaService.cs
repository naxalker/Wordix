#if PLAYGAMA
using Playgama;
using Playgama.Modules.Advertisement;
using Playgama.Modules.Leaderboards;
using Playgama.Modules.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaygamaService : IPlatformService
{
    private readonly HashSet<string> _savesInProgress = new();
    private readonly Dictionary<string, (List<string> keys, List<object> values, Action<bool> onComplete)> _pendingSaves = new();

    private Action _onRewarded;
    private EventSystem _pausedEventSystem;

    public bool IsInterstitialSupported => Bridge.advertisement.isInterstitialSupported;

    public bool IsRewardedSupported => Bridge.advertisement.isRewardedSupported;

    public bool IsLeaderboardSupported => Bridge.leaderboards.type != LeaderboardType.NotAvailable;

    public bool IsExternalLinksAllowed => Bridge.platform.isExternalLinksAllowed;

    public void Initialize(Action onInitialized)
    {
        Bridge.advertisement.rewardedStateChanged += RewardedStateChangedHandler;
        Bridge.platform.audioStateChanged += AudioStateChangedHandler;
        Bridge.platform.pauseStateChanged += PauseStateChangedHandler;

        AudioStateChangedHandler(Bridge.platform.isAudioEnabled);

        onInitialized?.Invoke();
    }

    public void GameReady()
        => Bridge.platform.SendMessage(PlatformMessage.GameReady);

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
            Bridge.advertisement.ShowInterstitial("next_word");
    }

    // Failures are reported by Bridge itself (useBuiltInErrorPopup in playgama-bridge-config.json).
    public void ShowRewarded(Action onRewarded, Action onFailed)
    {
        _onRewarded = onRewarded;
        Bridge.advertisement.ShowRewarded("hint");
    }

    public void SetLeaderboardScore(string leaderboardId, int score)
    {
        Bridge.leaderboards.SetScore(leaderboardId, score);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public string GetLanguage() => Bridge.platform.language;

    public void SaveData<T>(string key, T value, Action<bool> onComplete = null)
    {
        Save(new List<string> { key }, new List<object> { ToInvariantString(value) }, onComplete);
    }

    public void SaveData(List<string> keys, List<object> values, Action<bool> onComplete = null)
    {
        Save(keys, values.ConvertAll(ToInvariantString), onComplete);
    }

    public void LoadData(string key, Action<bool, string> onComplete = null)
    {
        Bridge.storage.Get(key, onComplete);
    }

    public void LoadData(List<string> keys, Action<bool, List<string>> onComplete = null)
    {
        Bridge.storage.Get(keys, onComplete);
    }

    // Bridge.storage ignores a new value while a save for the same keys is in progress, so the latest one is resent afterwards.
    private void Save(List<string> keys, List<object> values, Action<bool> onComplete)
    {
        // Bridge is already destroyed when the application is quitting.
        if (Bridge.instance == null)
            return;

        string saveKey = string.Join("|", keys);

        if (!_savesInProgress.Add(saveKey))
        {
            if (_pendingSaves.TryGetValue(saveKey, out var pending))
                onComplete = pending.onComplete + onComplete;

            _pendingSaves[saveKey] = (keys, values, onComplete);
            return;
        }

        Bridge.storage.Set(keys, values, success =>
        {
            _savesInProgress.Remove(saveKey);
            onComplete?.Invoke(success);

            if (_pendingSaves.Remove(saveKey, out var pending))
                Save(pending.keys, pending.values, pending.onComplete);
        });
    }

    private static object ToInvariantString(object value) => Convert.ToString(value, CultureInfo.InvariantCulture);

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

            // EventSystem.current becomes null once the EventSystem is disabled, so keep the reference to re-enable it.
            if (EventSystem.current != null)
            {
                _pausedEventSystem = EventSystem.current;
                _pausedEventSystem.enabled = false;
            }
        }
        else
        {
            Time.timeScale = 1;

            if (_pausedEventSystem != null)
            {
                _pausedEventSystem.enabled = true;
                _pausedEventSystem = null;
            }
        }
    }
}
#endif
