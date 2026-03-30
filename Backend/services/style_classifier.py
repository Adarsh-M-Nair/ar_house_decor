import torch
import torchvision.transforms as transforms
from torchvision import models
from PIL import Image


model = models.resnet18(pretrained=True)
model.eval()

transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor()
])


def classify_style(image_path):

    image = Image.open(image_path)
    image = transform(image).unsqueeze(0)

    with torch.no_grad():
        output = model(image)

    _, predicted = torch.max(output, 1)

    # Simplified mapping
    style_classes = {
        0: "modern",
        1: "minimal",
        2: "traditional"
    }

    return style_classes.get(predicted.item(), "modern")