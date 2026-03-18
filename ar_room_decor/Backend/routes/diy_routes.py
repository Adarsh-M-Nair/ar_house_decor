from flask import Blueprint, jsonify
from services.diy_services import get_diy_by_item

diy_bp = Blueprint("diy", __name__)


@diy_bp.route("/diy/<int:item_id>", methods=["GET"])
def get_diy(item_id):
    try:
        guide = get_diy_by_item(item_id)

        if not guide:
            return jsonify({"error": "DIY guide not found"}), 404

        return jsonify({
            "status": "success",
            "data": guide
        }), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500