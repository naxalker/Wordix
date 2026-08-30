using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Zenject;

public class WordsController : IInitializable, IDisposable
{
    public event Action OnAllWordsGuessed;
    public event Action OnWordsLoaded;

    public static readonly int WORDS_TO_GUESS_AMOUNT = 1000;
    private const string UNGUESSED_WORDS_RU_KEY = "UnguessedWords";
    private const string UNGUESSED_WORDS_EN_KEY = "UnguessedWordsEN";

    private List<string> _unguessedWords;
    private string[] _validWords;

    private readonly Board _board;

    public WordsController(Board board)
    {
        _board = board;
    }

    public List<string> UnguessedWords => _unguessedWords;
    public string[] ValidWords => _validWords;
    public int GuessedWordsAmount => WORDS_TO_GUESS_AMOUNT - (_unguessedWords?.Count ?? 0);

    public void Initialize()
    {
        LoadValidWords();
        LoadUnguessedWords();

        PlatformBridge.Service.GameReady();

        _board.OnGameOver += GameOverHandler;
        LocalizationSettings.SelectedLocaleChanged += LocaleChangedHandler;
    }

    public void Dispose()
    {
        _board.OnGameOver -= GameOverHandler;
        LocalizationSettings.SelectedLocaleChanged -= LocaleChangedHandler;
    }

    public void ResetProgress()
    {
        _unguessedWords = _validWords.Take(WORDS_TO_GUESS_AMOUNT).ToList();
        SaveProgress();
    }

    private void GameOverHandler(bool isVictory, string word)
    {
        if (isVictory)
        {
            _unguessedWords.Remove(word);

            if (_unguessedWords.Count == 0)
            {
                OnAllWordsGuessed?.Invoke();
                _unguessedWords = _validWords.Take(WORDS_TO_GUESS_AMOUNT).ToList();
            }

            SaveProgress();
        }
    }

    private void LocaleChangedHandler(Locale locale)
    {
        LoadValidWords();
        LoadUnguessedWords();
    }

    private void LoadValidWords()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        TextAsset textFile = Resources.Load<TextAsset>($"words_{currentLocale}");

        if (textFile != null)
        {
            _validWords = textFile.text
                .Split('\n')
                .Select(word => word.Trim())
                .Where(word => !string.IsNullOrEmpty(word))
                .ToArray();
        }
        else
        {
            Debug.LogError($"[WordsController] Words file not found: words_{currentLocale}");
            _validWords = Array.Empty<string>();
        }
    }

    private void LoadUnguessedWords()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        string storageKey = currentLocale == "ru" ? UNGUESSED_WORDS_RU_KEY : UNGUESSED_WORDS_EN_KEY;

        PlatformBridge.Service.LoadData(storageKey, (success, savedString) =>
        {
            if (success && !string.IsNullOrEmpty(savedString))
            {
                _unguessedWords = savedString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                _unguessedWords = _validWords.Take(WORDS_TO_GUESS_AMOUNT).ToList();
                SaveProgress();
            }

            OnWordsLoaded?.Invoke();
        });
    }

    private void SaveProgress()
    {
        if (_unguessedWords == null) return;

        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        string storageKey = currentLocale == "ru" ? UNGUESSED_WORDS_RU_KEY : UNGUESSED_WORDS_EN_KEY;

        string joinedString = string.Join(",", _unguessedWords);
        PlatformBridge.Service.SaveData(storageKey, joinedString);
    }
}
