import torch
import numpy as np
import cv2
from transformers import SegformerImageProcessor, SegformerForSemanticSegmentation
from PIL import Image

processor = SegformerImageProcessor.from_pretrained(
    "nvidia/segformer-b0-finetuned-ade-512-512"
)

model = SegformerForSemanticSegmentation.from_pretrained(
    "nvidia/segformer-b0-finetuned-ade-512-512"
)

model.eval()


def get_wall_mask(image_path):

    image_cv = cv2.imread(image_path)
    if image_cv is None:
        raise ValueError("Image load failed")

    image_rgb = cv2.cvtColor(image_cv, cv2.COLOR_BGR2RGB)
    image = Image.fromarray(image_rgb)

    inputs = processor(images=image, return_tensors="pt")

    with torch.no_grad():
        outputs = model(**inputs)

    logits = outputs.logits

    upsampled_logits = torch.nn.functional.interpolate(
        logits,
        size=image.size[::-1],
        mode="bilinear",
        align_corners=False,
    )

    pred_seg = upsampled_logits.argmax(dim=1)[0].cpu().numpy()

    mask = np.zeros_like(pred_seg, dtype=np.float32)
    mask[pred_seg == 0] = 1.0

    # Morphology
    kernel = np.ones((7,7), np.uint8)
    mask = cv2.morphologyEx(mask, cv2.MORPH_CLOSE, kernel)
    mask = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel)

    # Erode (important)
    mask = cv2.erode(mask, np.ones((3,3), np.uint8), iterations=1)

    # Blur
    mask = cv2.GaussianBlur(mask, (11,11), 0)

    # Edge clipping
    gray = cv2.cvtColor(image_cv, cv2.COLOR_BGR2GRAY)
    edges = cv2.Canny(gray, 80, 160)
    edges = cv2.dilate(edges, np.ones((2,2), np.uint8))
    mask[edges > 0] = 0

    # Floor removal
    h, w = mask.shape
    mask[int(h*0.92):h, :] = 0

    mask = (mask * 255).astype(np.uint8)

    return mask