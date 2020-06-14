using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : Singleton<TipPanel>
{
    [SerializeField] CanvasGroup menu;
    public Text text;
    public bool isActive { get; private set; }

    public void Show(string text)
    {
        transform.localScale = Vector3.one;
        this.text.text = text;
        StartCoroutine(FadeOverTime(0.2f, true));
        isActive = true;
    }

    public void Close()
    {
        StartCoroutine(FadeOverTime(0.2f, false));
        isActive = false;
    }

    IEnumerator FadeOverTime(float animTime, bool open)
    {
        float timer = 0f;

        float startingAlpha = menu.alpha;
        float targetAlpha = open ? 1f : 0f;

        menu.blocksRaycasts = open;
        menu.interactable = open;

        while (true)
        {
            //Debug.Log("TipPanel - FadeOverTime");
            timer += Time.deltaTime;

            menu.alpha = Mathf.Lerp(startingAlpha, targetAlpha, timer / animTime);

            if (timer > animTime)
            {
                yield break;
            }
            yield return null;
        }
    }
}
