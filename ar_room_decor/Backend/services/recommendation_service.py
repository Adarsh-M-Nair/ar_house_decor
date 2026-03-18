from models.furniture_model import get_furniture_by_budget_and_color
from services.location_service import get_nearby_shop_ids
from models.shop_model import get_all_shops
from utils.haversine import calculate_distance


def recommend_items(budget, color, user_lat, user_lon):

    # Step 1: Get all matching color items within budget
    items = get_furniture_by_budget_and_color(budget, color)

    ranked_items = []

    for item in items:

        # Budget Score (closer to budget = better)
        budget_score = 1 - (item["price"] / budget)

        # Style Score (simple match for now)
        style_score = 1 if item["color"].lower() == color.lower() else 0.5

        # Distance Score
        distance_score = 0
        if user_lat and user_lon:
            shops = get_all_shops()
            for shop in shops:
                if shop["id"] == item["shop_id"]:
                    distance = calculate_distance(
                        user_lat,
                        user_lon,
                        shop["latitude"],
                        shop["longitude"]
                    )
                    distance_score = max(0, 1 - (distance / 10))

        final_score = (
            0.4 * budget_score +
            0.3 * style_score +
            0.3 * distance_score
        )

        item["score"] = round(final_score, 3)
        ranked_items.append(item)

    # Sort descending by score
    ranked_items.sort(key=lambda x: x["score"], reverse=True)

    return ranked_items