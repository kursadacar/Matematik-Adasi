using UnityEngine;
using UnityEngine.UI;

public class Sell_Item_UI : MonoBehaviour
{
    [SerializeField] Image icon;

    [SerializeField] Text amountText;

    [SerializeField] Text valueText;
    [SerializeField] Text allValueText;

    [SerializeField] Text itemNameText;

    private Item targetItem;

    public void SellOne()
    {
        ShopUI.Instance.SellItem(targetItem);

        RecalculateUI();

        if (targetItem.data.amount == 0)
            Destroy(gameObject);
    }

    public void SellAll()
    {
        ShopUI.Instance.SellItemAll(targetItem);

        RecalculateUI();

        Destroy(gameObject);
    }

    public void Activate(Item item)
    {
        targetItem = item;
        amountText.text = targetItem.data.amount.ToString();
        allValueText.text = "(" + (targetItem.data.value / 2 * item.data.amount).ToString() + "₺" + ")";
        valueText.text = "(" + (targetItem.data.value / 2).ToString() + "₺" + ")";
        icon.sprite = targetItem.icon;
        itemNameText.text = item.name;
    }

    void RecalculateUI()
    {
        amountText.text = targetItem.data.amount.ToString();
        allValueText.text = "(" + (targetItem.data.value / 2 * targetItem.data.amount).ToString() + "₺" + ")";
        valueText.text = "(" + (targetItem.data.value / 2).ToString() + "₺" + ")";
    }
}
