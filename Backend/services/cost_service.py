from models.furniture_model import get_furniture_by_ids


def calculate_total_cost(item_ids, user_budget):
    """
    Calculates:
    - Item total
    - Installation cost (10%)
    - Grand total
    - Remaining budget
    - Budget alert
    """

    # Fetch selected items
    items = get_furniture_by_ids(item_ids)

    if not items:
        return {
            "error": "No valid items found"
        }

    # 1️⃣ Calculate item total
    item_total = sum(item["price"] for item in items)

    # 2️⃣ Installation cost (10% of item total)
    installation_cost = round(item_total * 0.10, 2)

    # 3️⃣ Final total cost
    total_cost = round(item_total + installation_cost, 2)

    # 4️⃣ Remaining budget
    remaining_budget = round(user_budget - total_cost, 2)

    # 5️⃣ Budget alert system
    if remaining_budget < 0:
        alert = "Budget Exceeded"
    elif remaining_budget < user_budget * 0.1:
        alert = "Warning: Low Remaining Budget"
    else:
        alert = "Within Budget"

    return {
        "selected_items_count": len(items),
        "item_total": item_total,
        "installation_cost": installation_cost,
        "total_cost": total_cost,
        "user_budget": user_budget,
        "remaining_budget": remaining_budget,
        "alert": alert
    }