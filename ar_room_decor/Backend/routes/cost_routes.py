from flask import Blueprint, request, jsonify
from services.cost_service import calculate_total_cost

cost_bp = Blueprint("cost", __name__)


@cost_bp.route("/calculate_total", methods=["POST"])
def calculate_total():
    try:
        data = request.get_json()

        if not data or "items" not in data or "budget" not in data:
            return jsonify({"error": "Items list and budget required"}), 400

        item_ids = data["items"]
        user_budget = data["budget"]

        result = calculate_total_cost(item_ids, user_budget)

        return jsonify({
            "status": "success",
            "data": result
        }), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500