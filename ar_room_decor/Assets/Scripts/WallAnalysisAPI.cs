using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class WallAnalysisAPI : MonoBehaviour
{
    // 🔥 CHANGE THIS TO YOUR LOCAL IP
    private const string BASE_URL = "http://176.20.0.41:5000";

    public static WallAnalysisAPI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------------
    // REQUEST MODEL
    // -----------------------------
    [System.Serializable]
    public class WallAnalysisRequest
    {
        public int budget;
        public string wall_image;
        public float user_lat;
        public float user_lon;
    }

    // -----------------------------
    // RESPONSE MODEL
    // -----------------------------
    [System.Serializable]
    public class WallAnalysisResponse
    {
        public string status;

        public string[] generated_images;

        public StoreData[] nearby_stores;
        public FurnitureData[] recommended_furniture;
    }

    [System.Serializable]
    public class StoreData
    {
        public int id;
        public string shop_name;
        public float latitude;
        public float longitude;
        public string address;
        public float distance;
    }

    [System.Serializable]
    public class FurnitureData
    {
        public int id;
        public string name;
        public string category;
        public string color;
        public string style;
        public string model_name;
        public string image_url;
    }

    // -----------------------------
    // MAIN API CALL
    // -----------------------------
    public IEnumerator AnalyzeWall(
        Texture2D wallImage,
        int budget,
        float userLat,
        float userLon,
        Action<WallAnalysisResponse> onSuccess,
        Action<string> onError)
    {
        Debug.Log("Preparing request...");

        string base64Image = ConvertTextureToBase64(wallImage);

        WallAnalysisRequest requestData = new WallAnalysisRequest
        {
            budget = budget,
            wall_image = base64Image,
            user_lat = userLat,
            user_lon = userLon
        };

        string jsonData = JsonUtility.ToJson(requestData);

        Debug.Log("Sending request to backend...");

        using (UnityWebRequest request =
            new UnityWebRequest(BASE_URL + "/analyze-wall", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("Request finished");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                Debug.Log("RAW RESPONSE:\n" + responseText);

                if (string.IsNullOrEmpty(responseText))
                {
                    onError?.Invoke("Empty response from server");
                    yield break;
                }

                WallAnalysisResponse response = null;

                try
                {
                    response = JsonUtility.FromJson<WallAnalysisResponse>(responseText);
                }
                catch (Exception e)
                {
                    onError?.Invoke("JSON parse error: " + e.Message);
                    yield break;
                }

                if (response == null)
                {
                    onError?.Invoke("Parsed response is NULL");
                    yield break;
                }

                Debug.Log("Parsed status: " + response.status);

                if (response.generated_images != null)
                {
                    Debug.Log("Images count: " + response.generated_images.Length);
                }
                else
                {
                    Debug.LogWarning("generated_images is NULL");
                }

                if (response.status == "success")
                {
                    Debug.Log("API SUCCESS → invoking callback");
                    onSuccess?.Invoke(response);
                }
                else
                {
                    onError?.Invoke("Backend returned status: " + response.status);
                }
            }
            else
            {
                Debug.LogError("Network Error: " + request.error);
                onError?.Invoke("Network error: " + request.error);
            }
        }
    }

    // -----------------------------
    // LOCATION (TEMP MOCK)
    // -----------------------------
    public IEnumerator GetCurrentLocation(Action<float, float> callback)
    {
        // 🔥 Replace later with real GPS
        float lat = 9.9816f;
        float lon = 76.2999f;

        callback?.Invoke(lat, lon);
        yield return null;
    }

    // -----------------------------
    // IMAGE ENCODING
    // -----------------------------
    private string ConvertTextureToBase64(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        return Convert.ToBase64String(bytes);
    }
}