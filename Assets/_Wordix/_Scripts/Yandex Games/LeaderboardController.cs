using System;
using YG;
using Zenject;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class LeaderboardController : IInitializable, IDisposable
{
    private const string RECORD_KEY = "Record";

    private PlayerStatistic _playerStatistic;
    private int _record;

    public LeaderboardController(PlayerStatistic playerStatistic)
    {
        _playerStatistic = playerStatistic;
    }

    public void Initialize()
    {
        _record = PlayerPrefs.GetInt(RECORD_KEY, 0);

        _playerStatistic.OnTotalWinsValueChanged += TotalWinsValueChangedHandler;
    }

    public void Dispose()
    {
        _playerStatistic.OnTotalWinsValueChanged -= TotalWinsValueChangedHandler;
    }

    private void TotalWinsValueChangedHandler(int winsAmount)
    {
        if (winsAmount > _record)
        {
            _record = winsAmount;
            PlayerPrefs.SetInt(RECORD_KEY, _record);
            YG2.SetLeaderboard("totalWins", winsAmount);
        }
    }
}
