using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Zenject;

public class PlayerStatistic : IInitializable, ITickable, IDisposable
{
    private const float MIN_TIME_BETWEEN_TIME_SAVING = 15f;

    private const string TOTAL_GAMES_PLAYED_KEY = "totalGamesPlayed";
    private const string TOTAL_WINS_KEY = "totalWins";
    private const string CURRENT_WIN_STREAK_KEY = "currentWinStreak";
    private const string BEST_WIN_STREAK_KEY = "bestWinStreak";
    private const string TOTAL_ATTEMPTS_KEY = "totalAttempts";
    private const string FASTEST_SOLVE_TIME_KEY = "fastestSolveTime";
    private const string TOTAL_TIME_PLAYED_KEY = "totalTimePlayed";

    private readonly List<string> _statisticKeys = new List<string>
    {
        TOTAL_GAMES_PLAYED_KEY,
        TOTAL_WINS_KEY,
        CURRENT_WIN_STREAK_KEY,
        BEST_WIN_STREAK_KEY,
        TOTAL_ATTEMPTS_KEY,
        FASTEST_SOLVE_TIME_KEY,
        TOTAL_TIME_PLAYED_KEY
    };

    public Action<float> OnTotalTimeValueChanged;
    public Action<int> OnTotalWinsValueChanged;
    public Action OnStatsLoaded;

    public int TotalGamesPlayed { get; private set; }
    public int TotalWins { get; private set; }
    public int CurrentWinStreak { get; private set; }
    public int BestWinStreak { get; private set; }
    public int TotalAttempts { get; private set; }
    public float FastestSolveTime { get; private set; } = Mathf.Infinity;
    public float TotalTimePlayed { get; private set; }

    private int _currentAttempts = 0;
    private float _currentSessionTime = 0f;
    private float _totalTimeSavingTimer;

    private readonly Board _board;

    public PlayerStatistic(Board board)
    {
        _board = board;
    }

    public void Initialize()
    {
        _totalTimeSavingTimer = MIN_TIME_BETWEEN_TIME_SAVING;

        _board.OnGameOver += GameOverHandler;
        _board.OnValidWordEntered += ValidWordEnteredHandler;
        _board.OnNewGameStarted += NewGameStartedHandler;

        LoadStatistic();
    }

    public void Tick()
    {
        _currentSessionTime += Time.deltaTime;
        _totalTimeSavingTimer -= Time.deltaTime;

        if (_totalTimeSavingTimer <= 0f)
        {
            TotalTimePlayed += MIN_TIME_BETWEEN_TIME_SAVING;
            OnTotalTimeValueChanged?.Invoke(TotalTimePlayed);

            PlatformBridge.Service.SaveData(TOTAL_TIME_PLAYED_KEY, TotalTimePlayed);

            _totalTimeSavingTimer = MIN_TIME_BETWEEN_TIME_SAVING;
        }
    }

    public void Dispose()
    {
        _board.OnGameOver -= GameOverHandler;
        _board.OnValidWordEntered -= ValidWordEnteredHandler;
        _board.OnNewGameStarted -= NewGameStartedHandler;

        SaveStatistic();
    }

    public void ResetProgress()
    {
        TotalGamesPlayed = 0;
        TotalWins = 0;
        CurrentWinStreak = 0;
        BestWinStreak = 0;
        TotalAttempts = 0;
        FastestSolveTime = Mathf.Infinity;
        TotalTimePlayed = 0;

        OnTotalWinsValueChanged?.Invoke(0);
        OnTotalTimeValueChanged?.Invoke(0);

        SaveStatistic();
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

    private void LoadStatistic()
    {
        PlatformBridge.Service.LoadData(_statisticKeys, (success, values) =>
        {
            if (!success || values == null || values.Count < _statisticKeys.Count)
                return;

            TotalGamesPlayed = DataParser.ParseInt(values[0], 0);
            TotalWins = DataParser.ParseInt(values[1], 0);
            CurrentWinStreak = DataParser.ParseInt(values[2], 0);
            BestWinStreak = DataParser.ParseInt(values[3], 0);
            TotalAttempts = DataParser.ParseInt(values[4], 0);
            FastestSolveTime = DataParser.ParseFloat(values[5], Mathf.Infinity);
            TotalTimePlayed = DataParser.ParseFloat(values[6], 0f);

            OnTotalWinsValueChanged?.Invoke(TotalWins);
            OnTotalTimeValueChanged?.Invoke(TotalTimePlayed);
            OnStatsLoaded?.Invoke();
        });
    }

    private void SaveStatistic()
    {
        var values = new List<object>
        {
            TotalGamesPlayed,
            TotalWins,
            CurrentWinStreak,
            BestWinStreak,
            TotalAttempts,
            FastestSolveTime,
            TotalTimePlayed
        };

        PlatformBridge.Service.SaveData(_statisticKeys, values);
    }
}