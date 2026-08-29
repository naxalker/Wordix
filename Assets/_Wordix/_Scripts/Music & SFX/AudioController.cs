using UnityEngine;
using Zenject;

public class AudioController : MonoBehaviour
{
    [SerializeField] private SoundEffectsSO _soundEffects;
    [SerializeField] private Board _board;

    private bool _isActive = true;

    private AudioSource _audioSource;
    private WordsController _wordsController;

    [Inject]
    private void Construct(WordsController wordsController)
    {
        _wordsController = wordsController;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _board.OnLetterPlaced += LetterPlacedHandler;
        _board.OnInvalidWord += InvalidWordEnteredHandler;
        _board.OnGameOver += GameOverHandler;
        _wordsController.OnAllWordsGuessed += AllWordsGuessedHandler;
    }

    private void OnDestroy()
    {
        _board.OnLetterPlaced -= LetterPlacedHandler;
        _board.OnInvalidWord -= InvalidWordEnteredHandler;
        _board.OnGameOver -= GameOverHandler;
        _wordsController.OnAllWordsGuessed -= AllWordsGuessedHandler;
    }

    public void ToggleSound(bool isActive)
    {
        _isActive = isActive;
    }

    private void LetterPlacedHandler() => PlayClip(_soundEffects.ClickSound, true);

    private void InvalidWordEnteredHandler() => PlayClip(_soundEffects.InvalidWordSound);

    private void GameOverHandler(bool hasWon, string word)
    {
        if (hasWon)
        {
            PlayClip(_soundEffects.VictorySound);
        }
        else
        {
            PlayClip(_soundEffects.LoseSound);
        }
    }

    private void AllWordsGuessedHandler()
    {
        PlayClip(_soundEffects.AllWordsGuessedSound);
    }

    private void PlayClip(AudioClip clip, bool pitched = false)
    {
        if (!_isActive) { return; }

        if (pitched)
            _audioSource.pitch = Random.Range(.8f, 1.2f);

        _audioSource.clip = clip;
        _audioSource.Play();

        if (pitched)
            _audioSource.pitch = 1f;
    }
}
