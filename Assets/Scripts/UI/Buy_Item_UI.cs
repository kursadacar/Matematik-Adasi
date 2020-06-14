using UnityEngine;
using UnityEngine.UI;

public class Buy_Item_UI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text itemName;
    [SerializeField] Text value;

    private Item targetItem;
    private bool isPreviewing;

    public void Activate(Item item)
    {
        targetItem = item;
        icon.sprite = targetItem.icon;
        itemName.text = targetItem.name.ToUpper();
        value.text = targetItem.data.value.ToString() + "₺";
    }

    public void Buy()
    {
        ShopUI.Instance.PurchaseItem(targetItem);
    }

    void Update()
    {
        if(targetItem != null)
            value.color = (Player.Instance.data.gold >= targetItem.data.value) ? Color.green : Color.red;
    }
}
