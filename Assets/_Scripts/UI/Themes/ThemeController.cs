using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

public class ThemeController : IInitializable, IDisposable
{
    private static string DARK_CAMERA_COLOR = "#1D1D1D";
    private static string LIGHT_CAMERA_COLOR = "#E2E2E2";

    private List<ThemeableObject> _themeableObjects = new List<ThemeableObject>();
    private Theme _currentTheme = Theme.Dark;

    private Board _board;

    public ThemeController(Board board)
    {
        _board = board;
    }

    public void Initialize()
    {
        _themeableObjects.AddRange(
            UnityEngine.Object.FindObjectsByType<ThemeableObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            )
        );

        _board.OnNewGameStarted += NewGameStartedHandler;
    }
    public void Dispose()
    {
        _board.OnNewGameStarted -= NewGameStartedHandler;
    }

    public void ChangeColors()
    {
        _currentTheme = _currentTheme == Theme.Dark ? Theme.Light : Theme.Dark;

        if (_currentTheme == Theme.Dark)
        {
            if (ColorUtility.TryParseHtmlString(DARK_CAMERA_COLOR, out Color newColor))
            {
                Camera.main.backgroundColor = newColor;
            }
        }
        else if (_currentTheme == Theme.Light)
        {
            if (ColorUtility.TryParseHtmlString(LIGHT_CAMERA_COLOR, out Color newColor))
            {
                Camera.main.backgroundColor = newColor;
            }
        }

        foreach (ThemeableObject themeableObject in _themeableObjects)
        {
            themeableObject.ApplyTheme(_currentTheme);
        }
    }

    private void NewGameStartedHandler()
    {
        foreach (ThemeableObject themeableObject in _themeableObjects)
        {
            themeableObject.Unlock();
            themeableObject.ApplyTheme(_currentTheme);
        }
    }
}
