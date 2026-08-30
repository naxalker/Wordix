using UnityEngine;
using Zenject;

public class IntegrationsInstaller : MonoInstaller
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