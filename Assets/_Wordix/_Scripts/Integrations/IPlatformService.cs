using System;
using System.Collections.Generic;

public interface IPlatformService
{
    bool IsInterstitialSupported { get; }
    bool IsRewardedSupported { get; }

    void Initialize();
    void GameReady();
    void GameLoadingStarted();
    void GameLoadingStopped();
    void LevelStarted(string level = null);
    void LevelCompleted(string level = null);
    void LevelFailed(string level = null);
    void LevelPaused(string level = null);
    void LevelResumed(string level = null);
    void ShowInterstitial();
    void ShowRewarded(Action onRewarded);
    void SaveData<T>(string key, T value, Action<bool> onComplete = null);
    void SaveData(List<string> keys, List<object> values, Action<bool> onComplete = null);
    void LoadData(string key, Action<bool, string> onComplete = null);
    void LoadData(List<string> keys, Action<bool, List<string>> onComplete = null);
    string GetLanguage();
}
