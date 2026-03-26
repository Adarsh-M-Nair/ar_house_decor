using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class WallAnalysisAPI : MonoBehaviour
{
    private const string BASE_URL = "http://192.168.1.80:5000";

    // -----------------------------
    // Store Data
    // -----------------------------
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

    // -----------------------------
    // Furniture Data
    // -----------------------------
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
    // Wall Color Data
    // -----------------------------
    [System.Serializable]
    public class DominantColor
    {
        public int[] rgb;
        public string hex;
        public string name;
    }

    [System.Serializable]
    public class WallAnalysisBatch
    {
        public DominantColor dominant_color;
    }

    // -----------------------------
    // Request Model
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
    // Response Model
    // -----------------------------
    [System.Serializable]
    public class WallAnalysisResponse
    {
        public string status;
        public WallAnalysisBatch[] wall_analysis_batch;
        public StoreData[] nearby_stores;
        public FurnitureData[] recommended_furniture;
    }

    public static WallAnalysisAPI Instance { get; private set; }

    private void Awake()
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
    // Main API Call
    // -----------------------------
    public IEnumerator AnalyzeWall(
        Texture2D wallImage,
        int budget,
        float userLat,
        float userLon,
        Action<WallAnalysisResponse> onSuccess,
        Action<string> onError)
    {
        string base64Image = ConvertTextureToBase64(wallImage);

        WallAnalysisRequest requestData = new WallAnalysisRequest
        {
            budget = budget,
            wall_image = base64Image,
            user_lat = userLat,
            user_lon = userLon
        };

        string jsonData = JsonUtility.ToJson(requestData);

        Debug.Log("Sending request to: " + BASE_URL + "/analyze-wall");

        using (UnityWebRequest request =
            new UnityWebRequest(BASE_URL + "/analyze-wall", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            // WAIT for request
            yield return request.SendWebRequest();

            Debug.Log("Request finished");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network Error: " + request.error);
                onError?.Invoke(request.error);
                yield break;
            }

            try
            {
                string responseText = request.downloadHandler.text;

                Debug.Log("Backend Response: " + responseText);

                WallAnalysisResponse response =
                    JsonUtility.FromJson<WallAnalysisResponse>(responseText);

                if (response != null && response.status == "success")
                {
                    Debug.Log("API Success");

                    onSuccess?.Invoke(response);
                }
                else
                {
                    Debug.LogError("Invalid response");
                    onError?.Invoke("Invalid response from server");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON Parse Error: " + e.Message);
                onError?.Invoke(e.Message);
            }
        }
    }

    // -----------------------------
    // Convert Texture
    // -----------------------------
    private string ConvertTextureToBase64(Texture2D texture)
    {
        byte[] pngBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(pngBytes);
    }

    // -----------------------------
    // Location (Mock)
    // -----------------------------
    public IEnumerator GetCurrentLocation(Action<float, float> callback)
    {
        callback?.Invoke(9.9816f, 76.2999f);
        yield return null;
    }
}