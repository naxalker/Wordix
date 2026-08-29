using System;
using YG;
using Zenject;

public class AdController : IInitializable, IDisposable
{
    private Board _board;
    private MessagePanel _messagePanel;
    private HintButton _hintButton;

    public AdController(Board board, MessagePanel messagePanel, HintButton hintButton)
    {
        _board = board;
        _messagePanel = messagePanel;
        _hintButton = hintButton;
    }

    public void Initialize()
    {
        _hintButton.Setup(ShowRewAd);
        _board.OnNewGameStarted += NewGameStartedHandler;
    }

    public void Dispose()
    {
        _board.OnNewGameStarted -= NewGameStartedHandler;
    }

    private void ShowRewAd()
    {
#if GameMonetizePlatform_yg
        YG2.InterstitialAdvShow();

        _messagePanel.ShowHintMessage();

        _hintButton.Disable();
#else
        YG2.RewardedAdvShow("giveHint", () =>
        {
            _messagePanel.ShowHintMessage();

            _hintButton.Disable();
        });
#endif
    }

    private void NewGameStartedHandler()
    {
        _hintButton.Enable();
        YG2.InterstitialAdvShow();
    }
}
