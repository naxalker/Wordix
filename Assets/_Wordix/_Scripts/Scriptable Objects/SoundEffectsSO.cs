using UnityEngine;

[CreateAssetMenu()]
public class SoundEffectsSO : ScriptableObject
{
    [field: SerializeField] public AudioClip ClickSound { get; private set; }
    [field: SerializeField] public AudioClip InvalidWordSound { get; private set; }
    [field: SerializeField] public AudioClip VictorySound { get; private set; }
    [field: SerializeField] public AudioClip LoseSound { get; private set; }
    [field: SerializeField] public AudioClip AllWordsGuessedSound { get; private set; }
}
