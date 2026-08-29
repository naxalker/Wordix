using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    public Action<bool> OnToggled;

    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)] private float _sliderValue;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float _animationDuration = 0.5f;
    [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _currentValue = true;
    private Slider _slider;
    private Coroutine _animateSliderCoroutine;

    public void Setup(float sliderValue)
    {
        SetupSliderComponent();

        _sliderValue = Mathf.Clamp01(sliderValue);
        _slider.value = _sliderValue;
        _currentValue = _sliderValue > 0.5f;
    }

    protected virtual void OnValidate()
    {
        SetupToggleComponents();

        _slider.value = _sliderValue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void SetupToggleComponents()
    {
        if (_slider != null)
            return;

        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            Debug.Log("No slider found!", this);
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    private void Toggle()
    {
        _currentValue = !_currentValue;

        OnToggled?.Invoke(_currentValue);

        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }

    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = _currentValue ? 1 : 0;

        float time = 0;
        if (_animationDuration > 0)
        {
            while (time < _animationDuration)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / _animationDuration);
                _slider.value = _sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                yield return null;
            }
        }

        _slider.value = endValue;
    }
}