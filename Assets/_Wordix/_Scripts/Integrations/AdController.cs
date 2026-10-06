using System;
using Zenject;

public class AdController : IInitializable, IDisposable
{
    private Board _board;
    private MessagePanel _messagePanel;
    private HintButton _hintButton;
    private bool _isFirstGame = true;

    public AdController(Board board, MessagePanel messagePanel, HintButton hintButton)
    {
        _board = board;
        _messagePanel = messagePanel;
        _hintButton = hintButton;
    }

    public void Initialize()
    {
        _hintButton.Setup(ShowRewardedAd);
        _board.OnNewGameStarted += NewGameStartedHandler;
    }

    public void Dispose()
    {
        _board.OnNewGameStarted -= NewGameStartedHandler;
    }

    private void ShowRewardedAd()
    {
        PlatformBridge.Service.ShowRewarded(() =>
        {
            _messagePanel.ShowHintMessage();

            _hintButton.Disable();
        });

    }

    private void NewGameStartedHandler()
    {
        if (PlatformBridge.Service.IsRewardedSupported)
            _hintButton.Enable();
        else
            _hintButton.Disable();

        if (_isFirstGame)
        {
            _isFirstGame = false;
            return;
        }

        PlatformBridge.Service.ShowInterstitial();
    }
}
