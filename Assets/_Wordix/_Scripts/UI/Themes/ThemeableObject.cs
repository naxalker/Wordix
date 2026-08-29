using UnityEngine;

public abstract class ThemeableObject : MonoBehaviour
{
    protected bool CanChangeColor = true;

    public abstract void ApplyTheme(Theme theme);

    public void Lock() => CanChangeColor = false;
    public void Unlock() => CanChangeColor = true;
}