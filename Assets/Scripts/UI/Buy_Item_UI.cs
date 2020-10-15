using UnityEngine;
using UnityEngine.UI;

public class Buy_Item_UI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text itemName;

    [SerializeField] Image valueHolder;
    [SerializeField] Text value;

    private Item targetItem;

    [SerializeField] float previewHoldTime;
    float previewTimer;
    bool isPreviewing;

    public void Activate(Item item)
    {
        targetItem = item;
        icon.sprite = targetItem.icon;
        itemName.text = targetItem.name.ToUpper();
        value.text = targetItem.data.value.ToString() + GameData.ActiveLanguage.currencySign;
    }

    public void Buy()
    {
        ShopUI.Instance.PurchaseItem(targetItem); 
    }

    public void GetReadyForPreview()
    {
        previewTimer += Time.unscaledDeltaTime;
        if(previewTimer > previewHoldTime)
        {
            Debug.Log("Preview item..");
        }
    }

    public void FlushPreviewTimer()
    {
        previewTimer = 0;
    }

    void Update()
    {
        if(targetItem != null)
            valueHolder.color = (Player.Instance.data.gold >= targetItem.data.value) ? ColorDatabase.GoodColor : ColorDatabase.BadColor;
    }
}
