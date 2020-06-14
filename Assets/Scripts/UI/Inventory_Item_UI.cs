using UnityEngine;
using UnityEngine.UI;

public class Inventory_Item_UI : MonoBehaviour
{
    public InventoryUI parent;

    [SerializeField] Text Amount, Name;
    [SerializeField] Image icon;
    [SerializeField] Button wearButton;
    [SerializeField] Image wearButtonIcon;

    Item targetItem;
    private bool isWearing;

    public void RemoveItem()
    {
        if (Player.Inventory.Contains(targetItem))
        {
            Player.Inventory.Remove(targetItem);
        }
        Destroy(targetItem.gameObject);

        Destroy(gameObject);
    }

    public void WearItem()
    {
        if (isWearing)
        {
            Player.Instance.UnwearItem(targetItem);
            isWearing = false;
        }
        else
        {
            Player.Instance.WearItem(targetItem);
            isWearing = true;
        }
        foreach(var child in parent.children)
        {
            child.RefreshWearButton();
        }
    }

    private void RefreshWearButton()
    {
        isWearing = Player.Equipment.Contains(targetItem);
        if (isWearing)
        {
            wearButtonIcon.sprite = IconDatabase.PreviewOffIcon;
            wearButtonIcon.color = ColorDatabase.PreviewOffColor;
        }
        else
        {
            wearButtonIcon.sprite = IconDatabase.PreviewOnIcon;
            wearButtonIcon.color = ColorDatabase.PreviewOnColor;
        }
    }

    public void Activate(Item item)
    {
        Amount.text = "x" + item.data.amount.ToString();
        Name.text = item.name;
        icon.sprite = item.icon;
        targetItem = item;
        if (item.type == Item.ItemType.Fish || item.type == Item.ItemType.None || item.type == Item.ItemType.Rod)
            wearButton.gameObject.SetActive(false);
        
        RefreshWearButton();
    }
}
