using UnityEngine;
using TMPro;

public class ResultSceneController : MonoBehaviour
{
    public TMP_Text storeText;
    public TMP_Text furnitureText;

   void Start()
{
    Debug.Log("ResultScene Loaded");

    string json = PlayerPrefs.GetString("Result");

    Debug.Log("JSON: " + json);

    if (string.IsNullOrEmpty(json))
    {
        Debug.LogError("No data received");
        return;
    }

    WallAnalysisAPI.WallAnalysisResponse response =
        JsonUtility.FromJson<WallAnalysisAPI.WallAnalysisResponse>(json);

    DisplayStores(response);
    DisplayFurniture(response);
}

    void DisplayStores(WallAnalysisAPI.WallAnalysisResponse response)
    {
        storeText.text = "Nearby Stores:\n";

        foreach (var store in response.nearby_stores)
        {
            storeText.text += store.shop_name + "\n";
        }
    }

    void DisplayFurniture(WallAnalysisAPI.WallAnalysisResponse response)
    {
        furnitureText.text = "Furniture:\n";

        foreach (var item in response.recommended_furniture)
        {
            furnitureText.text += item.name + "\n";
        }
    }
}