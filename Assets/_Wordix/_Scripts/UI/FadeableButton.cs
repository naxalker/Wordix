using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FadeableButton : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _outline;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private bool _fadeIn;
    [SerializeField] private bool _fadeOut;

    public void Show(float duration)
    {
        if (gameObject.activeInHierarchy)
            return;

        gameObject.SetActive(true);

        if (_fadeIn)
        {
            _background.DOFade(1f, duration).SetEase(Ease.Linear);
            _outline.DOFade(1f, duration).SetEase(Ease.Linear);
            _text.DOFade(1f, duration).SetEase(Ease.Linear);
        }
    }

    public void Hide(float duration)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (_fadeOut)
        {
            _outline
                .DOFade(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => gameObject.SetActive(false));

            _background
                .DOFade(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => gameObject.SetActive(false));

            _text
                .DOFade(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            Color color = _background.color;
            color.a = 0f;
            _background.color = color;

            color = _outline.color;
            color.a = 0f;
            _outline.color = color;

            color = _text.color;
            color.a = 0f;
            _text.color = color;

            gameObject.SetActive(false);
        }
    }
}
