using UnityEngine;
using Zenject;

public class ThemeControllerInstaller : MonoInstaller
{
    [SerializeField] private Board _board;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ThemeController>().AsSingle().WithArguments(_board);
    }
}