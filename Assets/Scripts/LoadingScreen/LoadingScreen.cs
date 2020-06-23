using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingBarFiller;
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        var op = SceneManager.LoadSceneAsync(1);

        Vector3 startingScale = new Vector3(0f, 1f, 1f);

        while (op.progress < 1)
        {
            loadingBarFiller.transform.localScale = Vector3.Lerp(startingScale, Vector3.one, op.progress);

            yield return null;
        }
    }
}
