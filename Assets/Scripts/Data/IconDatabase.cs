using UnityEngine;
using System.Collections;

public class IconDatabase : Singleton<IconDatabase>
{
    [SerializeField] private Sprite _previewOnIcon;
    [SerializeField] private Sprite _previewOffIcon;

    public static Sprite PreviewOnIcon => Instance._previewOnIcon;
    public static Sprite PreviewOffIcon => Instance._previewOffIcon;
}
