# AR Room Decor

An Augmented Reality (AR) mobile application for visualizing home decor items in real-world spaces using Unity and AR Foundation.

## Features

- **AR Object Placement**: Place 3D furniture and decor items on detected planes in your room
- **Interactive Manipulation**: Select, move, and rotate placed objects with touch gestures
- **Wall Analysis**: Capture and analyze wall images for decor recommendations
- **Budget Planning**: Input budget constraints for personalized suggestions
- **Multi-Scene Experience**: Navigate through splash, home, input, AR placement, and results scenes

## Requirements

- **Unity Version**: 6000.3.7f1 (Unity 6)
- **Platform**: Android (with ARCore support)
- **Minimum Android Version**: API level 24 (Android 7.0)
- **Device Requirements**: ARCore-compatible Android device with camera

## Dependencies

### Unity Packages
- AR Foundation 6.4.1
- ARCore XR Plugin 6.4.1
- Input System 1.18.0
- TextMesh Pro
- Unity UI
- Native Gallery (for image saving)

### Key Features Used
- Plane detection and tracking
- Raycasting for object placement
- Touch gesture recognition
- Scene management
- UI navigation

## Project Structure

```
Assets/
├── Scenes/           # Unity scenes (Splash, Home, Input, AR, Result)
├── Scripts/          # C# scripts for game logic
├── Prefabs/          # Reusable game objects
├── Materials/        # Material assets
├── Models/           # 3D model assets
├── Resources/        # Runtime-loaded assets
├── Fonts/            # Font assets
└── XR/              # XR-related configurations
```

## Setup Instructions

1. **Clone or Download** the project
2. **Open in Unity**: Launch Unity Hub and open the project folder
3. **Install Dependencies**: Unity will automatically resolve package dependencies
4. **Build Settings**:
   - Platform: Android
   - Texture Compression: ASTC
   - Minimum API Level: Android 7.0 (API 24)
5. **Player Settings**:
   - XR Plug-in Management: Enable ARCore
   - Camera Usage Description: "Required for AR functionality"
   - Write Permission: External (SDCard)

## Usage

1. **Launch App**: Start the application on an ARCore-compatible device
2. **Grant Permissions**: Allow camera and storage access
3. **Navigate Scenes**:
   - **Splash**: Initial loading screen
   - **Home**: Main menu
   - **Input**: Enter budget and capture/upload wall image
   - **AR Scene**: Place and manipulate 3D objects in your space
   - **Results**: View recommendations and save designs

## AR Controls

- **Tap on Plane**: Place next object from the prefab list
- **Tap on Object**: Select/deselect objects
- **Drag Selected Object**: Move object to new position
- **Two-Finger Rotate**: Rotate selected object
- **Delete**: Remove selected object

## Development Notes

- Built with Unity's AR Foundation for cross-platform AR support
- Uses Enhanced Touch API for gesture recognition
- Implements AR anchoring for stable object placement
- Includes debug text for development and troubleshooting

## Building for Android

1. **Switch Platform**: File > Build Settings > Android
2. **Configure Player Settings**:
   - Company Name: Your company
   - Product Name: AR Room Decor
   - Version: 1.0.0
   - Bundle Identifier: com.yourcompany.arroomdecor
3. **Build**: Build > Build APK or Build App Bundle

## Troubleshooting

- **AR Not Working**: Ensure device supports ARCore and camera permission is granted
- **Objects Not Placing**: Move device slowly to allow plane detection
- **Performance Issues**: Close other apps and ensure good lighting
- **Build Errors**: Check that all required packages are installed

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make changes and test on device
4. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
