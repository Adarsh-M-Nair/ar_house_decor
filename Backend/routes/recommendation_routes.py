from flask import Blueprint, request, jsonify
from services.recommendation_service import recommend_items

recommendation_bp = Blueprint("recommendation", __name__)

@recommendation_bp.route("/recommend", methods=["POST"])
def recommend():
    try:
        data = request.get_json()

        if not data:
            return jsonify({"error": "No data provided"}), 400

        if "budget" not in data or "wall_color" not in data:
            return jsonify({"error": "Missing budget or wall_color"}), 400

        budget = data["budget"]
        wall_color = data["wall_color"]
        latitude = data.get("latitude")
        longitude = data.get("longitude")

        items = recommend_items(budget, wall_color, latitude, longitude)

        return jsonify({
            "status": "success",
            "count": len(items),
            "data": items
        }), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500