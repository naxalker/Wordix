using UnityEngine;
using Zenject;

public class SettingsInstaller : MonoInstaller
{
    [SerializeField] private Board _board;
    [SerializeField] private SettingsPanel _settingsPanel;
    [SerializeField] private AudioController _audioController;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SettingsManager>()
            .AsSingle()
            .WithArguments(
                _audioController,
                _board,
                _settingsPanel
            );
    }
}