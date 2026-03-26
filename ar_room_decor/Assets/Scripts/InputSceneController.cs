using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputSceneController : MonoBehaviour
{
    public TMP_InputField budgetInput;
    public Button analyzeButton;
    public TMP_Text statusText;

    private Texture2D capturedWallImage;
    private WallAnalysisAPI api;

    private void Start()
    {
        Debug.Log("InputSceneController started");

        api = WallAnalysisAPI.Instance;

        analyzeButton.onClick.RemoveAllListeners();
        analyzeButton.onClick.AddListener(OnAnalyzeClick);
    }

    public void SetWallImage(Texture2D image)
    {
        Debug.Log("Wall image set");

        capturedWallImage = image;

        if (statusText != null)
        {
            statusText.text = "Image selected";
        }
    }

    private void OnAnalyzeClick()
    {
        Debug.Log("Analyze button clicked");

        if (capturedWallImage == null)
        {
            Debug.LogWarning("No image selected");
            statusText.text = "Upload image first";
            return;
        }

        int budget = 0;

        if (!string.IsNullOrEmpty(budgetInput.text))
        {
            int.TryParse(budgetInput.text, out budget);
        }

        StartCoroutine(Analyze(budget));
    }

    IEnumerator Analyze(int budget)
    {
        Debug.Log("Starting API call");

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
        Debug.Log("API Success");

        string json = JsonUtility.ToJson(response);

        Debug.Log("Saving JSON: " + json);

        PlayerPrefs.SetString("Result", json);
        PlayerPrefs.Save();

        Debug.Log("Loading ResultScene");

        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }

    void OnError(string error)
    {
        Debug.LogError("API Error: " + error);

        if (statusText != null)
        {
            statusText.text = "Error: " + error;
        }
    }
}