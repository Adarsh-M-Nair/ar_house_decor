import cv2
import numpy as np


def recolor_wall(image, mask, color):

    mask = mask / 255.0

    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV).astype(np.float32)

    color_bgr = np.uint8([[color]])
    color_hsv = cv2.cvtColor(color_bgr, cv2.COLOR_BGR2HSV)[0][0]

    # Apply color
    hsv[:,:,0] = hsv[:,:,0]*(1-mask) + color_hsv[0]*mask
    hsv[:,:,1] = hsv[:,:,1]*(1-mask) + color_hsv[1]*mask*0.6

    recolored = cv2.cvtColor(hsv.astype(np.uint8), cv2.COLOR_HSV2BGR)

    return recolored