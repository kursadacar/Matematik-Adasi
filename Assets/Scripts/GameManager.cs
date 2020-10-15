using UnityEngine;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine.Rendering;

public class GameManager : Singleton<GameManager>
{
    public void StartGame()
    {
        if (GameData.SaveData.PlayingFirstTime)
        {
            UI_Manager.Instance.OpenMenu(GameData.WelcomeMenu);
            GameData.SaveData.PlayingFirstTime = false;
            foreach(var item in GameData.StartingItems)
            {
                Player.AddGold(item.data.value);
                Player.BuyItem(item);
            }
            return;
        }

        Time.timeScale = 1f;

        CameraController.Instance.mode = CameraController.CameraMode.FollowPlayer;
        Player.Instance.StopMoving();

        GameData.isPaused = false;
        GameData.isInMenu = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        CameraController.Instance.mode = CameraController.CameraMode.RotateAroundIsland;

        GameData.StartGameText.text = GameData.ActiveLanguage.main_menu_resume_game;

        GameData.isPaused = true;
        GameData.isInMenu = true;
    }

    void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = 0;
#endif
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);

        Time.timeScale = 0f;
        GameData.isPaused = true;
        GameData.isInMenu = true;
    }

    public void SwitchGameMode(GameData.Mode mode)
    {
        GameData.gameMode = mode;
        switch (mode)
        {
            case GameData.Mode.Fishing:
                {
                    CameraController.Instance.mode = CameraController.CameraMode.Fishing;
                    UI_Manager.Instance.OpenMenu(GameData.FishingMenu);
                    Question.Instance.RegenerateRandomQuestion();
                    Player.Instance.EquipRod();
                    GameData.isInMenu = true;
                }
                break;
            case GameData.Mode.Shopping:
                {
                    CameraController.Instance.mode = CameraController.CameraMode.Shopping;
                    UI_Manager.Instance.OpenMenu(GameData.ShopMenu);
                    ShopUI.Instance.Activate();
                    GameData.isInMenu = true;
                }
                break;
            case GameData.Mode.Free:
                {
                    CameraController.Instance.mode = CameraController.CameraMode.FollowPlayer;
                    UI_Manager.Instance.OpenMenu(GameData.InGameUI);

                    GameData.MoneyText.text = Player.Instance.data.gold.ToString() + GameData.ActiveLanguage.currencySign;
                    GameData.isInMenu = false;

                    Player.Instance.UnequipRod();

                    if (Player.Instance.isMoving)
                        Player.Instance.ResumeOnDestination();
                    GameData.SaveGame();
                }
                break;
        }
    }

    private void ReduceRodDurability()
    {
        Player.Rod.data.durability--;
        Notification.Instance.Display(GameData.ActiveLanguage.fishing_rod_health_reduced, 3f, Notification.NotificationType.BadNews);
        if (Player.Rod.data.durability == 0)
        {
            Player.Instance.UnequipRod();
            var rod = Player.Rod;
            Player.Inventory.Remove(rod);
            Player.Equipment.Remove(rod);
            Destroy(rod.gameObject);
            Notification.Instance.Display(GameData.ActiveLanguage.fishing_rod_broken, 3f, Notification.NotificationType.BadNews);
            ReturnToFreePlay();
        }
    }

    public void ContinueQuestions()
    {
        UI_Manager.Instance.OpenMenu(GameData.FishingMenu);
        Question.Instance.RegenerateRandomQuestion();
    }

    public void CorrectAnswer()
    {
        Countdown.Instance.StopCountdown();
        UI_Manager.Instance.OpenMenu(GameData.QuestionAnsweredScreen);
        GameData.QuestionAnsweredImage.color = Color.green;
        GameData.QuestionAnsweredText.text = GameData.ActiveLanguage.fishing_correct_answer.Replace("<br>", "\n");

        SoundDatabase.PlayWinSound();

        var randomFish = GameData.FishDatabase.items[Random.Range(0, GameData.FishDatabase.items.Count)];
        var sameFishInInventory = Player.Inventory.Where(x => x.data.ID == randomFish.data.ID);
        if (sameFishInInventory.Count() > 0)
        {
            sameFishInInventory.ElementAt(0).data.amount++;
        }
        else
        {
            var fish = Instantiate(randomFish, Player.Instance.inventoryParent);
            fish.Activate();
            Player.Inventory.Add(fish);
        }
        int amount = Random.Range(1, Player.Rod.data.fishMultiplier);
        Notification.Instance.Display(string.Format(GameData.ActiveLanguage.fishing_caught_fish,amount,randomFish.name), randomFish.icon, 2f, Notification.NotificationType.GoodNews);
    }

    public void WrongAnswer()
    {
        Countdown.Instance.StopCountdown();
        UI_Manager.Instance.OpenMenu(GameData.QuestionAnsweredScreen);
        GameData.QuestionAnsweredImage.color = Color.red;
        GameData.QuestionAnsweredText.text = GameData.ActiveLanguage.fishing_wrong_answer.Replace("<br>", "\n");

        ReduceRodDurability();
    }

    public void Timeout()
    {
        UI_Manager.Instance.OpenMenu(GameData.Timeout);
        ReduceRodDurability();
    }

    public void QuitQuiz()
    {
        Countdown.Instance.StopCountdown();
        ReduceRodDurability();
        ReturnToFreePlay();
    }

    public void QuitWithoutPenalty()
    {
        Countdown.Instance.StopCountdown();
        ReturnToFreePlay();
    }

    public void ReturnToFreePlay()
    {
        SwitchGameMode(GameData.Mode.Free);
        Player.Instance.StopMoving();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if(GameData.gameMode == GameData.Mode.Fishing && Player.Rod != null)
            GameData.RodsLeftText.text = "x" + Player.Rod.data.durability.ToString();
    }
}