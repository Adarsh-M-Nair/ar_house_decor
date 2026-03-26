using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[Serializable]
public class WallResponse
{
    public string status;
    public string[] generated_images;
}

public class RecoloredImageLoader : MonoBehaviour
{
    public RawImage displayImage;   // Assign in inspector

    void Start()
    {
        LoadImageFromPrefs();
    }

    public void LoadImageFromPrefs()
    {
        string json = PlayerPrefs.GetString("Result");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("No data found in PlayerPrefs");
            return;
        }

        WallResponse response = JsonUtility.FromJson<WallResponse>(json);

        if (response == null || response.generated_images.Length == 0)
        {
            Debug.LogError("Invalid response or no images");
            return;
        }

        // Load first image
        Texture2D tex = Base64ToTexture(response.generated_images[0]);

        displayImage.texture = tex;
    }

    Texture2D Base64ToTexture(string base64)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        return texture;
    }
}