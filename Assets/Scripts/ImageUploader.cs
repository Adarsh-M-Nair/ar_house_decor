using UnityEngine;
using System.IO;

public class ImageUploader : MonoBehaviour
{
    private InputSceneController inputController;

    private void Start()
    {
        inputController = FindObjectOfType<InputSceneController>();
    }

    public void UploadImage()
    {
        Debug.Log("Upload button clicked");

#if UNITY_EDITOR

        string path = UnityEditor.EditorUtility.OpenFilePanel(
            "Select Wall Image",
            "",
            "png,jpg,jpeg"
        );

        if (!string.IsNullOrEmpty(path))
        {
            LoadImage(path);
        }

#else

        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                LoadImage(path);
            }

        }, "Select Wall Image", "image/*");

#endif
    }

    void LoadImage(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(imageData))
        {
            Debug.Log("Image loaded successfully");
            inputController.SetWallImage(texture);
        }
    }
}