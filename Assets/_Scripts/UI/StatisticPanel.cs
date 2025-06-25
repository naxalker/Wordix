using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class StatisticPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _totalGamesPlayedText;
    [SerializeField] private TMP_Text _totalWinsText;
    [SerializeField] private TMP_Text _totalWinsPercentageText;
    [SerializeField] private TMP_Text _currentWinStreakText;
    [SerializeField] private TMP_Text _bestWinStreakText;
    [SerializeField] private TMP_Text _averageAttemptsText;
    [SerializeField] private TMP_Text _fastestSolveTimeText;
    [SerializeField] private TMP_Text _totalTimeText;

    [SerializeField] private Button _returnButton;

    [SerializeField] private LocalizedString _totalGamesPlayedString;

    private PlayerStatistic _playerStatistic;

    [Inject]
    private void Construct(PlayerStatistic playerStatistic)
    {
        _playerStatistic = playerStatistic;
    }

    private void OnEnable()
    {
        _playerStatistic.OnTotalTimeValueChanged += TotalTimeValueChangedHandler;
        LocalizationSettings.SelectedLocaleChanged += LocaleChangedHandler;
    }

    private void Start()
    {
        _returnButton.onClick.AddListener(() => Hide());
    }

    private void OnDisable()
    {
        _playerStatistic.OnTotalTimeValueChanged -= TotalTimeValueChangedHandler;
        LocalizationSettings.SelectedLocaleChanged -= LocaleChangedHandler;
    }

    public async void Show()
    {
        gameObject.SetActive(true);

        _totalGamesPlayedText.text = await GetPlayedGamesText(_playerStatistic.TotalGamesPlayed);
        _totalWinsText.text = _playerStatistic.TotalWins.ToString();

        if (_playerStatistic.TotalGamesPlayed > 0)
        {
            _totalWinsPercentageText.text = $"({Mathf.RoundToInt((float)_playerStatistic.TotalWins / _playerStatistic.TotalGamesPlayed * 100)}%)";
        }
        else
        {
            _totalWinsPercentageText.text = "";
        }

        _currentWinStreakText.text = _playerStatistic.CurrentWinStreak.ToString();
        _bestWinStreakText.text = _playerStatistic.BestWinStreak.ToString();

        if (_playerStatistic.TotalWins > 0)
        {
            _averageAttemptsText.text = Mathf.RoundToInt((float)_playerStatistic.TotalAttempts / _playerStatistic.TotalWins).ToString();
        }
        else
        {
            _averageAttemptsText.text = "-";
        }

        if (_playerStatistic.FastestSolveTime != Mathf.Infinity)
        {
            _fastestSolveTimeText.text = GetFormattedTime(_playerStatistic.FastestSolveTime);
        }
        else
        {
            _fastestSolveTimeText.text = "-";
        }

        _totalTimeText.text = GetFormattedTime(_playerStatistic.TotalTimePlayed);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void TotalTimeValueChangedHandler(float time)
    {
        _totalTimeText.text = GetFormattedTime(time);
    }

    private async void LocaleChangedHandler(Locale locale)
    {
        _totalGamesPlayedText.text = await GetPlayedGamesText(_playerStatistic.TotalGamesPlayed);
    }

    private string GetFormattedTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt(time % 3600 / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (hours > 0)
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        else
            return $"{minutes:00}:{seconds:00}";
    }

    private async Task<string> GetPlayedGamesText(int totalGames)
    {
        string gameWord;

        if (LocalizationSettings.SelectedLocale.Identifier.Code == "ru")
        {
            if (totalGames % 100 >= 11 && totalGames % 100 <= 19)
            {
                gameWord = "игр";
            }
            else
            {
                int lastDigit = totalGames % 10;

                if (lastDigit == 1)
                    gameWord = "игру";
                else if (lastDigit >= 2 && lastDigit <= 4)
                    gameWord = "игры";
                else
                    gameWord = "игр";
            }
        }
        else
        {
            gameWord = totalGames > 1 ? "games" : "game";
        }

        _totalGamesPlayedString.Arguments = new object[]
        {
            totalGames,
            gameWord
        };

        var handle = _totalGamesPlayedString.GetLocalizedStringAsync();

        return await handle.Task;
    }
}
