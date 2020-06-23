using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Newtonsoft.Json;
using UnityEditor;

public class GameData : Singleton<GameData>
{
    public enum Mode
    {
        Fishing,
        Shopping,
        Free
    }
    [SerializeField] Mode _GameMode;
    public static Mode gameMode { get { return Instance._GameMode; } set { Instance._GameMode = value; } }

    public static bool isPaused;

    private static bool __isInMenu;
    public static bool isInMenu
    {
        get
        {
            return __isInMenu;
        }
        set
        {
            if (value)
            {
                Player.Instance.nvAgent.enabled = false;
            }
            else
            {
                Player.Instance.nvAgent.enabled = true;
                if (Player.Instance.isMoving)
                    Player.Instance.ResumeOnDestination();
                var shoes = Player.Equipment.Where(x => x.type == Item.ItemType.Shoe);
                if (shoes.Count() > 0)
                    Player.Instance.nvAgent.speed = shoes.ElementAt(0).data.movementSpeed;
                //Debug.Log("In menu..");
            }
            __isInMenu = value;
        }
    }

    public static Shop activeShop;

    #region Common Gameobjects
    [SerializeField] GameObject _xMark;

    public static GameObject XMark => Instance._xMark;
    #endregion

    #region Characters
    [SerializeField] GameObject _characterPrefab;
    [SerializeField] List<Item> _startingItems;

    public static List<Item> StartingItems => Instance._startingItems;
    public static GameObject CharacterPrefab => Instance._characterPrefab;
    #endregion

    #region Localization
    [Header("Localization")]
    [SerializeField] LanguageDatabase _languageDatabase;
    [SerializeField] Language _activeLanguage;
    [SerializeField] Image _languageIcon;
    public static LanguageDatabase LanguageDatabase => Instance._languageDatabase;
    public static Language ActiveLanguage => Instance._activeLanguage;

    public void SwitchLanguage()
    {
        var currentLanguageIndex = LanguageDatabase.languages.IndexOf(ActiveLanguage);
        currentLanguageIndex = (currentLanguageIndex + 1) % LanguageDatabase.languages.Count;
        SwitchToLanguage(LanguageDatabase.languages[currentLanguageIndex]);
        PlayerPrefs.SetInt("Language", currentLanguageIndex);
    }

    private void SwitchToLanguage(Language language)
    {
        _activeLanguage = language;
        _languageIcon.sprite = language.languageIcon;
        UI_Language_Setter.Instance.SetLanguage(_activeLanguage);
    }
    #endregion

    #region Question
    [Header("Questions")]
    [SerializeField] int _questionTime;
    public static int QuestionTime => Instance._questionTime;
    #endregion

    #region Settings
    public static float masterVolume;
    public static float musicVolume;
    #endregion

    #region Menus
    [Header("Menus")]
    [SerializeField] float _menuAnimTime;
    [SerializeField] CanvasGroup _mainMenu, _settingsMenu, _inGameUI,_fishingMenu,_shopMenu, _questionAnswered, _timeout;
    [SerializeField] Image _questionAnsweredImage;
    [SerializeField] Text _questionAnsweredText;
    [SerializeField] Text _startGameText,_fpsText,_moneyText,_rodsLeftText;

    [SerializeField] CanvasGroup _welcomeMenu,_shopTutorialMenu,_fishTutorialMenu;

    [SerializeField] Countdown _countdownBar;

    public static float MenuAnimTime => Instance._menuAnimTime;
    public static List<CanvasGroup> AllMenus = new List<CanvasGroup>();
    public static CanvasGroup MainMenu => Instance._mainMenu;
    public static CanvasGroup SettingsMenu => Instance._settingsMenu;
    public static CanvasGroup InGameUI => Instance._inGameUI;
    public static CanvasGroup FishingMenu => Instance._fishingMenu;
    public static CanvasGroup ShopMenu => Instance._shopMenu;
    public static CanvasGroup QuestionAnsweredScreen => Instance._questionAnswered;
    public static CanvasGroup Timeout => Instance._timeout;

    public static CanvasGroup WelcomeMenu => Instance._welcomeMenu;
    public static CanvasGroup ShopTutorialMenu => Instance._shopTutorialMenu;
    public static CanvasGroup FishTutorialMenu => Instance._fishTutorialMenu;

    public static Image QuestionAnsweredImage => Instance._questionAnsweredImage;
    public static Text QuestionAnsweredText => Instance._questionAnsweredText;
    public static Text StartGameText => Instance._startGameText;
    public static Text FPSText => Instance._fpsText;
    public static Text MoneyText => Instance._moneyText;
    public static Text RodsLeftText => Instance._rodsLeftText;

