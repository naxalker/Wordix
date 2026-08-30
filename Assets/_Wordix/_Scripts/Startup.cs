using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    [SerializeField]
    private AssetReference _mainSceneReference;

    private AsyncOperationHandle<SceneInstance> _loadOperation;

    void Start()
    {
        LoadMainScene();
    }

    private async void LoadMainScene()
    {
        PlatformBridge.Service.Initialize();
        PlatformBridge.Service.GameLoadingStarted();

        await LocalizationSettings.InitializationOperation.Task;

        string locale = PlatformBridge.Service.GetLanguage() == "ru" ? "ru" : "en";
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(locale);

        await LocalizationSettings.InitializationOperation.Task;

        _loadOperation = Addressables.LoadSceneAsync(_mainSceneReference, LoadSceneMode.Single);
        await _loadOperation.Task;

        PlatformBridge.Service.GameLoadingStopped();
    }
}
