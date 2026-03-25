using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ImageUploader : MonoBehaviour
{
    public Transform previewContainer;
    public GameObject previewPrefab;

    public static List<Texture2D> uploadedImages =
        new List<Texture2D>();

    public void UploadImage()
    {
        Debug.Log("Upload button clicked");

#if UNITY_EDITOR

        // PC Upload (Unity Editor)
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

        // Android Gallery
        NativeGallery.GetImagesFromGallery((paths) =>
        {
            if (paths == null)
            {
                Debug.Log("No images selected");
                return;
            }

            foreach (string path in paths)
            {
                LoadImage(path);
            }

        }, "Select Wall Images", "image/*");

#endif
    }

    void LoadImage(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        if (texture != null)
        {
            uploadedImages.Add(texture);

            GameObject preview =
                Instantiate(previewPrefab, previewContainer);

            preview.GetComponent<RawImage>().texture =
                texture;
        }
        else
        {
            Debug.Log("Failed to load image");
        }
    }
}