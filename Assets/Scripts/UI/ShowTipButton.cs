using UnityEngine;
using System.Collections;

public class ShowTipButton : MonoBehaviour
{
    private IEnumerator DisableAfterTimeRoutine;
    private bool available;

    public RectTransform tipCooldownBar;
    public float tipCooldown;

    private float cooldownLeft;

    private void Awake()
    {
        available = true;
        tipCooldownBar.localScale = new Vector3(1f, 0f, 1f);
    }

    private void OnEnable()
    {
        if(!available)
            StartCoroutine(Cooldown(cooldownLeft));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ToggleTip()
    {
        if (TipPanel.Instance.isActive)
            StopShowingTip();
        else
            ShowTip();
    }

    private IEnumerator Cooldown(float time)
    {
        float timer = 0f;
        available = false;

        cooldownLeft = time;
        while (true)
        {
            //Debug.Log("ShowTipButton - Cooldown"); -- Optimized!
            timer += Time.deltaTime;
            cooldownLeft -= Time.deltaTime;

            tipCooldownBar.localScale = new Vector3(1f, Mathf.Lerp(1f, 0f, timer / time), 1f);

            if(timer > time)
            {
                cooldownLeft = 0f;
                available = true;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DisableAfterTime(float time)
    {
        float timer = 0f;
        while (true)
        {
            //Debug.Log("ShowTipButton - DisableAfterTime"); -- Optimized!
            timer += Time.deltaTime;

            if(timer > time)
            {
                StopShowingTip();
                yield break;
            }
            yield return null;
        }
    }

    private void ShowTip()
    {
        if (!available)
            return;
        string text = "";
        Countdown.Instance.PauseCountdown();
        switch (Question.Instance.type)
        {
            case Question.Type.NaturalNumbers:
                {
                    switch (Question.Instance.operation)
                    {
                        case Question.Operation.Add:
                            {
                                text = GameData.ActiveLanguage.tip_natural_numbers_addition;
                            }
                            break;
                        case Question.Operation.Substract:
                            {
                                text = GameData.ActiveLanguage.tip_natural_numbers_substraction;
                            }
                            break;
                        case Question.Operation.Multiply:
                            {
                                text = GameData.ActiveLanguage.tip_natural_numbers_multiplication;
                            }
                            break;
                        case Question.Operation.Divide:
                            {
                                text = GameData.ActiveLanguage.tip_natural_numbers_division;
                            }
                            break;
                    }
                }
                break;
            case Question.Type.Fractions:
                {
                    switch (Question.Instance.operation)
                    {
                        case Question.Operation.Add:
                            {
                                text = GameData.ActiveLanguage.tip_fractions_addition;
                            }
                            break;
                        case Question.Operation.Substract:
                            {
                                text = GameData.ActiveLanguage.tip_fractions_substraction;
                            }
                            break;
                        case Question.Operation.Multiply:
                            {
                                text = GameData.ActiveLanguage.tip_fractions_multiplication;
                            }
                            break;
                        case Question.Operation.Divide:
                            {
                                text = GameData.ActiveLanguage.tip_fractions_division;
                            }
                            break;
                    }
                }
                break;
        }

        TipPanel.Instance.Show(text);
        DisableAfterTimeRoutine = DisableAfterTime(4f);
        StartCoroutine(DisableAfterTimeRoutine);
    }

    private void StopShowingTip()
    {
        StopCoroutine(DisableAfterTimeRoutine);
        StartCoroutine(Cooldown(tipCooldown));

        Countdown.Instance.ResumeCountdown();
        TipPanel.Instance.Close();
    }
}
