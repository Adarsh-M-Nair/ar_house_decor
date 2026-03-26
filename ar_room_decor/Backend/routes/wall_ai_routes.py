from flask import Blueprint, request, jsonify
import os
import cv2

from services.wall_analysis_service import analyze_wall
from services.color_recommendation_service import recommend_colors
from services.image_generation_service import generate_wall_images

wall_ai_bp = Blueprint("wall_ai", __name__)

UPLOAD_FOLDER = "uploads"
os.makedirs(UPLOAD_FOLDER, exist_ok=True)


from flask import Blueprint, request, jsonify
import os
import cv2

from services.wall_analysis_service import analyze_wall
from services.color_recommendation_service import recommend_colors
from services.image_generation_service import generate_wall_images

wall_ai_bp = Blueprint("wall_ai", __name__)

UPLOAD_FOLDER = "uploads"
os.makedirs(UPLOAD_FOLDER, exist_ok=True)


@wall_ai_bp.route("/analyze-wall", methods=["POST"])
def recommend_wall():

    data = request.get_json()

    if "wall_image" not in data:
        return jsonify({"error": "No image"}), 400

    import base64
    import numpy as np

    image_data = base64.b64decode(data["wall_image"])
    nparr = np.frombuffer(image_data, np.uint8)
    image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    mask, color, style = analyze_wall(image, None)

    colors = recommend_colors(style)

    images = generate_wall_images(image, mask, colors)

    return jsonify({
        "status": "success",
        "generated_images": images,
        "style": style,
        "dominant_color": str(color)
    })