from database.supabase_client import supabase


def get_all_shops():
    """
    Returns all shops from database.
    """
    response = supabase.table("shops") \
        .select("*") \
        .execute()

    return response.data


def get_shop_by_id(shop_id):
    """
    Returns a single shop by ID.
    """
    response = supabase.table("shops") \
        .select("*") \
        .eq("id", shop_id) \
        .execute()

    return response.data