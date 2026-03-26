from flask import Blueprint, request, jsonify, send_file
import base64
import io
import os
from PIL import Image
import numpy as np
import cv2
from models.furniture_model import get_furniture_by_budget_and_color
from models.shop_model import get_all_shops
from utils.haversine import calculate_distance

wall_bp = Blueprint("wall", __name__)

def rgb_to_hex(rgb):
    """Convert RGB list to hex color"""
    return "#{:02x}{:02x}{:02x}".format(int(rgb[0]), int(rgb[1]), int(rgb[2]))


def preprocess_image(image, max_size=1024):
    """Resize and convert image to RGB"""
    if image.mode != 'RGB':
        image = image.convert('RGB')

    width, height = image.size
    scale = min(max_size / width, max_size / height, 1.0)
    if scale < 1.0:
        image = image.resize((int(width * scale), int(height * scale)), Image.ANTIALIAS)

    return image


def segformer_wall_mask(image):
    """Simulated SegFormer output for wall mask (placeholder)."""
    cv_img = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)
    hsv = cv2.cvtColor(cv_img, cv2.COLOR_BGR2HSV)

    # Wall-like color detection: low saturation, medium brightness
    lower = np.array([0, 0, 60])
    upper = np.array([179, 80, 255])
    mask = cv2.inRange(hsv, lower, upper)

    # If mask is too small, fallback to lightness-based detection
    if cv2.countNonZero(mask) < mask.size * 0.05:
        gray = cv2.cvtColor(cv_img, cv2.COLOR_BGR2GRAY)
        _, mask = cv2.threshold(gray, 110, 255, cv2.THRESH_BINARY)

    return mask


def refine_wall_mask(mask, original_image):
    """Refine raw wall mask with morphological ops, edge filtering, and floor removal."""
    kernel = np.ones((11, 11), np.uint8)
    mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)
    mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)

    # Edge-aware filtering using Canny
    gray = cv2.cvtColor(np.array(original_image), cv2.COLOR_RGB2GRAY)
    edges = cv2.Canny(gray, 50, 150)
    edges_dilated = cv2.dilate(edges, np.ones((5, 5), np.uint8), iterations=1)
    mask[edges_dilated > 0] = 0

    # Floor removal: remove bottom 15%
    h = mask.shape[0]
    floor_zone = int(h * 0.15)
    mask[-floor_zone:, :] = 0

    # Smoothing
    mask = cv2.GaussianBlur(mask, (15, 15), 0)
    _, mask = cv2.threshold(mask, 127, 255, cv2.THRESH_BINARY)

    return mask


def get_dominant_wall_color(image, mask):
    """Compute average color of masked wall pixels."""
    np_img = np.array(image)
    wall_pixels = np_img[mask == 255]

    if len(wall_pixels) == 0:
        return [200, 200, 200]

    dominant = np.mean(wall_pixels, axis=0).astype(int).tolist()
    return dominant


def recolor_wall_image(image, mask, target_rgb):
    """Apply HSV-based recoloring using mask."""
    cv_img = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)
    hsv = cv2.cvtColor(cv_img, cv2.COLOR_BGR2HSV).astype(np.float32)

    target_bgr = np.uint8([[[target_rgb[2], target_rgb[1], target_rgb[0]]]])
    target_hsv = cv2.cvtColor(target_bgr, cv2.COLOR_BGR2HSV)[0][0]

    h, s, v = cv2.split(hsv)
    wall = (mask == 255)

    h[wall] = target_hsv[0]
    s[wall] = np.clip(s[wall] * 0.6 + target_hsv[1] * 0.4, 0, 255)
    # keep brightness from original

    recolored = cv2.merge([h, s, v]).astype(np.uint8)
    recolored_bgr = cv2.cvtColor(recolored, cv2.COLOR_HSV2BGR)

    recolor_rgb = cv2.cvtColor(recolored_bgr, cv2.COLOR_BGR2RGB)
    original = np.array(image)

    alpha = 0.75
    output = original.copy()
    output[wall] = cv2.addWeighted(original[wall], 1 - alpha, recolor_rgb[wall], alpha, 0)

    return Image.fromarray(output)


def image_to_base64(image):
    """Convert PIL image to base64 string."""
    buffered = io.BytesIO()
    image.save(buffered, format="JPEG")
    return base64.b64encode(buffered.getvalue()).decode('utf-8')


def rgb_to_color_name(rgb):
    r, g, b = rgb
    if r > 200 and g > 200 and b > 200:
        return "white"
    elif r > 150 and g < 100 and b < 100:
        return "red"
    elif r < 100 and g < 100 and b < 100:
        return "black"
    elif r > 150 and g > 100 and b < 100:
        return "brown"
    else:
        return "multi"


