using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InputSceneManager : MonoBehaviour
{
    public TMP_InputField budgetInput;
    public TMP_Dropdown preferenceDropdown;

    public void Generate()
    {
        // Save budget
        float budget = float.Parse(budgetInput.text);
        PlayerPrefs.SetFloat("Budget", budget);

        // Save preference
        string preference = preferenceDropdown.options[
            preferenceDropdown.value].text;

        PlayerPrefs.SetString("Preference", preference);

        PlayerPrefs.Save();

        // Load Result Scene
        SceneManager.LoadScene("ResultScene");
    }
}