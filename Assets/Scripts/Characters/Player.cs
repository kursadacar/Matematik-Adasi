using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using MTAssets.SkinnedMeshCombiner;
using System.Net.Sockets;
using System;

public class Player : Character
{
    #region Singleton
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }
    #endregion

    [SerializeField] internal Animator animator;
    [SerializeField] internal NavMeshAgent nvAgent;

    public Transform inventoryParent, armature;

    public CharacterData data;

    public static List<Item> Inventory => Instance.data.inventory;
    public static List<Item> Equipment => Instance.data.equipment;

    public Transform headPosition;
    [SerializeField] AudioSource audioSrc;

    RaycastHit hit;

    public Vector3 lastDestination;

    public bool isMoving;

    private SkinnedMeshCombiner smCombiner;

    public static Item Rod
    {
        get
        {
            var rods = Equipment.Where(x => x.type == Item.ItemType.Rod);
            if (rods.Count() > 0)
                return rods.ElementAt(0);
            else return null;
        }
    }

    public static int Gold => Instance.data.gold;
    public static string Name { get
        {
            return Instance.data.name;
        } set
        {
            Instance.data.name = value;
        }
    }

    private enum DestinationType
    {
        Fishing,
        Shop,
        Free
    }

    private void Awake()
    {
        smCombiner = GetComponent<SkinnedMeshCombiner>();
        smCombiner.mergeMethod = SkinnedMeshCombiner.MergeMethod.OneMeshPerMaterial;
    }
    void Start()
    {
        foreach(var item in data.inventory)
        {
            if(item.data.amount == 0)
            {
                data.inventory.Remove(item);
            }
        }
    }

    void Update()
    {
        animator.SetFloat("Speed", nvAgent.velocity.magnitude);

        //Footstep sound
        if (!GameData.isPaused && nvAgent.velocity.magnitude > 1f && !audioSrc.isPlaying)
        {
            audioSrc.PlayOneShot(SoundDatabase.GetRandomFootstep());
        }
        //Clicks
        if (!GameData.isInMenu)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    ClickOnPosition(touch.position);
                    return;
                }
            }
            if (GameData.gameMode == GameData.Mode.Free)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ClickOnPosition(Input.mousePosition);
                }
            }
        }
    }

    public void SetName(string name)
    {
        data.name = name;
    }

    public void DisableDestinationMarker()
    {
        GameData.XMark.SetActive(false);
    }


    public static void BuyItem(Item item)
    {
        Item boughtItem = null;

        if (Inventory.Where(x => x.data.ID == item.data.ID).Count() > 0)
        {
            boughtItem = Inventory.Where(x => x.data.ID == item.data.ID).ElementAt(0);
            boughtItem.data.amount++;
        }
        else
        {
            boughtItem = Instantiate(item, Instance.inventoryParent); ;
            boughtItem.Activate();
            Inventory.Add(boughtItem);
        }
        if (item.type != Item.ItemType.Fish || item.type != Item.ItemType.None)
        {
            Instance.WearItem(boughtItem);
        }
        AddGold(-boughtItem.data.value);

        Notification.Instance.Display(string.Format(GameData.ActiveLanguage.shop_you_have_bought, boughtItem.name), boughtItem.icon, 3f, Notification.NotificationType.GoodNews);
    }

    private void ClickOnPosition(Vector3 position)
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(GameData.MainCamera.ScreenPointToRay(position), out hit))
        {
            var comp = hit.transform.gameObject.GetComponent<TouchSpot>();
            if (comp != null)
            {
                switch (comp.type)
                {
                    case TouchSpot.SpotType.Shop:
                        {
                            GoShop(comp.target);
                        }
                        break;
                    case TouchSpot.SpotType.FishingSpot:
                        {
                            GoFishing();
                        }
                        break;
                }
                GameData.XMark.SetActive(false);
            }
            else
            {
                SetDestination(hit.point, DestinationType.Free);
                GameData.XMark.SetActive(true);
                GameData.XMark.transform.position = hit.point + hit.normal * 0.1f;
                GameData.XMark.transform.rotation = Quaternion.LookRotation(hit.normal,transform.position - hit.point);
                GameData.activeShop = null;
            }
        }
    }

    public void ResumeOnDestination()
    {
        SetDestination(lastDestination, DestinationType.Free);
    }

    public void GoFishing()
    {
        StopMoving();
        SetDestination(GameData.FishingSpot.position, DestinationType.Fishing);
    }

    public void GoNearestShop()
    {
        float closestDist = Mathf.Infinity;
        Shop closestShop = null;
        foreach(var shop in FindObjectsOfType<Shop>())
        {
            var dist = Vector3.Distance(shop.transform.position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestShop = shop;
            }
        }
        GoShop(closestShop);
    }
    public void GoShop(Shop shop)
    {
        StopMoving();
        GameData.activeShop = shop;
        SetDestination(GameData.activeShop.playerStandPoint.position, DestinationType.Shop);
    }

    private void StartFishing()
    {
        if(Rod == null)
        {
            Notification.Instance.Display(GameData.ActiveLanguage.fishing_you_need_rod, 5f, Notification.NotificationType.Warning);
            return;
        }

        if (GameData.SaveData.FishingFirstTime)
        {
            GameData.isInMenu = true;
            UI_Manager.Instance.OpenMenu(GameData.FishTutorialMenu);
            GameData.SaveData.FishingFirstTime = false;
            return;
        }

        StartFishingAnim();
        GameManager.Instance.SwitchGameMode(GameData.Mode.Fishing);
        StartCoroutine(LerpToPosition(GameData.FishingSpot.position, GameData.FishingSpot.rotation, 0.7f));
    }

    private void StartShop()
    {
        if (GameData.SaveData.ShoppingFirstTime)
        {
            GameData.isInMenu = true;
            UI_Manager.Instance.OpenMenu(GameData.ShopTutorialMenu);
            GameData.SaveData.ShoppingFirstTime = false;
            return;
        }
        GameManager.Instance.SwitchGameMode(GameData.Mode.Shopping);
        StartCoroutine(LerpToPosition(GameData.activeShop.playerStandPoint.position, GameData.activeShop.playerStandPoint.rotation, 0.7f));
    }

    public void StopMoving()
    {
        StopAllCoroutines();
        StartFreeMove();
        GameManager.Instance.SwitchGameMode(GameData.Mode.Free);
        nvAgent.SetDestination(transform.position);
    }

    private void SetDestination(Vector3 destination,DestinationType type)
    {
        StopAllCoroutines();
        nvAgent.SetDestination(destination);
        StartCoroutine(CheckDestination(type,destination));
        lastDestination = hit.point;
        isMoving = true;
    }

    private void Arrived(DestinationType type)
    {
        switch (type)
        {
            case DestinationType.Fishing:
                {
                    StartFishing();
                }
                break;
            case DestinationType.Shop:
                {
                    StartShop();
                }
                break;
            case DestinationType.Free:
                {
                    //TODO: when arrived at a free destination
                }
                break;
        }
        GameData.XMark.SetActive(false);
        isMoving = false;
    }

    private IEnumerator LerpToPosition(Vector3 targetPosition, Quaternion targetRotation, float arriveTime)
    {
        float timer = 0f;

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (true)
        {
            //Debug.Log("Player - Lerp to Position"); -- Optimized!
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(startingPos, targetPosition, timer / arriveTime);
            transform.rotation = Quaternion.Lerp(startingRot, targetRotation, timer / arriveTime);

            if(timer > arriveTime)
            {
                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Check if arrived to destination
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckDestination(DestinationType type,Vector3 destination)
    {
        while (true)
        {
            //Debug.Log("Player - CheckDestination"); -- Optimized!
            if (Vector3.Distance(transform.position,destination) < 0.5f)
            {
                Arrived(type);
                yield break;
            }

            yield return null;
        }
    }

    public static void AddGold(int amount)
    {
        Instance.data.gold += amount;
        GameData.MoneyText.text = Instance.data.gold.ToString() + "₺";
    }

    public void WearItem(Item item)
    {
        if (item.type == Item.ItemType.Fish || item.type == Item.ItemType.None)
            return;

        if(item.type == Item.ItemType.Shoe)
        {
            Instance.nvAgent.speed = item.data.movementSpeed;
        }

        var sameItems = Equipment.Where(x => x.type == item.type);
        if (sameItems.Count() > 0)
            UnwearItem(sameItems.ElementAt(0));

        Equipment.Add(item);

        if (item.type == Item.ItemType.Rod)
            return;

        foreach (var child in item.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player");
        }

        var reassignWeights = item.rendererGameObject.GetComponent<ReassignBoneWeigthsToNewMesh>();
        item.transform.position = transform.position;
        item.rendererGameObject.transform.parent = transform;

        if(smCombiner.isMeshesCombined())
            smCombiner.UndoCombineMeshes(true,true);

        reassignWeights.newArmature = Instance.armature;
        reassignWeights.rootBoneName = "Spine1";
        reassignWeights.Reassign();


        smCombiner.CombineMeshes();
    }

    public void UnwearItem(Item item)
    {
        if (Equipment.Contains(item))
        {
            Equipment.Remove(item);
        }

        if(smCombiner.isMeshesCombined())
            smCombiner.UndoCombineMeshes(true,true);

        item.rendererGameObject.transform.parent = item.transform;

        if (item.type == Item.ItemType.Shoe)
        {
            Instance.nvAgent.speed = 3f;
        }
        smCombiner.CombineMeshes();
    }

    /// <summary>
    /// Set parent of item's renderer to player. Reassign bone weights.
    /// </summary>
    /// <param name="item"></param>
    [Obsolete]
    public void PreviewWearItem(Item item)
    {
        if (item.type == Item.ItemType.Fish || item.type == Item.ItemType.None || item.type == Item.ItemType.Rod)
            return;

        foreach (var child in item.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player");
        }
        var comp = item.rendererGameObject.GetComponent<ReassignBoneWeigthsToNewMesh>();
        //item.rendererGameObject.transform.parent = transform;
        comp.newArmature = Instance.armature;
        comp.rootBoneName = "Spine1";
        comp.Reassign();
    }

    /// <summary>
    /// Uncombine mesh, set item renderer back to its parent, recombine rest of the items
    /// </summary>
    /// <param name="item"></param>
    [Obsolete]
    public void PreviewUnwearItem(Item item)
    {
        item.rendererGameObject.transform.parent = item.transform;

        Destroy(item.gameObject);
    }

    private void StartFishingAnim()
    {
        animator.SetBool("Fishing", true);
    }

    private void StartFreeMove()
    {
        animator.SetBool("Fishing", false);
    }
}
