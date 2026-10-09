#if PLUGIN_YG_2
using System;
using System.Collections.Generic;
using System.Globalization;
using YG;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class PluginYourGamesService : IPlatformService
{
    private Action _onRewardedFailed;

    public bool IsInterstitialSupported => true;

#if CrazyGamesPlatform_yg || GameDistributionPlatform_yg || GamePixPlatform_yg
    public bool IsLeaderboardSupported => false;
    public bool IsExternalLinksAllowed => false;
#else
    public bool IsLeaderboardSupported => true;
    public bool IsExternalLinksAllowed => true;
#endif

#if GameMonetizePlatform_yg
    public bool IsRewardedSupported => false;
#else
    public bool IsRewardedSupported => true;
#endif

    // Saves and environment data are available only after the platform SDK has initialized.
    public void Initialize(Action onInitialized)
    {
        if (YG2.isSDKEnabled)
        {
            CompleteInitialization(onInitialized);
            return;
        }

        void SDKDataReceivedHandler()
        {
            YG2.onGetSDKData -= SDKDataReceivedHandler;
            CompleteInitialization(onInitialized);
        }

        YG2.onGetSDKData += SDKDataReceivedHandler;
    }

    public void GameReady()
    {
        YG2.GameReadyAPI();
    }

    public void LevelStarted(string level = null)
    {
        YG2.GameplayStart();
    }

    public void LevelCompleted(string level = null)
    {
        YG2.GameplayStop();
    }

    public void LevelFailed(string level = null)
    {
        YG2.GameplayStop();
    }

    public void LevelPaused(string level = null)
    {
        YG2.GameplayStop();
    }

    public void LevelResumed(string level = null)
    {
        YG2.GameplayStart();
    }

    public string GetLanguage() => YG2.envir.language;

    public void ShowInterstitial()
    {
        YG2.InterstitialAdvShow();
    }

    public void ShowRewarded(Action onRewarded, Action onFailed)
    {
#if GameMonetizePlatform_yg
        YG2.InterstitialAdvShow();

        onRewarded?.Invoke();
#else
#if CrazyGamesPlatform_yg
        // CrazyGames forbids rewarded buttons without effect, e.g. when an ad blocker blocks the ad.
        _onRewardedFailed = onFailed;
#endif
        YG2.RewardedAdvShow("giveHint", onRewarded);
#endif
    }

    public void SetLeaderboardScore(string leaderboardId, int score)
    {
        YG2.SetLeaderboard(leaderboardId, score);
    }

    public void OpenUrl(string url)
    {
        YG2.OnURL(url);
    }

    public void SaveData<T>(string key, T value, Action<bool> onComplete = null)
    {
        SaveSingle(key, value);
        PlayerPrefs.Save();
        onComplete?.Invoke(true);
    }

    public void SaveData(List<string> keys, List<object> values, Action<bool> onComplete = null)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            SaveSingle(keys[i], values[i]);
        }

        PlayerPrefs.Save();
        onComplete?.Invoke(true);
    }

    public void LoadData(string key, Action<bool, string> onComplete = null)
    {
        if (PlayerPrefs.HasKey(key))
        {
            onComplete?.Invoke(true, ReadSingle(key));
        }
        else
        {
            onComplete?.Invoke(false, null);
        }
    }

    public void LoadData(List<string> keys, Action<bool, List<string>> onComplete = null)
    {
        var results = new List<string>(keys.Count);
        bool anyFound = false;

        for (int i = 0; i < keys.Count; i++)
        {
            string k = keys[i];
            if (PlayerPrefs.HasKey(k))
            {
                results.Add(ReadSingle(k));
                anyFound = true;
            }
            else
            {
                results.Add(null);
            }
        }

        onComplete?.Invoke(anyFound, results);
    }

    private void CompleteInitialization(Action onInitialized)
    {
        YG2.onErrorRewardedAdv += RewardedErrorHandler;
        onInitialized?.Invoke();
    }

    private void RewardedErrorHandler()
    {
        _onRewardedFailed?.Invoke();
    }

    private void SaveSingle(string key, object value)
    {
        switch (value)
        {
            case int i:
                PlayerPrefs.SetInt(key, i);
                break;
            case float f:
                PlayerPrefs.SetFloat(key, f);
                break;
            case string s:
                PlayerPrefs.SetString(key, s);
                break;
            default:
                PlayerPrefs.SetString(key, Convert.ToString(value, CultureInfo.InvariantCulture));
                break;
        }
    }

    private string ReadSingle(string key)
    {
        string s = PlayerPrefs.GetString(key, null);
        if (!string.IsNullOrEmpty(s)) return s;

        float f = PlayerPrefs.GetFloat(key, float.NaN);
        if (!float.IsNaN(f)) return f.ToString(CultureInfo.InvariantCulture);

        int i = PlayerPrefs.GetInt(key, int.MinValue);
        if (i != int.MinValue) return i.ToString(CultureInfo.InvariantCulture);

        return null;
    }
}
#endif
