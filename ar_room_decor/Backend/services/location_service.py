from database.supabase_client import supabase
from utils.haversine import calculate_distance

def get_nearby_shop_ids(user_lat, user_lon):
    response = supabase.table("shops").select("*").execute()
    shops = response.data

    nearby = []

    for shop in shops:
        distance = calculate_distance(
            user_lat,
            user_lon,
            shop["latitude"],
            shop["longitude"]
        )

        if distance <= 5:
            nearby.append(shop["id"])

    return nearby