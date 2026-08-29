using UnityEngine;
using Zenject;

public class PlayerStatisticInstaller : MonoInstaller
{
    [SerializeField] private Board _board;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerStatistic>().AsSingle().WithArguments(_board);
    }
}