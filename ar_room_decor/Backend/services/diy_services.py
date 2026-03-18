from models.diy_model import get_diy_by_item as fetch_diy


def get_diy_by_item(item_id):
    """
    Service layer for DIY guidance.
    Can include formatting or validation logic.
    """

    data = fetch_diy(item_id)

    if not data:
        return None

    guide = data[0]

    # Optional: Format response cleanly
    return {
        "steps": guide["steps"],
        "tools_required": guide["tools"],
        "safety_instructions": guide["safety"],
        "estimated_time": guide["time_estimate"]
    }