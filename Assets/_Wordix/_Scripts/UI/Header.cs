using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class Header : MonoBehaviour
{
    [Header("Help")]
    [SerializeField] private Button _helpButton;
    [SerializeField] private HelpPanel _helpPanel;

    [Header("Statistic")]
    [SerializeField] private Button _statButton;
    [SerializeField] private StatisticPanel _statisticPanel;

    [Header("Progress Text")]
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private LocalizedString _progressTextLocalized;

    [Header("Settings")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private SettingsPanel _settingsPanel;

    [Header("Other References")]
    [SerializeField] private Board _board;

    private WordsController _wordsController;

    [Inject]
    private void Construct(WordsController wordsController)
    {
        _wordsController = wordsController;
    }

    private void Start()
    {
        RefreshProgressText();

        _board.OnNewGameStarted += NewGameStartedHandler;

        _helpButton.onClick.AddListener(() => _helpPanel.Show());
        _statButton.onClick.AddListener(() => _statisticPanel.Show());
        _settingsButton.onClick.AddListener(() =>
        {
            PlatformBridge.Service.LevelPaused();
            _settingsPanel.gameObject.SetActive(true);
        });

        LocalizationSettings.SelectedLocaleChanged += LocaleChangedHandler;
    }

    private void OnDestroy()
    {
        _board.OnNewGameStarted -= NewGameStartedHandler;
        LocalizationSettings.SelectedLocaleChanged -= LocaleChangedHandler;
    }

    private void NewGameStartedHandler()
    {
        RefreshProgressText();
    }

    private void LocaleChangedHandler(Locale locale)
    {
        RefreshProgressText();
    }

    private async void RefreshProgressText()
    {
        _progressTextLocalized.Arguments = new object[] {
            _wordsController.GuessedWordsAmount,
            WordsController.WORDS_TO_GUESS_AMOUNT
        };

        var handle = _progressTextLocalized.GetLocalizedStringAsync();
        string localizedString = await handle.Task;
        _progressText.text = localizedString;
    }
}
