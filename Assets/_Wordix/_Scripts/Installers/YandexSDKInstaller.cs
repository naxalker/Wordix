using UnityEngine;
using Zenject;

public class YandexSDKInstaller : MonoInstaller
{
    [SerializeField] private Board _board;
    [SerializeField] private MessagePanel _messagePanel;
    [SerializeField] private HintButton _hintButton;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AdController>().AsSingle().WithArguments(_board, _messagePanel, _hintButton);
        Container.BindInterfacesAndSelfTo<LeaderboardController>().AsSingle();
    }
}