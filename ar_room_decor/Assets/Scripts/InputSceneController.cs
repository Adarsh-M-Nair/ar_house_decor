using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputSceneController : MonoBehaviour
{
    public TMP_InputField budgetInput;
    public RawImage wallImageDisplay;
    public Button analyzeButton;
    public TMP_Text statusText;

    private Texture2D capturedWallImage;
    private WallAnalysisAPI api;

    private void Start()
    {
        api = WallAnalysisAPI.Instance;
        analyzeButton.onClick.AddListener(OnAnalyzeClick);
    }

    public void SetWallImage(Texture2D image)
    {
        capturedWallImage = image;
        wallImageDisplay.texture = image;
        statusText.text = "Image ready";
    }

    private void OnAnalyzeClick()
    {
        if (capturedWallImage == null)
        {
            statusText.text = "Upload image first";
            return;
        }

        int budget = int.Parse(budgetInput.text);
        StartCoroutine(Analyze(budget));
    }

    IEnumerator Analyze(int budget)
    {
        statusText.text = "Analyzing...";

        float lat = 0;
        float lon = 0;

        yield return StartCoroutine(api.GetCurrentLocation((a, b) =>
        {
            lat = a;
            lon = b;
        }));

        yield return StartCoroutine(api.AnalyzeWall(
            capturedWallImage,
            budget,
            lat,
            lon,
            OnSuccess,
            OnError
        ));
    }

    void OnSuccess(WallAnalysisAPI.WallAnalysisResponse response)
{
    string json = JsonUtility.ToJson(response);
    
    Debug.Log("Saving JSON: " + json);

    PlayerPrefs.SetString("Result", json);
    PlayerPrefs.Save();   // IMPORTANT

    UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
}

    void OnError(string error)
{
    Debug.LogError("Backend Error: " + error);
    statusText.text = error;
}
}