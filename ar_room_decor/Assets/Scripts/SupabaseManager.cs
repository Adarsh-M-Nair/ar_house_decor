using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SupabaseManager : MonoBehaviour
{
    private static string shopsURL = 
        SupabaseConfig.ProjectURL + "/rest/v1/shops?select=*";
    
    private static string productsURL = 
        SupabaseConfig.ProjectURL + "/rest/v1/products?select=*";

    // Fetch all shops
    public IEnumerator GetShops(System.Action<List<Shop>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(shopsURL))
        {
            request.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
            request.SetRequestHeader("Authorization", "Bearer " + SupabaseConfig.AnonKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"shops\":" + request.downloadHandler.text + "}";
                ShopListResponse response = JsonUtility.FromJson<ShopListResponse>(json);
                callback(response.shops);
            }
            else
            {
                Debug.LogError("Failed to fetch shops: " + request.error);
                callback(null);
            }
        }
    }

    // Fetch products filtered by max price
    public IEnumerator GetProductsWithinBudget(float budget, System.Action<List<Product>> callback)
    {
        string url = SupabaseConfig.ProjectURL + 
                     "/rest/v1/products?select=*&price=lte." + budget;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
            request.SetRequestHeader("Authorization", "Bearer " + SupabaseConfig.AnonKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"products\":" + request.downloadHandler.text + "}";
                ProductListResponse response = JsonUtility.FromJson<ProductListResponse>(json);
                callback(response.products);
            }
            else
            {
                Debug.LogError("Failed to fetch products: " + request.error);
                callback(null);
            }
        }
    }
}