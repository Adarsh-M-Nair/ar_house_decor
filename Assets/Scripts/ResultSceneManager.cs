using UnityEngine;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    public TextMeshProUGUI preferenceText;
    public TextMeshProUGUI budgetText;

    void Start()
    {
        Debug.Log("ResultSceneManager started");

        // Load preference
        preferenceText.text =
            "Style: " + PlayerPrefs.GetString("Preference", "Modern");

        // Load budget
        budgetText.text =
            "Budget: ₹" + PlayerPrefs.GetInt("Budget", 0);

        // Load backend result
        string json = PlayerPrefs.GetString("Result");

        Debug.Log("Result JSON: " + json);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("No backend data received");
            return;
        }

        WallAnalysisAPI.WallAnalysisResponse response =
            JsonUtility.FromJson<WallAnalysisAPI.WallAnalysisResponse>(json);

        if (response == null)
        {
            Debug.LogError("Failed to parse backend response");
            return;
        }

        Debug.Log("Stores: " + response.nearby_stores.Length);
        Debug.Log("Furniture: " + response.recommended_furniture.Length);
    }
}