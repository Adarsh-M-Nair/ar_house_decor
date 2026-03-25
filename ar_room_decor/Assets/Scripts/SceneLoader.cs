using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Splash → Home
    public void LoadHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    // Home → Input
    public void LoadInput()
    {
        SceneManager.LoadScene("InputScene");
    }

    // Input → Result
    public void LoadResult()
    {
        SceneManager.LoadScene("ResultScene");
    }

    // Result → AR
    public void LoadAR()
    {
        SceneManager.LoadScene("ARScene");
    }

    // Back to Home
    public void LoadHomeFromAny()
    {
        SceneManager.LoadScene("HomeScene");
    }

    // Quit App (Optional)
    public void QuitApp()
    {
        Application.Quit();
    }
}