    public static Countdown CountDownBar => Instance._countdownBar;
    #endregion

    #region Locations
    [Header("Locations")]
    [SerializeField] Transform _fishingSpot;
    [SerializeField] Transform _fishingCameraSpot;
    [SerializeField] Transform _islandCenter;
    [SerializeField] Transform _islandCenterPositionHolder;
    [SerializeField] Transform _playerStartingPosition;

    public static Transform IslandCenter => Instance._islandCenter;
    public static Transform IslandCenterPositionHolder => Instance._islandCenterPositionHolder;
    public static Transform FishingSpot => Instance._fishingSpot;
    public static Transform FishingCameraSpot => Instance._fishingCameraSpot;
    public static Transform PlayerStartingPosition => Instance._playerStartingPosition;
    #endregion

    #region Save
    [Header("SaveGame")]

    private static SaveData _saveData;
    public static SaveData SaveData
    {
        get
        {
            if (_saveData == null)
            {
                _saveData = LoadGame();
                AfterLoad(_saveData);
            }
            return _saveData;
        }
    }

    private static string savePath => Application.persistentDataPath + "/Save.json";

    public void ResetData()
    {
        _saveData = new SaveData();
        SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads data if there is a save present
    /// </summary>
    public static SaveData LoadGame()
    {
        //Creating save for the first time
        if (!File.Exists(savePath))
        {
            var save = new SaveData();
            save.playerData.position = new float[3] { PlayerStartingPosition.position.x, PlayerStartingPosition.position.y, PlayerStartingPosition.position.z };
            File.WriteAllText(savePath, JsonConvert.SerializeObject(save));
        }
        var loadedData =  JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(savePath));

        return loadedData;
    }

    private static void AfterLoad(SaveData loadedData)
    {
        Player.Instance.data = loadedData.playerData;

        //set position
        Player.Instance.transform.position = new Vector3(Player.Instance.data.position[0], Player.Instance.data.position[1], Player.Instance.data.position[2]);

        //set inventory -- by using item id and spawning corresponding item from ItemDatabase
        Player.Instance.data.inventory.Clear();

        for (int i = 0; i < Player.Instance.data._inventorySave.Count; i++)
        {
            var item = Instantiate(ItemDatabase.items[Player.Instance.data._inventorySave[i].ID], Player.Instance.inventoryParent);
            item.Activate();
            item.data = Player.Instance.data._inventorySave[i];
            Player.Inventory.Add(item);
        }

        //set equipment -- by using inventory id to equip item from inventory
        Player.Instance.data.equipment.Clear();
        foreach(var equipmentID in Player.Instance.data._equipmentSave)
        {
            var item = Player.Inventory[equipmentID];
            Player.Instance.WearItem(item);
        }

    }

    /// <summary>
    /// Saves player data to the save asset
    /// </summary>
    public static void SaveGame()
    {
        //Debug.Log("Saved...");
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Formatting = Formatting.Indented;

        //save position
        Player.Instance.data.position = new float[3] { Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z };

        //save inventory by saving the item id for each item
        Player.Instance.data._inventorySave.Clear();
        foreach(var item in Player.Instance.data.inventory)
        {
            Player.Instance.data._inventorySave.Add(item.data);
        }

        //save equipment by saving the inventory id for each item
        Player.Instance.data._equipmentSave.Clear();
        foreach (var equipment in Player.Instance.data.equipment)
        {
            Player.Instance.data._equipmentSave.Add(Player.Instance.data.inventory.IndexOf(equipment));
        }

        SaveData.playerData = Player.Instance.data;

        File.WriteAllText(savePath, JsonConvert.SerializeObject(SaveData, settings));
    }
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] Camera _mainCamera;

    public static Camera MainCamera => Instance._mainCamera;
    #endregion

    #region Item
    [SerializeField] ItemDatabase _itemDatabase, _fishDatabase;

    public static ItemDatabase ItemDatabase => Instance._itemDatabase;
    public static ItemDatabase FishDatabase => Instance._fishDatabase;
    #endregion

    #region Unity Messages
    private void Awake()
    {
        AllMenus = FindObjectsOfType<CanvasGroup>().ToList();
        activeShop = null;
    }
    private void Start()
    {
        SwitchToLanguage(LanguageDatabase.languages[PlayerPrefs.GetInt("Language")]);
    }
    #endregion
}
