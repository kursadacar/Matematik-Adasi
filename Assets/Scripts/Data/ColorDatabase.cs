using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorDatabase : Singleton<ColorDatabase>
{
    [SerializeField] private Color _previewOnColor, _previewOffColor;

    [SerializeField] private Color _goodColor, _badColor;

    public static Color PreviewOnColor => Instance._previewOnColor;
    public static Color PreviewOffColor => Instance._previewOffColor;

    public static Color GoodColor => Instance._goodColor;
    public static Color BadColor => Instance._badColor;

}
