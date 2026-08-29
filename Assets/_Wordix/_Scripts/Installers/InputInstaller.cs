using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [SerializeField] private Board _board;
    [SerializeField] private VirtualKeyboard _keyboardRU;
    [SerializeField] private VirtualKeyboard _keyboardEN;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerInput>()
            .AsSingle()
            .WithArguments(_board, _keyboardRU, _keyboardEN);
    }
}