using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Question : Singleton<Question>
{
    [SerializeField] Text elementA, elementB, elementC, elementD;
    [SerializeField] Text _operator;
    [SerializeField] Image AB_Seperator, CD_Seperator;
    [SerializeField] Text questionText;

    public List<Answer> options = new List<Answer>();


    public enum Type
    {
        NaturalNumbers,
        Fractions,
        //Money
    }

    public Type type { get; private set; }
    public Operation operation { get; private set; }

    public enum Operation
    {
        Add,
        Substract,
        Multiply,
        Divide
    }

    /// <summary>
    /// returns all divisors for all numbers bigger than 1. if number is 1 or less, returns empty array
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static int[] FindDivisors(int number)
    {
        List<int> divisors = new List<int>();
        if (number <= 1)
            return divisors.ToArray();

        for (int i = 2; i <= number; i++)
        {
            if (number % i == 0)
            {
                divisors.Add(i);
            }
        }
        return divisors.ToArray();
    }

    private static int FindCommonDivisor(int number1, int number2)
    {
        var array1 = FindDivisors(number1);
        var array2 = FindDivisors(number2);

        if(array1.Length == 0 || array2.Length == 0)
        {
            return -1;
        }

        foreach(var num1 in array1)
        {
            foreach(var num2 in array2)
            {
                if(num1 == num2)
                {
                    return num1;
                }
            }
        }

        return -1;
    }

    private static void SimplifyFraction(ref int number1, ref int number2)
    {
        while (FindCommonDivisor(number1, number2) != -1)
        {
            var divisor = FindCommonDivisor(number1, number2);
            number1 /= divisor;
            number2 /= divisor;
        }
    }

    public void RegenerateQuestion(Type type, Operation operation)
    {
        this.type = type;
        this.operation = operation;

        if (type == Type.Fractions)
        {
            if(operation == Operation.Substract)
            {
                Countdown.Instance.StartCountdown(GameData.QuestionTime * 3f);
            }
            else
            {
                Countdown.Instance.StartCountdown(GameData.QuestionTime * 2f);
            }
        }
        else
        {
            Countdown.Instance.StartCountdown(GameData.QuestionTime);
        }

        var correctAnswerIndex = Random.Range(0, options.Count);
        var correctAnswer = options[correctAnswerIndex];

        foreach(var answer in options)
        {
            answer.isCorrectAnswer = false;
        }
        correctAnswer.isCorrectAnswer = true;

        questionText.text = GameData.ActiveLanguage.question_ask_result_of_equation;

        int a = 0, b = 0, c = 1, d = 1;

        AB_Seperator.enabled = true;
        elementB.enabled = true;
        CD_Seperator.enabled = true;
        elementD.enabled = true;

        switch (operation)
        {
            case Operation.Add:
                {
                    _operator.text = "+";
                }
                break;
            case Operation.Substract:
                {
                    _operator.text = "-";
                }
                break;
            case Operation.Multiply:
                {
                    _operator.text = "x";
                }
                break;
            case Operation.Divide:
                {
                    _operator.text = "÷";
                }
                break;
        }

        switch (type)
        {
            case Type.NaturalNumbers:
                {
                    foreach(var answer in options)
                    {
                        answer.Denominator = 1;//all natural numbers have 1 as denominator
                    }

                    b = 1;
                    c = 1;

                    switch (operation)
                    {
                        case Operation.Add:
                            {
                                a = Random.Range(1, 100);
                                c = Random.Range(1, 100);
                                correctAnswer.Numerator = a + c;
                            }
                            break;
                        case Operation.Substract:
                            {
                                a = Random.Range(1, 100);
                                c = Random.Range(1, 100);

                                if(c > a)//Value1 should be bigger
                                {
                                    var tmp = c;
                                    c = a;
                                    a = tmp;
                                }

                                correctAnswer.Numerator = a - c;
                            }
                            break;
                        case Operation.Multiply:
                            {
                                a = Random.Range(1, 10);
                                c = Random.Range(1, 10);
                                correctAnswer.Numerator = a * c;
                            }
                            break;
                        case Operation.Divide:
                            {
                                c = Random.Range(1, 10);

                                a = Random.Range(1, 10) * c;
                                correctAnswer.Numerator = a / c;
                            }
                            break;
                    }
                }
                break;
            case Type.Fractions:
                {
                    switch (operation)
                    {
                        case Operation.Add:
                            {
                                a = Random.Range(1, 10);
                                b = Random.Range(1, 10);

                                c = Random.Range(1, 10);
                                d = Random.Range(1, 10);

                                var answer_n = a * d + b * c;
                                var answer_d = b * d;

                                correctAnswer.Numerator = answer_n;
                                correctAnswer.Denominator = answer_d;
                            }
                            break;
                        case Operation.Substract:
                            {
                                a = Random.Range(1, 10);
                                b = Random.Range(1, 10);

                                c = Random.Range(1, 10);
                                d = Random.Range(1, 10);

                                if(c/d > a / b)
                                {
                                    var tmp1 = c;
                                    var tmp2 = d;

                                    c = a;
                                    d = b;

                                    a = tmp1;
                                    b = tmp2;
                                }

                                var answer_n = a * d - b * c;
                                var answer_d = b * d;

                                correctAnswer.Numerator = answer_n;
                                correctAnswer.Denominator = answer_d;
                            }
                            break;
                        case Operation.Multiply:
                            {
                                a = Random.Range(1, 10);
                                b = Random.Range(1, 10);

                                c = Random.Range(1, 10);
                                d = Random.Range(1, 10);

                                var answer_n = a * c;
                                var answer_d = b * d;

                                correctAnswer.Numerator = answer_n;
                                correctAnswer.Denominator = answer_d;
                            }
                            break;
                        case Operation.Divide:
                            {
                                a = Random.Range(1, 10);
                                b = Random.Range(1, 10);

                                c = Random.Range(1, 10);
                                d = Random.Range(1, 10);

                                var answer_n = a * d;
                                var answer_d = b * c;

                                correctAnswer.Numerator = answer_n;
                                correctAnswer.Denominator = answer_d;
                            }
                            break;
                    }
                }
                break;
                //case Type.Money:
                //    break;
        }

        SimplifyFraction(ref a, ref b);
        SimplifyFraction(ref c, ref d);

        if (b == 1 || a == 0)
        {
            AB_Seperator.enabled = false;
            elementB.enabled = false;
        }
        if (d == 1 || c == 0)
        {
            CD_Seperator.enabled = false;
            elementD.enabled = false;
        }

        elementA.text = a.ToString();
        elementB.text = b.ToString();
        elementC.text = c.ToString();
        elementD.text = d.ToString();

        //Debug.Log(a + " , " + b + " , " + c + " , " + d + " , ");

        //Can not use simplify fraction, doing it manually
        while (FindCommonDivisor(correctAnswer.Numerator, correctAnswer.Denominator) != -1)
        {
            var divisor = FindCommonDivisor(correctAnswer.Numerator, correctAnswer.Denominator);
            correctAnswer.Numerator /= divisor;
            correctAnswer.Denominator /= divisor;
        }

        foreach (var answer in options)
        {
            if (answer != correctAnswer)
            {
                if(Random.Range(0f,1f) > 0.5f)
                {
                    answer.Numerator = correctAnswer.Numerator + Random.Range(1, 6);
                }
                else
                {
                    answer.Numerator = correctAnswer.Numerator - Random.Range(1, 6);
                }
                answer.Denominator = correctAnswer.Denominator;
            }
        }
    }

    public void RegenerateRandomQuestion()
    {
        var types = Enum.GetValues(typeof(Type));
        var ops = Enum.GetValues(typeof(Operation));

        RegenerateQuestion((Type)types.GetValue(Random.Range(0, types.Length)), (Operation)ops.GetValue(Random.Range(0, ops.Length)));
    }
}
