import numpy as np
from models.wall_segmentation_model import get_wall_mask


def analyze_wall(image, image_path):

    mask = get_wall_mask(image_path)

    wall_pixels = image[mask > 0]

    if len(wall_pixels) == 0:
        dominant_color = (0,0,0)
    else:
        dominant_color = np.mean(wall_pixels, axis=0)

    style = "modern"

    return mask, dominant_color, style