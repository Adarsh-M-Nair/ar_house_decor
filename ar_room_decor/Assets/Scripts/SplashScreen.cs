using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public Image loaderFill;
    public float duration = 3f;

    void Start()
    {
        if (loaderFill == null)
        {
            Debug.LogError("LoaderFill is not assigned!");
            return;
        }

        loaderFill.fillAmount = 0f;
        StartCoroutine(LoadSplash());
    }

    IEnumerator LoadSplash()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loaderFill.fillAmount = elapsed / duration;
            yield return null;
        }

        // Make sure bar is completely full
        loaderFill.fillAmount = 1f;

        // Wait 0.5 seconds so user sees full bar
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("HomeScene");
    }
}