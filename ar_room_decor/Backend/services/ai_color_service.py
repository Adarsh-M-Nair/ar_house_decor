import cv2
import numpy as np


def extract_dominant_color(image_path):

    image = cv2.imread(image_path)
    image = cv2.resize(image, (100, 100))

    pixels = image.reshape((-1, 3))
    pixels = np.float32(pixels)

    # KMeans clustering
    criteria = (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_MAX_ITER, 10, 1.0)
    _, labels, centers = cv2.kmeans(
        pixels, 3, None, criteria, 10, cv2.KMEANS_RANDOM_CENTERS
    )

    centers = np.uint8(centers)

    # Find most frequent cluster
    counts = np.bincount(labels.flatten())
    dominant = centers[np.argmax(counts)]

    return dominant.tolist()