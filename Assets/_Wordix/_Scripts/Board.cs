using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public event Action OnNewGameStarted;
    public event Action OnLetterPlaced;
    public event Action OnLetterRemoved;
    public event Action OnInvalidWord;
    public event Action OnValidWordEntered;
    public event Action<bool, string> OnGameOver;

    private bool _isActive;

    private Row[] _rows;

    private string _word;
    private char[] _guessedLettersInWord = new char[5];

    private int _rowIndex;
    private int _columnIndex;

    private PlayerInput _playerInput;
    private WordsController _wordsController;

    [Inject]
    private void Construct(PlayerInput playerInput, WordsController wordsController)
    {
        _playerInput = playerInput;
        _wordsController = wordsController;
    }

    public string Word => _word;
    public char[] GuessedLettersInWord => _guessedLettersInWord;

    private List<string> UnguessedWords => _wordsController.UnguessedWords;
    private string[] ValidWords => _wordsController.ValidWords;

    private void Awake()
    {
        _rows = GetComponentsInChildren<Row>();
    }

    private void OnEnable()
    {
        _playerInput.OnLetterKeyPressed += PlaceLetter;
        _playerInput.OnClearPressed += RemoveLetter;
        _playerInput.OnSubmitPressed += SubmitWord;

        _wordsController.OnWordsLoaded += StartNewGame;
    }

    private void OnDisable()
    {
        _playerInput.OnLetterKeyPressed -= PlaceLetter;
        _playerInput.OnClearPressed -= RemoveLetter;
        _playerInput.OnSubmitPressed -= SubmitWord;

        _wordsController.OnWordsLoaded -= StartNewGame;
    }

    public void StartNewGame()
    {
        ClearBoard();
        SetRandomWord();

        for (int i = 0; i < _guessedLettersInWord.Length; i++)
        {
            _guessedLettersInWord[i] = '\0';
        }

        _isActive = true;

        PlatformBridge.Service.LevelStarted();

        OnNewGameStarted?.Invoke();
    }

    public void PlaceLetter(char letter)
    {
        if (!_isActive) { return; }

        Row currentRow = _rows[_rowIndex];

        if (_columnIndex >= currentRow.Tiles.Length)
            return;

        currentRow.Tiles[_columnIndex].SetLetter(letter);
        currentRow.Tiles[_columnIndex].SetState(TileState.OccupiedState);
        _columnIndex++;

        OnLetterPlaced?.Invoke();
    }

    public void SubmitWord()
    {
        if (!_isActive) { return; }

        Row currentRow = _rows[_rowIndex];

        if (_columnIndex >= currentRow.Tiles.Length)
        {
            StartCoroutine(SubmitRow(currentRow));
        }
    }

    public void RemoveLetter()
    {
        if (!_isActive) { return; }
        if (_columnIndex == 0) { return; }

        Row currentRow = _rows[_rowIndex];

        _columnIndex--;

        currentRow.Tiles[_columnIndex].SetLetter('\0');
        currentRow.Tiles[_columnIndex].SetState(TileState.EmptyState);

        OnLetterRemoved?.Invoke();
    }

    private void SetRandomWord()
    {
        _word = UnguessedWords[Random.Range(0, UnguessedWords.Count)];
        _word = _word.ToLower().Trim();

#if UNITY_EDITOR
        Debug.Log(_word);
#endif
    }

    private IEnumerator SubmitRow(Row row)
    {
        if (!IsValidWord(row.Word))
        {
            row.Shake();

            OnInvalidWord?.Invoke();
            yield break;
        }

        _isActive = false;

        OnValidWordEntered?.Invoke();

        string remaining = _word;

        for (int i = 0; i < row.Tiles.Length; i++)
        {
            Tile tile = row.Tiles[i];

            if (tile.Letter == _word[i])
            {
                tile.SetState(TileState.CorrectState);
                _guessedLettersInWord[i] = _word[i];

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!_word.Contains(tile.Letter))
            {
                tile.SetState(TileState.IncorrectState);
            }
        }

        for (int i = 0; i < row.Tiles.Length; i++)
        {
            Tile tile = row.Tiles[i];

            if (tile.State != TileState.CorrectState && tile.State != TileState.IncorrectState)
            {
                if (remaining.Contains(tile.Letter))
                {
                    tile.SetState(TileState.WrongSpotState);

                    int index = remaining.IndexOf(tile.Letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(TileState.IncorrectState);
                }
            }
        }

        foreach (Tile tile in row.Tiles)
        {
            yield return StartCoroutine(tile.Flip());
        }

        if (HasWon(row))
        {
            PlatformBridge.Service.LevelCompleted();
            OnGameOver?.Invoke(true, _word);
        }
        else
        {
            _rowIndex++;
            _columnIndex = 0;

            if (_rowIndex >= _rows.Length)
            {
                PlatformBridge.Service.LevelFailed();
                OnGameOver?.Invoke(false, _word);
            }
        }

        _isActive = true;
    }

    private void ClearBoard()
    {
        for (int row = 0; row < _rows.Length; row++)
        {
            for (int col = 0; col < _rows[row].Tiles.Length; col++)
            {
                _rows[row].Tiles[col].SetLetter('\0');
                _rows[row].Tiles[col].SetState(TileState.EmptyState);
            }
        }

        _rowIndex = 0;
        _columnIndex = 0;
    }

    private bool IsValidWord(string word) => ValidWords.Contains(word);

    private bool HasWon(Row row) => row.Tiles.All(tile => tile.State == TileState.CorrectState);
}
