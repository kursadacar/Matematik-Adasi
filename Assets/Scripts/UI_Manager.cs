using System.Collections;
using UnityEngine;

public class UI_Manager : Singleton<UI_Manager>
{
    private void Start()
    {
        foreach (var menu in GameData.AllMenus)
        {
            StartCoroutine(ToggleMenuOvertime(menu, false, 0f));
        }
        OpenMenu(GameData.MainMenu);
    }

    private CanvasGroup activeMenu;

    public void OpenMenu(CanvasGroup menuToOpen)
    {
        if (activeMenu == menuToOpen)
            return;

        StartCoroutine(ToggleMenuOvertime(menuToOpen, true));

        if(activeMenu != null)
            StartCoroutine(ToggleMenuOvertime(activeMenu, false));

        activeMenu = menuToOpen;
    }

    private IEnumerator ToggleMenuOvertime(CanvasGroup menu, bool open)
    {
        float timer = 0f;
        float animTime = GameData.MenuAnimTime;

        Vector3 startingScale = open ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1f, 1f, 1f);
        float startingAlpha = open ? 0f : 1f;

        Vector3 targetScale = open ? new Vector3(1f, 1f, 1f) : new Vector3(1.2f, 1.2f, 1.2f);
        float targetAlpha = open ? 1f : 0f;

        menu.blocksRaycasts = open;
        menu.interactable = open;

        menu.gameObject.SetActive(open);

        while (true)
        {
            //Debug.Log("UI_Manager - ToggleMenuOvertime"); --Optimized!
            timer += Time.unscaledDeltaTime;

            menu.alpha = Mathf.Lerp(startingAlpha, targetAlpha, timer / animTime);
            menu.transform.localScale = Vector3.Lerp(startingScale, targetScale, timer / animTime);

            if (timer > animTime)
            {
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator ToggleMenuOvertime(CanvasGroup menu, bool open,float animTime)
    {
        float timer = 0f;

        Vector3 startingScale = open ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1f, 1f, 1f);
        float startingAlpha = open ? 0f : 1f;

        Vector3 targetScale = open ? new Vector3(1f, 1f, 1f) : new Vector3(1.2f, 1.2f, 1.2f);
        float targetAlpha = open ? 1f : 0f;

        menu.blocksRaycasts = open;
        menu.interactable = open;

        while (true)
        {
            //Debug.Log("UI_Manager - Toggle Menu Overtime"); -- Optimized!
            timer += Time.unscaledDeltaTime;

            menu.alpha = Mathf.Lerp(startingAlpha, targetAlpha, timer / animTime);
            menu.transform.localScale = Vector3.Lerp(startingScale, targetScale, timer / animTime);

            if (timer > animTime)
            {
                yield break;
            }

            yield return null;
        }
    }
}
