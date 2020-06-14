using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    [SerializeField] Text NumeratorText;
    [SerializeField] Image Line;
    [SerializeField] Text DenominatorText;

    public int Numerator
    {
        get => int.Parse(NumeratorText.text);
        set
        {
            NumeratorText.text = value.ToString();
            RecheckLayout();
        }
    }

    public int Denominator
    {
        get => int.Parse(DenominatorText.text);
        set
        {
            if (value == 0)
                value = 1;
            if(value == -1)
            {
                value = 1;
                Numerator *= -1;
            }
            DenominatorText.text = value.ToString();
            RecheckLayout();
        }
    }

    public bool isCorrectAnswer;

    private void RecheckLayout()
    {
        if(Numerator == 0 || Denominator == 1)
        {
            Line.enabled = false;
            DenominatorText.enabled = false;
        }
        else
        {
            Line.enabled = true;
            DenominatorText.enabled = true;
        }
    }

    public void SelectAnswer()
    {
        if (isCorrectAnswer)
            GameManager.Instance.CorrectAnswer();
        else
            GameManager.Instance.WrongAnswer();
    }

}
