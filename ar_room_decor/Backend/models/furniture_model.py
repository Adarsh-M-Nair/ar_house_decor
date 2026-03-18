from database.supabase_client import supabase


def get_furniture_by_budget_and_color(budget, color):
    """
    Returns furniture items filtered by budget and wall color.
    """
    response = supabase.table("furniture") \
        .select("*") \
        .lte("price", budget) \
        .eq("color", color) \
        .execute()

    return response.data


def get_furniture_by_ids(item_ids):
    """
    Returns furniture items for given list of IDs.
    Used for cost calculation.
    """
    response = supabase.table("furniture") \
        .select("*") \
        .in_("id", item_ids) \
        .execute()

    return response.data


def get_furniture_by_shop_ids(shop_ids, budget, color):
    """
    Returns furniture items filtered by shop IDs + budget + color.
    Used for location-aware recommendation.
    """
    response = supabase.table("furniture") \
        .select("*") \
        .in_("shop_id", shop_ids) \
        .lte("price", budget) \
        .eq("color", color) \
        .execute()

    return response.data