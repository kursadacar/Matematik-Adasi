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
        RecalculateUI();
        icon.sprite = targetItem.icon;
        itemNameText.text = item.name;
    }

    void RecalculateUI()
    {
        amountText.text = "x" + targetItem.data.amount.ToString();
        allValueText.text = GameData.ActiveLanguage.shop_sell_item_all;
        valueText.text = (targetItem.data.value / 2).ToString() + GameData.ActiveLanguage.currencySign;
    }
}
