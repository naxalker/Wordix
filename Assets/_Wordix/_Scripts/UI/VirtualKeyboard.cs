using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    public event Action<char> OnKeyPressed;
    public event Action OnClearPressed;
    public event Action OnSubmitPressed;

    [Header("Keyboard Buttons")]
    [SerializeField] private Button[] _letterButtons;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _submitButton;

    private Dictionary<char, Button> _letterToButton = new Dictionary<char, Button>();

    private void Awake()
    {
        foreach (Button button in _letterButtons)
        {
            char letter = button.GetComponentInChildren<TMP_Text>().text[0];
            _letterToButton[letter] = button;

            button.onClick.AddListener(() => OnKeyPressed?.Invoke(letter));
        }

        _clearButton.onClick.AddListener(() => OnClearPressed?.Invoke());
        _submitButton.onClick.AddListener(() => OnSubmitPressed?.Invoke());
    }

    public void ChangeKeyColor(Tile tile)
    {
        if (tile.State == TileState.EmptyState || tile.State == TileState.OccupiedState) { return; }

        Button letterButton = _letterToButton[char.ToLower(tile.Letter)];

        if (letterButton.TryGetComponent(out KeyboardButton keyboardButton))
        {
            if (tile.State == TileState.CorrectState || keyboardButton.ColorHasChanged == false)
            {
                keyboardButton.ChangeColor(tile.FillColor);
            }
        }
    }

    public void ResetButtons()
    {
        foreach (Button button in _letterButtons)
        {
            if (button.TryGetComponent(out KeyboardButton keyboardButton))
            {
                keyboardButton.ResetButton();
            }
        }
    }
}
