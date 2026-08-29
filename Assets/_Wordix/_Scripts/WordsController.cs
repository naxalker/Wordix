using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YG;
using Zenject;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class WordsController : IInitializable, IDisposable
{
    public event Action OnAllWordsGuessed;
    public event Action OnWordsLoaded;

    public static readonly int WORDS_TO_GUESS_AMOUNT = 1000;
    private const string UNGUESSED_WORDS_RU_KEY = "UnguessedWords";
    private const string UNGUESSED_WORDS_EN_KEY = "UnguessedWordsEN";

    private List<string> _unguessedWords;
    private string[] _validWords;

    private Board _board;

    public WordsController(Board board)
    {
        _board = board;
    }

    public List<string> UnguessedWords => _unguessedWords;
    public string[] ValidWords => _validWords;
    public int GuessedWordsAmount => WORDS_TO_GUESS_AMOUNT - _unguessedWords.Count;

    public void Initialize()
    {
        LoadValidWords();
        LoadUnguessedWords();

        YG2.GameReadyAPI();

        OnWordsLoaded?.Invoke();

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

        OnWordsLoaded?.Invoke();
    }

    private void LoadValidWords()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        TextAsset textFile = Resources.Load($"words_{currentLocale}") as TextAsset;

        _validWords = textFile.text
            .Split('\n')
            .Select(word => word.Trim())
            .ToArray();
    }

    private void LoadUnguessedWords()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        string UNGUESSED_WORDS_KEY = currentLocale == "ru" ?
                                    UNGUESSED_WORDS_RU_KEY :
                                    UNGUESSED_WORDS_EN_KEY;

        if (PlayerPrefs.HasKey(UNGUESSED_WORDS_KEY))
        {
            string savedString = PlayerPrefs.GetString(UNGUESSED_WORDS_KEY);

            _unguessedWords = savedString.Split(',').ToList();
        }
        else
        {
            _unguessedWords = _validWords.Take(WORDS_TO_GUESS_AMOUNT).ToList();
            SaveProgress();
        }
    }

    private void SaveProgress()
    {
        string currentLocale = LocalizationSettings.SelectedLocale.Identifier.Code;
        string UNGUESSED_WORDS_KEY = currentLocale == "ru" ?
                                    UNGUESSED_WORDS_RU_KEY :
                                    UNGUESSED_WORDS_EN_KEY;

        string joinedString = string.Join(",", _unguessedWords.ToArray());
        PlayerPrefs.SetString(UNGUESSED_WORDS_KEY, joinedString);
        PlayerPrefs.Save();
    }
}
