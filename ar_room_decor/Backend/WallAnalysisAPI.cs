using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class WallAnalysisAPI : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:5000";

    [System.Serializable]
    public class WallAnalysisRequest
    {
        public int budget;
        public string wall_image; // base64 encoded
        public float user_lat;
        public float user_lon;
    }

    [System.Serializable]
    public class WallAnalysisResponse
    {
        public string status;
        public WallAnalysisData wall_analysis;
        public StoreData[] nearby_stores;
        public FurnitureData[] recommended_furniture;
    }

    [System.Serializable]
    public class WallAnalysisData
    {
        public ColorData dominant_color;
        public ColorVariation[] design_variations;
    }

    [System.Serializable]
    public class ColorData
    {
        public int[] rgb;
        public string hex;
        public string name;
    }

    [System.Serializable]
    public class ColorVariation
    {
        public string name;
        public int[] rgb;
        public string hex;
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

    // Singleton instance
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

    public IEnumerator AnalyzeWall(Texture2D wallImage, int budget,
                                 float userLat, float userLon,
                                 System.Action<WallAnalysisResponse> onSuccess,
                                 System.Action<string> onError)
    {
        // Convert texture to base64
        string base64Image = ConvertTextureToBase64(wallImage);

        WallAnalysisRequest requestData = new WallAnalysisRequest
        {
            budget = budget,
            wall_image = base64Image,
            user_lat = userLat,
            user_lon = userLon
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(BASE_URL + "/analyze-wall", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    WallAnalysisResponse response = JsonUtility.FromJson<WallAnalysisResponse>(responseText);

                    if (response.status == "success")
                    {
                        onSuccess?.Invoke(response);
                    }
                    else
                    {
                        onError?.Invoke("Server returned error: " + responseText);
                    }
                }
                catch (Exception e)
                {
                    onError?.Invoke("JSON parsing error: " + e.Message);
                }
            }
            else
            {
                onError?.Invoke("Network error: " + request.error);
            }
        }
    }

    private string ConvertTextureToBase64(Texture2D texture)
    {
        // Convert texture to PNG bytes
        byte[] pngBytes = texture.EncodeToPNG();

        // Convert to base64
        return Convert.ToBase64String(pngBytes);
    }

    // Helper method to get current location (you'll need to implement GPS)
    public IEnumerator GetCurrentLocation(System.Action<float, float> onLocationReceived)
    {
        // For now, return mock location (Kochi, India)
        // In production, use Unity's Input.location
        onLocationReceived?.Invoke(9.9816f, 76.2999f);
        yield return null;
    }
}