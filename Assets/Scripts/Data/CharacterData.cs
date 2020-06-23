using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public string name;
    public int gold = 10;

    public int characterType;

    public float[] position = new float[3];

    [JsonIgnore, NonSerialized] public List<Item> inventory = new List<Item>();
    [SerializeField] public List<ItemData> _inventorySave = new List<ItemData>();


    [JsonIgnore, NonSerialized] public List<Item> equipment = new List<Item>();
    /// <summary>
    /// inventory id of the equipped items
    /// </summary>
    [SerializeField] public List<int> _equipmentSave = new List<int>();
}