using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Zenject;

public class PlayerInput : IInitializable, ITickable, IDisposable
{
    private static readonly Dictionary<KeyCode, char> ENGLISH_TO_RUSSIAN_MAP = new Dictionary<KeyCode, char>()
    {
        { KeyCode.A, 'ф' }, { KeyCode.B, 'и' }, { KeyCode.C, 'с' },
        { KeyCode.D, 'в' }, { KeyCode.E, 'у' }, { KeyCode.F, 'а' },
        { KeyCode.G, 'п' }, { KeyCode.H, 'р' }, { KeyCode.I, 'ш' },
        { KeyCode.J, 'о' }, { KeyCode.K, 'л' }, { KeyCode.L, 'д' },
        { KeyCode.M, 'ь' }, { KeyCode.N, 'т' }, { KeyCode.O, 'щ' },
        { KeyCode.P, 'з' }, { KeyCode.Q, 'й' }, { KeyCode.R, 'к' },
        { KeyCode.S, 'ы' }, { KeyCode.T, 'е' }, { KeyCode.U, 'г' },
        { KeyCode.V, 'м' }, { KeyCode.W, 'ц' }, { KeyCode.X, 'ч' },
        { KeyCode.Y, 'н' }, { KeyCode.Z, 'я' }, { KeyCode.LeftBracket, 'х' },
        { KeyCode.RightBracket, 'ъ' }, { KeyCode.Semicolon, 'ж' },
        { KeyCode.Quote, 'э' }, { KeyCode.Comma, 'б' }, { KeyCode.Period, 'ю' }
    };

    private static readonly KeyCode[] SUPPORTED_KEYS_RU = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z, KeyCode.LeftBracket, KeyCode.RightBracket,
        KeyCode.Semicolon, KeyCode.Quote, KeyCode.Comma, KeyCode.Period
    };

    private static readonly KeyCode[] SUPPORTED_KEYS_EN = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    public event Action<char> OnLetterKeyPressed;
    public event Action OnClearPressed;
    public event Action OnSubmitPressed;

    private KeyCode[] _supportedKeys = SUPPORTED_KEYS_RU;
    private VirtualKeyboard _activeKeyboard;

    private Board _board;
    private VirtualKeyboard _keyboardRU;
    private VirtualKeyboard _keyboardEN;

    public PlayerInput(
        Board board,
        VirtualKeyboard keyboardRU,
        VirtualKeyboard keyboardEN)
    {
        _board = board;
        _keyboardRU = keyboardRU;
        _keyboardEN = keyboardEN;
    }

    public void Initialize()
    {
        SetActiveKeyboard(LocalizationSettings.SelectedLocale);

        _keyboardRU.OnKeyPressed += LetterKeyPressedHandler;
        _keyboardRU.OnClearPressed += ClearPressedHandler;
        _keyboardRU.OnSubmitPressed += SubmitPressedHandler;

        _keyboardEN.OnKeyPressed += LetterKeyPressedHandler;
        _keyboardEN.OnClearPressed += ClearPressedHandler;
        _keyboardEN.OnSubmitPressed += SubmitPressedHandler;

        Tile.OnTileChangedState += TileChangedStateHandler;
        _board.OnNewGameStarted += NewGameStartedHandler;

        LocalizationSettings.SelectedLocaleChanged += LocaleChangedHandler;
    }

    public void Dispose()
    {
        Tile.OnTileChangedState -= TileChangedStateHandler;
        _board.OnNewGameStarted -= NewGameStartedHandler;

        _keyboardRU.OnKeyPressed -= LetterKeyPressedHandler;
        _keyboardRU.OnClearPressed -= ClearPressedHandler;
        _keyboardRU.OnSubmitPressed -= SubmitPressedHandler;

        _keyboardEN.OnKeyPressed -= LetterKeyPressedHandler;
        _keyboardEN.OnClearPressed -= ClearPressedHandler;
        _keyboardEN.OnSubmitPressed -= SubmitPressedHandler;

        LocalizationSettings.SelectedLocaleChanged -= LocaleChangedHandler;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            OnClearPressed?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmitPressed?.Invoke();
        }
        else
        {
            foreach (KeyCode keyCode in _supportedKeys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (LocalizationSettings.SelectedLocale.Identifier.Code == "ru")
                    {
                        char letter = ENGLISH_TO_RUSSIAN_MAP[keyCode];
                        OnLetterKeyPressed?.Invoke(letter);
                    }
                    else
                    {
                        char letter = keyCode.ToString().ToLower()[0];
                        OnLetterKeyPressed?.Invoke(letter);
                    }
                }
            }
        }
    }

    private void LetterKeyPressedHandler(char letter)
    {
        OnLetterKeyPressed?.Invoke(letter);
    }

    private void SubmitPressedHandler()
    {
        OnSubmitPressed?.Invoke();
    }

    private void ClearPressedHandler()
    {
        OnClearPressed?.Invoke();
    }

    private void TileChangedStateHandler(Tile tile)
    {
        _activeKeyboard.ChangeKeyColor(tile);
    }

    private void NewGameStartedHandler()
    {
        _activeKeyboard.ResetButtons();
    }

    private void LocaleChangedHandler(Locale locale)
    {
        SetActiveKeyboard(locale);
    }

    private void SetActiveKeyboard(Locale locale)
    {
        if (locale.Identifier.Code == "ru")
        {
            _supportedKeys = SUPPORTED_KEYS_RU;
            _activeKeyboard = _keyboardRU;
            _keyboardRU.gameObject.SetActive(true);
            _keyboardEN.gameObject.SetActive(false);
        }
        else
        {
            _supportedKeys = SUPPORTED_KEYS_EN;
            _activeKeyboard = _keyboardEN;
            _keyboardRU.gameObject.SetActive(false);
            _keyboardEN.gameObject.SetActive(true);
        }
    }
}
