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

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Image image))
            {
                image.color =
                    GetComponent<Image>().color * _button.colors.normalColor;
            }
        }
    }

    public void Disable()
    {
        _button.interactable = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Image image))
            {
                image.color =
                    GetComponent<Image>().color * _button.colors.disabledColor;
            }
        }
    }
}
