from database.supabase_client import supabase


def get_diy_by_item(item_id):
    """
    Fetch structured DIY guide for selected furniture item.
    """

    response = supabase.table("diy_guide") \
        .select("steps, tools, safety, time_estimate") \
        .eq("item_id", item_id) \
        .execute()

    return response.data