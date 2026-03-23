using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void LoadAR()
    {
        SceneManager.LoadScene("ARScene");
    }

    public void LoadBudget()
    {
        SceneManager.LoadScene("BudgetScene");
    }

    public void LoadSplash()
    {
        SceneManager.LoadScene("SplashScene");
    }
}