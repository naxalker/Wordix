using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
    [SerializeField] private List<Button> _returnButtons;

    private void Awake()
    {
        foreach (Button button in _returnButtons)
        {
            button.onClick.AddListener(() => Hide());
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
