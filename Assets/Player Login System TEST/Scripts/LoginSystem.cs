using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    [SerializeField] InputField nameField;
    [SerializeField] InputField passwordField;

    public void RegisterPlayerData()
    {
        StartCoroutine(RegisterRoutine());
    }

    public void Login()
    {
        StartCoroutine(LoginRoutine());
    }

    private IEnumerator RegisterRoutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/WickedGames-PlayerData/register.php", form);

        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Couldn't connect");
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }

    }

    private IEnumerator LoginRoutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/WickedGames-PlayerData/login.php", form);

        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Couldn't connect");
        }
        else
        {
            foreach (var message in www.downloadHandler.text.Split('\t'))
            {
                Debug.Log(message);
            }
        }
    }
}
