using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    void Start()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    void OnDeepLinkActivated(string url)
    {
        Debug.Log("Deep Link Received: " + url);

        if (url.Contains("open-ar"))
        {
            SceneManager.LoadScene("ARScene");
        }
    }
}