def filter_stores_by_distance(shops, user_lat, user_lon, max_distance=50):
    """Filter shops by distance from user location"""
    nearby_shops = []

    for shop in shops:
        try:
            distance = calculate_distance(
                user_lat, user_lon,
                shop['latitude'], shop['longitude']
            )
            if distance <= max_distance:
                shop_with_distance = shop.copy()
                shop_with_distance['distance'] = round(distance, 1)
                nearby_shops.append(shop_with_distance)
        except:
            continue

    # Sort by distance
    nearby_shops.sort(key=lambda x: x['distance'])
    return nearby_shops[:5]  # Return top 5 closest

@wall_bp.route("/analyze-wall", methods=["POST"])
def analyze_wall():
    try:
        data = request.get_json()

        if not data:
            return jsonify({"error": "No data provided"}), 400

        budget = data.get("budget", 1000)
        user_lat = data.get("user_lat")
        user_lon = data.get("user_lon")

        # Accept either a single image or list of images
        wall_images_b64 = data.get("wall_images")
        if not wall_images_b64:
            wall_image_b64 = data.get("wall_image")
            if not wall_image_b64:
                return jsonify({"error": "No wall image provided"}), 400
            wall_images_b64 = [wall_image_b64]

        output_mode = data.get("output_mode", "json").lower()
        selected_color = data.get("selected_color", "blue").lower()

        results = []

        for idx, item in enumerate(wall_images_b64):
            try:
                image_bytes = base64.b64decode(item)
                image_single = Image.open(io.BytesIO(image_bytes))
            except Exception as e:
                return jsonify({"error": f"Invalid image data at index {idx}: {str(e)}"}), 400

            image_single = preprocess_image(image_single, max_size=1024)
            raw_mask = segformer_wall_mask(image_single)
            refined_mask = refine_wall_mask(raw_mask, image_single)
            dominant_rgb = get_dominant_wall_color(image_single, refined_mask)
            wall_color_name_single = rgb_to_color_name(dominant_rgb)

            recommended_palette = [
                {"name": "Blue", "rgb": [70, 130, 180]},
                {"name": "Grey", "rgb": [160, 160, 160]},
                {"name": "Beige", "rgb": [210, 180, 140]},
                {"name": "Pastel", "rgb": [173, 216, 230]}
            ]

            recolored_outputs = []
            recolored_images_map = {}

            for entry in recommended_palette:
                color_rgb = entry["rgb"]
                recolored_image = recolor_wall_image(image_single, refined_mask, color_rgb)
                recolored_outputs.append({
                    "name": entry["name"],
                    "rgb": color_rgb,
                    "hex": rgb_to_hex(color_rgb)
                })
                recolored_images_map[entry["name"].lower()] = recolored_image

            # if image output mode with single selected_color and single input: return direct JPEG now
            if output_mode == "image" and len(wall_images_b64) == 1:
                selected_image = recolored_images_map.get(selected_color)
                if selected_image is None:
                    selected_image = recolored_images_map[recommended_palette[0]["name"].lower()]

                out_io = io.BytesIO()
                selected_image.save(out_io, format="JPEG")
                out_io.seek(0)
                return send_file(out_io, mimetype="image/jpeg")

            results.append({
                "image_index": idx,
                "dominant_color": {
                    "rgb": dominant_rgb,
                    "hex": rgb_to_hex(dominant_rgb),
                    "name": wall_color_name_single
                },
                "wall_mask_base64": image_to_base64(Image.fromarray(cv2.cvtColor(refined_mask, cv2.COLOR_GRAY2RGB))),
                "recolored_wall_images": [
                    {
                        "name": rv["name"],
                        "rgb": rv["rgb"],
                        "hex": rv["hex"],
                        "image_base64": image_to_base64(recolored_images_map[rv["name"].lower()])
                    } for rv in recolored_outputs
                ],
                "recommended_colors": recommended_palette
            })

        # Keep rest of the behavior intact for JSON mode
        first_color_name = results[0]["dominant_color"]["name"]
        furniture = get_furniture_by_budget_and_color(budget, first_color_name)
        all_shops = get_all_shops()
        nearby_stores = []
        if user_lat is not None and user_lon is not None:
            nearby_stores = filter_stores_by_distance(all_shops, user_lat, user_lon)

        response = {
            "status": "success",
            "wall_analysis_batch": results,
            "nearby_stores": nearby_stores,
            "recommended_furniture": [
                {
                    "id": item["id"],
                    "name": item["furniture"],
                    "category": item["category"],
                    "color": item["color"],
                    "style": item["style"],
                    "model_name": item["model_name"],
                    "image_url": f"https://example.com/models/{item['model_name']}.png"
                }
                for item in furniture[:6]
            ]
        }

        return jsonify(response), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500