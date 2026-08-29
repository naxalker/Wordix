using UnityEngine;
using Zenject;

public class ScriptableObjectsInstaller : MonoInstaller
{
    [SerializeField] private TileColorsSO _tileColors;
    [SerializeField] private SoundEffectsSO _soundEffects;

    public override void InstallBindings()
    {
        Container.Bind<TileColorsSO>().FromScriptableObject(_tileColors).AsSingle();
        Container.Bind<SoundEffectsSO>().FromScriptableObject(_soundEffects).AsSingle();
    }
}