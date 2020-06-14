using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : Singleton<Countdown>
{
    [SerializeField] Image filler;
    [SerializeField] Text indicatorText;

    private float maxValue;
    private bool updateCountdown;

    public float value
    {
        get
        {
            return maxValue * filler.transform.localScale.x;
        }
        set
        {
            if (value < 0f)
            {
                GameManager.Instance.Timeout();
                return;
            }
            if (value > maxValue)
                value = maxValue;
            filler.transform.localScale = new Vector3(value / maxValue, 1f, 1f);
                string text = value.ToString();
                if(text.Length> 0)
                {
                    var array = text.Split('.');
                    if(array.Length > 1)
                        text = array[0] + "." + array[1].Substring(0,1); 
                }
                indicatorText.text = text;
        }
    }

    public void StartCountdown(float time)
    {
        StopAllCoroutines();
        maxValue = time;
        StartCoroutine(Count(time));
        updateCountdown = true;
    }

    public void StopCountdown()
    {
        StopAllCoroutines();
        maxValue = 10f;
        value = maxValue;
        updateCountdown = false;
    }

    public void PauseCountdown()
    {
        updateCountdown = false;
    }

    public void ResumeCountdown()
    {
        updateCountdown = true;
    }

    IEnumerator Count(float questionTime)
    {
        float timer = 0f;

        while (true)
        {
            //Debug.Log("Count - Countdown"); -- Optimized!
            if (updateCountdown)
            {
                timer += Time.deltaTime;

                value = Mathf.Lerp(questionTime, 0f, timer / questionTime);

                if (timer > questionTime)
                {
                    StopCountdown();
                    GameManager.Instance.Timeout();
                    yield break;
                }
            }

            yield return null;
        }
    }
}
