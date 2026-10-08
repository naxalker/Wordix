using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MessagePanel : MonoBehaviour
{
    private static float FADE_DURATION = .5f;

    [Header("Localization Strings")]
    [SerializeField] private LocalizedString[] _victoryMessagesLocalized;
    [SerializeField] private LocalizedString _invalidWordLocalized;
    [SerializeField] private LocalizedString _gameOverLoseFormatLocalized;
    [SerializeField] private LocalizedString _hintStringLocalized;
    [SerializeField] private LocalizedString _adUnavailableLocalized;

    [Header("References")]
    [SerializeField] private Board _board;

    [Space]
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private FadeableButton _nextWordButton;

    private void Awake()
    {
        _messageText.gameObject.SetActive(false);
        MakeMessageTransparent();

        _nextWordButton.Hide(0f);

        _nextWordButton.GetComponent<Button>().onClick.AddListener(() => _board.StartNewGame());
    }

    private void Start()
    {
        _board.OnLetterRemoved += LetterRemovedHandler;
        _board.OnInvalidWord += InvalidWordHandler;
        _board.OnGameOver += GameOverHandler;
        _board.OnNewGameStarted += NewGameStartedHandler;
    }

    private void OnDestroy()
    {
        _board.OnLetterRemoved -= LetterRemovedHandler;
        _board.OnInvalidWord -= InvalidWordHandler;
        _board.OnGameOver -= GameOverHandler;
        _board.OnNewGameStarted -= NewGameStartedHandler;
    }

    public void ShowHintMessage()
    {
        for (int i = 0; i < _board.GuessedLettersInWord.Length; i++)
        {
            if (_board.GuessedLettersInWord[i] == '\0')
            {
                ShowLocalizedMessage(_hintStringLocalized, i, char.ToUpper(_board.Word[i]));
                break;
            }
        }
    }

    public void ShowAdUnavailableMessage()
    {
        ShowLocalizedMessage(_adUnavailableLocalized);
    }

    private async void ShowLocalizedMessage(LocalizedString localizedString, params object[] arguments)
    {
        _messageText.DOKill();
        _messageText.gameObject.SetActive(true);

        localizedString.Arguments = arguments;

        var handle = localizedString.GetLocalizedStringAsync();
        string localizedStringValue = await handle.Task;
        _messageText.text = localizedStringValue;

        _messageText.DOFade(1f, FADE_DURATION)
                    .From(0f)
                    .SetEase(Ease.Linear);
    }

    private void InvalidWordHandler()
    {
        ShowLocalizedMessage(_invalidWordLocalized);
    }

    private void LetterRemovedHandler()
    {
        HideMessage();
    }

    private void GameOverHandler(bool isVictory, string word)
    {
        if (isVictory)
        {
            int randomIndex = Random.Range(0, _victoryMessagesLocalized.Length);
            ShowLocalizedMessage(_victoryMessagesLocalized[randomIndex]);
        }
        else
        {
            ShowLocalizedMessage(_gameOverLoseFormatLocalized, word.ToUpper());
        }

        _nextWordButton.Show(FADE_DURATION);
    }

    private void NewGameStartedHandler()
    {
        HideMessage();
        _nextWordButton.Hide(FADE_DURATION);
    }

    private void HideMessage()
    {
        MakeMessageTransparent();
        _messageText.gameObject.SetActive(false);
    }

    private void MakeMessageTransparent()
    {
        Color color = _messageText.color;
        color.a = 0f;
        _messageText.color = color;
    }
}
