using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : Singleton<ShopUI>
{
    [SerializeField] GameObject BuyMenuParent;
    [SerializeField] GameObject BuyMenuContainer;

    [SerializeField] GameObject SellMenuParent;
    [SerializeField] GameObject SellMenuContainer;

    [SerializeField] Buy_Item_UI buyPrefab;
    [SerializeField] Sell_Item_UI sellPrefab;

    [SerializeField] Text shopMoneyText;

    private Item previewedItem;

    public List<Buy_Item_UI> buy_item_children = new List<Buy_Item_UI>();

    public void Activate()
    {
        OpenBuyMenu();
        StartCoroutine(CheckForInput());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        //StopPreview();
    }

    //public void PreviewItem(Item item)
    //{
    //    foreach (var existingItem in Player.Equipment.Where(x => x.type == item.type))
    //    {
    //        Player.Instance.PreviewUnwearItem(existingItem);
    //    }
    //    if (previewedItem != null)
    //    {
    //        Destroy(previewedItem.gameObject);
    //    }
    //    previewedItem = Instantiate(item, Player.Instance.transform);
    //    previewedItem.Activate();
    //    Player.Instance.PreviewWearItem(previewedItem);
    //}

    //public void StopPreview()
    //{
    //    if (previewedItem != null)
    //    {
    //        Destroy(previewedItem.gameObject);
    //        Destroy(previewedItem.rendererGameObject);
    //    }
    //    foreach (var item in Player.Equipment)
    //    {
    //        Player.Instance.PreviewWearItem(item);
    //    }
    //}

    public void OpenBuyMenu()
    {
        SellMenuParent.SetActive(false);
        BuyMenuParent.SetActive(true);

        var children = BuyMenuContainer.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != BuyMenuContainer.transform)
            {
                Destroy(children[i].gameObject);
            }
        }

        buy_item_children.Clear();
        foreach(var item in GameData.activeShop.ItemDatabase.items)
        {
            var itemMenu = Instantiate(buyPrefab, BuyMenuContainer.transform);
            itemMenu.Activate(item);
            buy_item_children.Add(itemMenu);
        }
    }

    public void OpenSellFishMenu()
    {
        SellMenuParent.SetActive(true);
        BuyMenuParent.SetActive(false);

        var children = SellMenuContainer.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != SellMenuContainer.transform)
            {
                Destroy(children[i].gameObject);
            }
        }

        foreach (var item in Player.Inventory)
        {
            if (item.type != Item.ItemType.Fish)
                continue;
            var itemMenu = Instantiate(sellPrefab, SellMenuContainer.transform);
            itemMenu.Activate(item);
        }
    }

    public void OpenSellEquipmentMenu()
    {
        SellMenuParent.SetActive(true);
        BuyMenuParent.SetActive(false);

        var children = SellMenuContainer.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != SellMenuContainer.transform)
            {
                Destroy(children[i].gameObject);
            }
        }

        foreach (var item in Player.Inventory)
        {
            if (item.type == Item.ItemType.Fish)
                continue;
            var itemMenu = Instantiate(sellPrefab, SellMenuContainer.transform);
            itemMenu.Activate(item);
        }
    }

    public void PurchaseItem(Item item)
    {
        if (Player.Gold < item.data.value)
        {
            Notification.Instance.Display(GameData.ActiveLanguage.shop_not_enough_money, 3f,Notification.NotificationType.BadNews);
            return;
        }

        //StopPreview();
        Player.BuyItem(item);
    }

    public void SellItem(Item item)
    {
        Player.AddGold(item.data.value / 2);

        item.data.amount--;
        if(item.data.amount == 0)
        {
            if (Player.Inventory.Contains(item))
            {
                Player.Inventory.Remove(item);
            }
            if(Player.Equipment.Contains(item))
            {
                Player.Equipment.Remove(item);
            }
            Player.Instance.UnwearItem(item);
            Destroy(item.gameObject);
        }
    }

    public void SellItemAll(Item item)
    {
        Player.AddGold(item.data.value / 2 * item.data.amount);

        item.data.amount = 0;
        if (Player.Inventory.Contains(item))
        {
            Player.Inventory.Remove(item);
        }
        if (Player.Equipment.Contains(item))
        {
            Player.Equipment.Remove(item);
        }
        Player.Instance.PreviewUnwearItem(item);
        Destroy(item.gameObject);
    }

    void Start()
    {

    }

    IEnumerator CheckForInput()
    {
        while (true)
        {
            //Debug.Log("ShopUI-Check for input"); -- Optimized!
            if (GameData.gameMode == GameData.Mode.Shopping)
                shopMoneyText.text = Player.Gold.ToString() + "₺";

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    Player.Instance.transform.Rotate(0f, touch.deltaPosition.x * Time.deltaTime * 180f, 0f, Space.Self);
                }
                yield return null;
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Player.Instance.transform.Rotate(0f, -Input.GetAxis("Mouse X") * Time.deltaTime * 180f, 0f, Space.Self);
            }
            yield return null;
        }
    }
}
