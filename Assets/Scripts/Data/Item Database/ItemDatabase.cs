using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Item Database", menuName = "MatOgretici/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public Item.ItemType type;
    public List<Item> items = new List<Item>();
}