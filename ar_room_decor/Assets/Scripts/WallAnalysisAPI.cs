using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class WallAnalysisAPI : MonoBehaviour
{
    private const string BASE_URL = "http://10.67.101.73:5000";

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

    [System.Serializable]
    public class WallAnalysisRequest
    {
        public int budget;
        public string wall_image;
        public float user_lat;
        public float user_lon;
    }

    [System.Serializable]
    public class WallAnalysisResponse
    {
        public string status;
        public StoreData[] nearby_stores;
        public FurnitureData[] recommended_furniture;
    }

    [System.Serializable]
public class WallAnalysisBatch
{
    public DominantColor dominant_color;
}

[System.Serializable]
public class DominantColor
{
    public int[] rgb;
    public string hex;
    public string name;
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

        using (UnityWebRequest request =
            new UnityWebRequest(BASE_URL + "/analyze-wall", "POST"))
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
                    Debug.Log("Backend Response: " + responseText);
                    WallAnalysisResponse response =
                        JsonUtility.FromJson<WallAnalysisResponse>(responseText);

                    onSuccess?.Invoke(response);
                }
                catch (Exception e)
                {
                    onError?.Invoke("JSON error: " + e.Message);
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
        byte[] pngBytes = texture.EncodeToPNG();
        return Convert.ToBase64String(pngBytes);
    }

    public IEnumerator GetCurrentLocation(Action<float, float> callback)
    {
        callback?.Invoke(9.9816f, 76.2999f);
        yield return null;
    }
}