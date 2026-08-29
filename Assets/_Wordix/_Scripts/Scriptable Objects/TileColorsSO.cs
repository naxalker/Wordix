using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TileColorConfig
{
    [field: SerializeField] public TileState TileState { get; private set; }
    [field: SerializeField] public Color FillColor { get; private set; }
    [field: SerializeField] public Color OutlineColor { get; private set; }
}

[CreateAssetMenu()]
public class TileColorsSO : ScriptableObject
{
    [field: SerializeField] public List<TileColorConfig> DarkThemeValues { get; private set; }
    [field: SerializeField] public List<TileColorConfig> LightThemeValues { get; private set; }
}
