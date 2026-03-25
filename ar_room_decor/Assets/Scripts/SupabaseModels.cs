    using System.Collections.Generic;

[System.Serializable]
public class Shop
{
    public int id;
    public string shop_name;
    public string address;
    public double latitude;
    public double longitude;
}

[System.Serializable]
public class Product
{
    public int id;
    public int shop_id;
    public string furniture;
    public float price;
    public string category;
}

[System.Serializable]
public class ShopListResponse
{
    public List<Shop> shops;
}

[System.Serializable]
public class ProductListResponse
{
    public List<Product> products;
}