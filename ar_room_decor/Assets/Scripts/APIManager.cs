using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    public ARSpawner spawner;

    string url = "http://YOUR-IP:5000/furniture";

    void Start()
    {
        StartCoroutine(GetFurniture());
    }

    IEnumerator GetFurniture()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // Example: parse manually for now
            string modelName = "sofa1"; // Replace with JSON parsing

            spawner.SpawnObject(modelName);
        }
        else
        {
            Debug.LogError("API Error: " + request.error);
        }
    }
}