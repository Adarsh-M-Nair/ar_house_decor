using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputSceneController : MonoBehaviour
{
    [Header("UI References")]
    public InputField budgetInput;
    public RawImage wallImageDisplay;
    public Button analyzeButton;
    public Text statusText;

    [Header("Settings")]
    public int defaultBudget = 1000;

    private Texture2D capturedWallImage;
    private WallAnalysisAPI api;

    private void Start()
    {
        api = WallAnalysisAPI.Instance;

        // Set default budget
        if (budgetInput != null)
        {
            budgetInput.text = defaultBudget.ToString();
        }

        // Setup button
        if (analyzeButton != null)
        {
            analyzeButton.onClick.AddListener(OnAnalyzeButtonClick);
        }

        UpdateStatus("Ready to capture wall image");
    }

    // Call this when user captures/selects wall image
    public void SetWallImage(Texture2D image)
    {
        capturedWallImage = image;

        if (wallImageDisplay != null)
        {
            wallImageDisplay.texture = image;
        }

        UpdateStatus("Wall image captured! Ready to analyze.");
    }

    private void OnAnalyzeButtonClick()
    {
        if (capturedWallImage == null)
        {
            UpdateStatus("Please capture a wall image first!");
            return;
        }

        int budget = defaultBudget;
        if (budgetInput != null && !string.IsNullOrEmpty(budgetInput.text))
        {
            int.TryParse(budgetInput.text, out budget);
        }

        StartCoroutine(AnalyzeWallImage(budget));
    }

    private IEnumerator AnalyzeWallImage(int budget)
    {
        UpdateStatus("Analyzing wall image...");
        analyzeButton.interactable = false;

        // Get current location
        float userLat = 0, userLon = 0;
        yield return StartCoroutine(api.GetCurrentLocation((lat, lon) => {
            userLat = lat;
            userLon = lon;
        }));

        // Send to backend
        yield return StartCoroutine(api.AnalyzeWall(
            capturedWallImage,
            budget,
            userLat,
            userLon,
            OnAnalysisSuccess,
            OnAnalysisError
        ));
    }

    private void OnAnalysisSuccess(WallAnalysisAPI.WallAnalysisResponse response)
    {
        UpdateStatus("Analysis complete! Loading results...");

        // Store results for ResultScene
        PlayerPrefs.SetString("WallAnalysisResult", JsonUtility.ToJson(response));
        PlayerPrefs.SetInt("UserBudget", int.Parse(budgetInput.text));
        PlayerPrefs.Save();

        // Load ResultScene
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }

    private void OnAnalysisError(string error)
    {
        UpdateStatus("Analysis failed: " + error);
        analyzeButton.interactable = true;
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log("InputScene: " + message);
    }

    // Call this from your image capture system
    public void OnWallImageCaptured(Texture2D image)
    {
        SetWallImage(image);
    }
}