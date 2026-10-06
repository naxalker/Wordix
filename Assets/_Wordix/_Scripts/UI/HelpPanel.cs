using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HelpPanel : MonoBehaviour
{
    [SerializeField] private List<Button> _returnButtons;

    private PlayerInput _playerInput;

    [Inject]
    private void Construct(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }

    private void Awake()
    {
        foreach (Button button in _returnButtons)
        {
            button.onClick.AddListener(() => Hide());
        }
    }

    private void OnEnable() => _playerInput.Block();

    private void OnDisable() => _playerInput.Unblock();

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
