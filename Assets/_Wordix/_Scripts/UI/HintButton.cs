using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HintButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Setup(Action onClick)
    {
        _button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void Enable()
    {
        _button.interactable = true;
    }

    public void Disable()
    {
        _button.interactable = false;
    }
}
