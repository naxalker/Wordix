using System;
using Zenject;

public class LeaderboardController : IInitializable, IDisposable
{
    private const string TOTAL_WINS_LEADERBOARD = "totalWins";

    private PlayerStatistic _playerStatistic;
    private int _record;

    public LeaderboardController(PlayerStatistic playerStatistic)
    {
        _playerStatistic = playerStatistic;
    }

    public void Initialize()
    {
        _playerStatistic.OnTotalWinsValueChanged += TotalWinsValueChangedHandler;
    }

    public void Dispose()
    {
        _playerStatistic.OnTotalWinsValueChanged -= TotalWinsValueChangedHandler;
    }

    private void TotalWinsValueChangedHandler(int winsAmount)
    {
        if (winsAmount <= _record)
            return;

        _record = winsAmount;

        if (PlatformBridge.Service.IsLeaderboardSupported)
            PlatformBridge.Service.SetLeaderboardScore(TOTAL_WINS_LEADERBOARD, winsAmount);
    }
}
