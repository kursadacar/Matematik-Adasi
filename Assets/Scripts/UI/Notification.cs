using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Notification : Singleton<Notification>
{
    [SerializeField] Text text;
    [SerializeField] RectTransform targetPos;
    [SerializeField] RectTransform startingPos;
    [SerializeField] Image background;
    [SerializeField] Image icon;
    [SerializeField] Sprite defaultIcon;

    [SerializeField] Color goodNewsColor, badNewsColor, warningColor;
    public enum NotificationType
    {
        GoodNews,
        BadNews,
        Warning
    }

    public NotificationType type { get; private set; }

    private void Start()
    {
        var rect = (RectTransform)transform;
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(GoBack(0.3f));
    }

    private void SetWarningColor(NotificationType type)
    {
        this.type = type;

        switch (type)
        {
            case NotificationType.GoodNews:
                {
                    background.color = goodNewsColor;
                }
                break;
            case NotificationType.BadNews:
                {
                    background.color = badNewsColor;
                }
                break;
            case NotificationType.Warning:
                {
                    background.color = warningColor;
                }
                break;
        }
    }

    public void Display(string text,Sprite icon, float duration, NotificationType type)
    {
        StopAllCoroutines();

        SoundDatabase.PlayNotificationSound();

        this.icon.sprite = icon;
        this.text.text = text;

        SetWarningColor(type);

        StartCoroutine(DisplayOvertime(0.3f, duration));
    }

    public void Display(string text, float duration, NotificationType type)
    {
        StopAllCoroutines();

        SoundDatabase.PlayNotificationSound();

        SetWarningColor(type);

        icon.sprite = defaultIcon;
        this.text.text = text;
        StartCoroutine(DisplayOvertime(0.3f, duration));
    }

    private IEnumerator DisplayOvertime(float animTime, float duration)
    {
        float timer = 0f;
        while (true)
        {
            //Debug.Log("Notification - Display Over Time");
            timer += Time.unscaledDeltaTime;

            transform.position = Vector3.Lerp(startingPos.position, targetPos.position, timer / animTime);

            if (timer > duration)
            {
                StartCoroutine(GoBack(animTime));
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator GoBack(float animTime)
    {
        float timer = 0f;
        Vector3 curPos = transform.position;
        while (true)
        {
            //Debug.Log("Notification - GoBack");
            timer += Time.unscaledDeltaTime;

            transform.position = Vector3.Lerp(curPos, startingPos.position, timer / animTime);

            if(timer > animTime)
            {
                yield break;
            }
            yield return null;
        }
    }
}
