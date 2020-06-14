using Newtonsoft.Json;
using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public SkinnedMeshRenderer smRenderer;
    public GameObject rendererGameObject;
    public new string name
    {
        get
        {
            string _return = "";

            var subStrings = data.name.Split('|');
            foreach (var sub in subStrings)
            {
                if (sub.StartsWith(GameData.ActiveLanguage.languagePrefix))
                {
                    _return = sub;
                    break;
                }
            }
            if (_return.Length > 0)
            {
                _return = _return.Remove(0, 3);
            }
            return _return;
        }
    }

    public ItemType type => data.type;

    public enum ItemType
    {
        None,
        Fish,
        Rod,
        Hat,
        Shoe,
        Tshirt,
        Pants
    }

    public void Activate()
    {
        smRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (smRenderer != null)
        {
            rendererGameObject = smRenderer.gameObject;
        }
    }

    public ItemData data;

    public void SetData(ItemData data)
    {
        this.data = data;
    }

    /// <summary>
    /// Use only in the CreateItemIcons class!!
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        dbIcon = icon;
    }

    [JsonIgnore, SerializeField] Sprite dbIcon;
    public Sprite icon => GameData.ItemDatabase.items[data.ID].dbIcon;

    public Item(ItemData data)
    {
        this.data = data;
    }
}

[Serializable]
public class ItemData
{
    [HideInInspector] public int ID;

    public Item.ItemType type;

    public string name;
    public int amount;
    public int value;

    public float movementSpeed;

    public int durability;
    public int fishMultiplier;
}