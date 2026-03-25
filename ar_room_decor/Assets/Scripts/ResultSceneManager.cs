using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ResultSceneManager : MonoBehaviour
{
    public Transform generatedContainer;
    public GameObject imagePrefab;

    public TextMeshProUGUI preferenceText;
    public TextMeshProUGUI budgetText;

    void Start()
    {
        // Load preference
        preferenceText.text =
            "Style: " + PlayerPrefs.GetString("Preference");

        // Load budget
        budgetText.text =
            "Budget: ₹" + PlayerPrefs.GetFloat("Budget");

        // Load uploaded images
        foreach (Texture2D tex in ImageUploader.uploadedImages)
        {
            GameObject img =
                Instantiate(imagePrefab, generatedContainer);

            img.GetComponent<RawImage>().texture = tex;
        }
    }
}