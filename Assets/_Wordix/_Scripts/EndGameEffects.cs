using UnityEngine;
using Zenject;

[RequireComponent(typeof(ParticleSystem))]
public class EndGameEffects : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private WordsController _wordsController;

    [Inject]
    private void Construct(WordsController wordsController)
    {
        _wordsController = wordsController;
    }

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        _wordsController.OnAllWordsGuessed += AllWordsGuessedHandler;
    }

    private void OnDestroy()
    {
        _wordsController.OnAllWordsGuessed -= AllWordsGuessedHandler;
    }

    private void AllWordsGuessedHandler()
    {
        _particleSystem.Play();
    }
}
