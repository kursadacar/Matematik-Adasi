using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform ItemsContainer;
    [SerializeField] Inventory_Item_UI item_ui_prefab;
    [SerializeField] Transform PlayerTargetPosition;
    [SerializeField] Text playerNameField;

    [SerializeField] Image switchButtonImage;
    [SerializeField] Text switchButtonText;

    [SerializeField] Color equipmentColor, fishColor;

    Vector3 playerOriginalPosition;
    Quaternion playerOriginalRotation;

    public List<Inventory_Item_UI> children = new List<Inventory_Item_UI>();

    public enum Type
    {
        Equipment,
        Fish
    }
    [SerializeField]public Type displayType;

    public void Activate()
    {
        GameData.isInMenu = true;
        playerNameField.text = Player.Name;

        CameraController.Instance.mode = CameraController.CameraMode.Fixed;
        CameraController.Instance.IsolatePlayer();

        BringItems(Type.Fish);

        StartCoroutine(TransportPlayerToPosition(0.6f));
        StartCoroutine(CheckForInput());
    }

    private void BringItems(Type type)
    {
        //Debug.Log("Switch");
        displayType = type;

        switch (displayType)
        {
            case Type.Equipment:
                {
                    switchButtonText.text = GameData.ActiveLanguage.inventory_view_fish;
                    switchButtonImage.color = fishColor;
                }
                break;
            case Type.Fish:
                {
                    switchButtonText.text = GameData.ActiveLanguage.inventory_view_equipment;
                    switchButtonImage.color = equipmentColor;
                }
                break;
        }

        foreach (var child in ItemsContainer.GetComponentsInChildren<Transform>())
        {
            if (child != ItemsContainer)
                Destroy(child.gameObject);
        }

        children.Clear();
        foreach (var item in Player.Inventory)
        {
            if (type == Type.Fish && item.type != Item.ItemType.Fish)
                continue;
            if (type == Type.Equipment && item.type == Item.ItemType.Fish)
                continue;
            var ui_display = Instantiate(item_ui_prefab, ItemsContainer);
            ui_display.Activate(item);
            ui_display.parent = this;
            children.Add(ui_display);
        }
    }

    public void SwitchDisplay()
    {
        if(displayType == Type.Equipment)
        {
            BringItems(Type.Fish);
        }else if(displayType == Type.Fish)
        {
            BringItems(Type.Equipment);
        }
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        CameraController.Instance.StopIsolatingPlayer();
        StartCoroutine(TransportPlayerBack(0.6f));
    }

    private void ReturnToGame()
    {
        GameData.isInMenu = false;
        UI_Manager.Instance.OpenMenu(GameData.InGameUI);
        CameraController.Instance.mode = CameraController.CameraMode.FollowPlayer;
    }

    private IEnumerator CheckForInput()
    {
        while (true)
        {
            //Debug.Log("InventoryUI - CheckForInput"); --Optimized!
            if (EventSystem.current.IsPointerOverGameObject())
                yield return null;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Moved)
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

    private IEnumerator TransportPlayerToPosition(float animTime)
    {
        float timer = 0f;

        playerOriginalPosition = Player.Instance.transform.position;
        playerOriginalRotation = Player.Instance.transform.rotation;

        while (true)
        {
            //Debug.Log("InventoryUI - TransportPlayerToPosition"); -- Optimized!
            timer += Time.unscaledDeltaTime;

            Player.Instance.transform.position = Vector3.Lerp(playerOriginalPosition, PlayerTargetPosition.position, timer / animTime);

            if((Player.Instance.transform.position - PlayerTargetPosition.position).magnitude > 0.1f)
            {
                Player.Instance.transform.rotation = PlayerTargetPosition.rotation;
            }

            yield return null;
        }
    }

    private IEnumerator TransportPlayerBack(float animTime)
    {
        float timer = 0f;
        while (true)
        {
            //Debug.Log("InventoryUI - TransportPlayerBack"); --Optimized!
            timer += Time.unscaledDeltaTime;

            Player.Instance.transform.position = Vector3.Lerp(PlayerTargetPosition.position, playerOriginalPosition, timer / animTime);
            Player.Instance.transform.rotation = Quaternion.Lerp(PlayerTargetPosition.rotation, playerOriginalRotation, timer / animTime);

            if (timer > animTime)
            {
                ReturnToGame();
                yield break;
            }

            yield return null;
        }
    }
}
