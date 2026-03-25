using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BudgetManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField budgetInput;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI locationText;
    public GameObject resultPanel;
    public Transform resultContainer;
    public GameObject shopCardPrefab;
    public GameObject productRowPrefab;

    [Header("Settings")]
    public float searchRadiusKm = 50f;

    private double userLat = 0;
    private double userLon = 0;
    private bool locationFound = false;
    private SupabaseManager supabase;
    private List<Shop> allShops = new List<Shop>();

    void Start()
    {
        supabase = GetComponent<SupabaseManager>();
        resultPanel.SetActive(false);
        statusText.text = "📍 Getting your location...";
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            SetDefaultLocation();
            yield break;
        }

        Input.location.Start();
        int maxWait = 10;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            SetDefaultLocation();
        }
        else
        {
            userLat = Input.location.lastData.latitude;
            userLon = Input.location.lastData.longitude;
            locationFound = true;
            locationText.text = "📍 " + userLat.ToString("F4") + ", " + userLon.ToString("F4");
            statusText.text = "✅ Location found! Enter budget to search.";
        }

        Input.location.Stop();

        // Preload shops
        yield return StartCoroutine(supabase.GetShops((shops) =>
        {
            if (shops != null)
            {
                allShops = shops;
                Debug.Log("Loaded " + shops.Count + " shops from Supabase");
            }
        }));
    }

    void SetDefaultLocation()
    {
        userLat = 9.9312;
        userLon = 76.2673;
        locationFound = true;
        locationText.text = "📍 Kochi, Kerala (default)";
        statusText.text = "✅ Using default location. Enter budget to search.";
    }

    public void SearchRecommendations()
    {
        if (!locationFound)
        {
            statusText.text = "⏳ Still getting location...";
            return;
        }

        string budgetStr = budgetInput.text.Trim();
        if (string.IsNullOrEmpty(budgetStr))
        {
            statusText.text = "❌ Please enter your budget!";
            return;
        }

        if (!float.TryParse(budgetStr, out float budget) || budget <= 0)
        {
            statusText.text = "❌ Enter a valid budget amount!";
            return;
        }

        StartCoroutine(FetchAndDisplay(budget));
    }

    IEnumerator FetchAndDisplay(float budget)
    {
        statusText.text = "🔍 Searching...";

        foreach (Transform child in resultContainer)
            Destroy(child.gameObject);

        List<Product> products = null;
        yield return StartCoroutine(supabase.GetProductsWithinBudget(budget, (result) =>
        {
            products = result;
        }));

        if (products == null || products.Count == 0)
        {
            statusText.text = "😔 No products found within ₹" + budget.ToString("N0");
            resultPanel.SetActive(true);
            yield break;
        }

        Dictionary<int, List<Product>> shopProducts = new Dictionary<int, List<Product>>();

        foreach (var product in products)
        {
            if (!shopProducts.ContainsKey(product.shop_id))
                shopProducts[product.shop_id] = new List<Product>();

            shopProducts[product.shop_id].Add(product);
        }

        int totalResults = 0;

        foreach (var shop in allShops)
        {
            if (!shopProducts.ContainsKey(shop.id))
                continue;

            double distance = HaversineCalculator.CalculateDistance(
                userLat,
                userLon,
                shop.latitude,
                shop.longitude
            );

            if (distance <= searchRadiusKm)
            {
                GameObject card = Instantiate(shopCardPrefab, resultContainer);

                var cardTexts = card.GetComponentsInChildren<TextMeshProUGUI>();

                if (cardTexts.Length >= 2)
                {
                    cardTexts[0].text = shop.shop_name;
                    cardTexts[1].text = distance.ToString("F1") + " km • " + shop.address;
                }

                Transform productContainer = card.transform.Find("ProductContainer");

                if (productContainer != null)
                {
                    foreach (var product in shopProducts[shop.id])
                    {
                        GameObject row = Instantiate(productRowPrefab, productContainer);

                        var rowTexts = row.GetComponentsInChildren<TextMeshProUGUI>();

                        if (rowTexts.Length >= 2)
                        {
                            rowTexts[0].text = product.furniture;
                            rowTexts[1].text = "₹" + product.price.ToString("N0");
                        }

                        totalResults++;
                    }
                }
            }
        }

        resultPanel.SetActive(true);

        if (totalResults == 0)
            statusText.text = "😔 No nearby shops found within ₹" + budget.ToString("N0");
        else
            statusText.text = "✅ Found " + totalResults + " items within ₹" + budget.ToString("N0");
    }
}