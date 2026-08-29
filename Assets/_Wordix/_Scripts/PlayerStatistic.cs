using System;
using UnityEngine;
using Zenject;

using PlayerPrefs = RedefineYG.PlayerPrefs;

public class PlayerStatistic : IInitializable, ITickable, IDisposable
{
    private const float MIN_TIME_BETWEEN_TIME_SAVING = 1f;

    private const string TOTAL_GAMES_PLAYED_KEY = "totalGamesPlayed";
    private const string TOTAL_WINS_KEY = "totalWins";
    private const string CURRENT_WIN_STREAK_KEY = "currentWinStreak";
    private const string BEST_WIN_STREAK_KEY = "bestWinStreak";
    private const string TOTAL_ATTEMPTS_KEY = "totalAttempts";
    private const string FASTEST_SOLVE_TIME_KEY = "fastestSolveTime";
    private const string TOTAL_TIME_PLAYED_KEY = "totalTimePlayed";

    public Action<float> OnTotalTimeValueChanged;
    public Action<int> OnTotalWinsValueChanged;

    public int TotalGamesPlayed { get; private set; }
    public int TotalWins { get; private set; }
    public int CurrentWinStreak { get; private set; }
    public int BestWinStreak { get; private set; }
    public int TotalAttempts { get; private set; }
    public float FastestSolveTime { get; private set; }
    public float TotalTimePlayed { get; private set; }

    private int _currentAttempts = 0;
    private float _currentSessionTime = 0f;
    private float _totalTimeSavingTimer;

    private Board _board;

    public PlayerStatistic(Board board)
    {
        _board = board;
    }

    public void Initialize()
    {
        TotalGamesPlayed = PlayerPrefs.GetInt(TOTAL_GAMES_PLAYED_KEY, 0);
        TotalWins = PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0);
        CurrentWinStreak = PlayerPrefs.GetInt(CURRENT_WIN_STREAK_KEY, 0);
        BestWinStreak = PlayerPrefs.GetInt(BEST_WIN_STREAK_KEY, 0);
        TotalAttempts = PlayerPrefs.GetInt(TOTAL_ATTEMPTS_KEY, 0);
        FastestSolveTime = PlayerPrefs.GetFloat(FASTEST_SOLVE_TIME_KEY, Mathf.Infinity);
        TotalTimePlayed = PlayerPrefs.GetFloat(TOTAL_TIME_PLAYED_KEY, 0f);

        _totalTimeSavingTimer = MIN_TIME_BETWEEN_TIME_SAVING;

        _board.OnGameOver += GameOverHandler;
        _board.OnValidWordEntered += ValidWordEnteredHandler;
        _board.OnNewGameStarted += NewGameStartedHandler;
    }

    public void Tick()
    {
        _currentSessionTime += Time.deltaTime;
        _totalTimeSavingTimer -= Time.deltaTime;

        if (_totalTimeSavingTimer <= 0f)
        {
            TotalTimePlayed += MIN_TIME_BETWEEN_TIME_SAVING;
            OnTotalTimeValueChanged?.Invoke(TotalTimePlayed);

            PlayerPrefs.SetFloat(TOTAL_TIME_PLAYED_KEY, TotalTimePlayed);
            PlayerPrefs.Save();

            _totalTimeSavingTimer = MIN_TIME_BETWEEN_TIME_SAVING;
        }
    }

    public void Dispose()
    {
        _board.OnGameOver -= GameOverHandler;
        _board.OnValidWordEntered -= ValidWordEnteredHandler;
        _board.OnNewGameStarted -= NewGameStartedHandler;
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();

        TotalGamesPlayed = 0;
        TotalWins = 0;
        CurrentWinStreak = 0;
        BestWinStreak = 0;
        TotalAttempts = 0;
        FastestSolveTime = Mathf.Infinity;
        TotalTimePlayed = 0;
    }

    private void GameOverHandler(bool hasWon, string word)
    {
        TotalGamesPlayed++;

        if (hasWon)
        {
            TotalWins++;
            OnTotalWinsValueChanged?.Invoke(TotalWins);

            CurrentWinStreak++;

            if (CurrentWinStreak > BestWinStreak)
            {
                BestWinStreak = CurrentWinStreak;
            }

            TotalAttempts += _currentAttempts;

            if (_currentSessionTime < FastestSolveTime)
            {
                FastestSolveTime = _currentSessionTime;
            }
        }
        else
        {
            CurrentWinStreak = 0;
        }

        SaveStatistic();
    }

    private void ValidWordEnteredHandler()
    {
        _currentAttempts++;
    }

    private void NewGameStartedHandler()
    {
        _currentSessionTime = 0f;
        _currentAttempts = 0;
    }

    private void SaveStatistic()
    {
        PlayerPrefs.SetInt(TOTAL_GAMES_PLAYED_KEY, TotalGamesPlayed);
        PlayerPrefs.SetInt(TOTAL_WINS_KEY, TotalWins);
        PlayerPrefs.SetInt(CURRENT_WIN_STREAK_KEY, CurrentWinStreak);
        PlayerPrefs.SetInt(BEST_WIN_STREAK_KEY, BestWinStreak);
        PlayerPrefs.SetInt(TOTAL_ATTEMPTS_KEY, TotalAttempts);
        PlayerPrefs.SetFloat(FASTEST_SOLVE_TIME_KEY, FastestSolveTime);
        PlayerPrefs.SetFloat(TOTAL_TIME_PLAYED_KEY, TotalTimePlayed);

        PlayerPrefs.Save();
    }
}
