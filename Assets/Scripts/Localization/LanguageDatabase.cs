using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language Database", menuName = "MatOgretici/Language Database")]
public class LanguageDatabase : ScriptableObject
{
    public List<Language> languages = new List<Language>();
}
