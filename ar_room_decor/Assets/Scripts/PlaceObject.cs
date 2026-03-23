using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlaceObject : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject[] objectPrefabs;
    public TextMeshProUGUI debugText;
    public GameObject deleteButton;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private GameObject selectedObject = null;
    private Camera arCamera;
    private int currentPrefabIndex = 0;

    private List<GameObject> placedObjects = new List<GameObject>();

    // Rotation variables
    private float previousAngle = 0f;
    private bool isRotating = false;

    void OnEnable() { EnhancedTouchSupport.Enable(); }
    void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        arCamera = Camera.main;
        if (deleteButton != null)
            deleteButton.SetActive(false);
        SetPlanesVisible(true);
        SetDebug("🔍 Scanning... point at floor and move slowly");
    }

    void Update()
    {
        if (objectPrefabs == null || objectPrefabs.Length == 0 || raycastManager == null) return;

        int planeCount = 0;
        foreach (var plane in planeManager.trackables)
            planeCount++;

        SetDebug("🔍 Planes: " + planeCount + " | Objects: " + placedObjects.Count + " - Tap to place!");

        // TWO FINGER ROTATION
        if (Touch.activeTouches.Count == 2 && selectedObject != null)
        {
            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];

            float currentAngle = GetAngleBetweenTouches(touch0.screenPosition, touch1.screenPosition);

            if (!isRotating)
            {
                previousAngle = currentAngle;
                isRotating = true;
            }
            else
            {
                float angleDiff = currentAngle - previousAngle;
                selectedObject.transform.Rotate(0, -angleDiff, 0);
                previousAngle = currentAngle;
                SetDebug("🔄 Rotating object...");
            }
            return;
        }
        else
        {
            isRotating = false;
        }

        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];

        // DRAG to move selected object
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && selectedObject != null)
        {
            if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                selectedObject.transform.position = hits[0].pose.position;
                SetDebug("↔️ Moving object...");
            }
            return;
        }

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        // IGNORE touches on UI buttons
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        // CHECK if tapping an existing placed object
        Ray ray = arCamera.ScreenPointToRay(touch.screenPosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject tappedObject = hitInfo.collider.gameObject;
            GameObject placedRoot = GetPlacedRoot(tappedObject);

            if (placedRoot != null)
            {
                if (placedRoot == selectedObject)
                {
                    DeselectObject();
                    SetDebug("✅ Deselected!");
                }
                else
                {
                    SelectObject(placedRoot);
                    SetDebug("✅ Selected! Drag to move, 2 fingers to rotate.");
                }
                return;
            }
        }

        // Tapped empty space - deselect if something is selected
        if (selectedObject != null)
        {
            DeselectObject();
            SetDebug("✅ Deselected!");
            return;
        }

        // PLACE new object on plane
        if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Pick current prefab and advance index
            GameObject prefabToPlace = objectPrefabs[currentPrefabIndex];
            currentPrefabIndex = (currentPrefabIndex + 1) % objectPrefabs.Length;

            GameObject obj = Instantiate(prefabToPlace, hitPose.position, Quaternion.identity);
            obj.AddComponent<ARAnchor>();

            if (obj.GetComponent<Collider>() == null)
                obj.AddComponent<BoxCollider>();

            placedObjects.Add(obj);
            SetDebug("✅ Placed: " + prefabToPlace.name + " | Next: " + objectPrefabs[currentPrefabIndex].name);
        }
        else
        {
            SetDebug("❌ No plane found - keep scanning floor");
        }
    }

    private GameObject GetPlacedRoot(GameObject obj)
    {
        foreach (var placed in placedObjects)
        {
            if (obj == placed || obj.transform.IsChildOf(placed.transform))
                return placed;
        }
        return null;
    }

    float GetAngleBetweenTouches(Vector2 pos0, Vector2 pos1)
    {
        Vector2 direction = pos1 - pos0;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    void SetPlanesVisible(bool visible)
    {
        foreach (var plane in planeManager.trackables)
        {
            var mr = plane.GetComponent<MeshRenderer>();
            var lr = plane.GetComponent<LineRenderer>();
            if (mr != null) mr.enabled = visible;
            if (lr != null) lr.enabled = visible;
        }

        planeManager.trackablesChanged.AddListener((args) =>
        {
            foreach (var plane in args.added)
            {
                var mr = plane.GetComponent<MeshRenderer>();
                var lr = plane.GetComponent<LineRenderer>();
                if (mr != null) mr.enabled = visible;
                if (lr != null) lr.enabled = visible;
            }
        });
    }

    void SelectObject(GameObject obj)
    {
        if (selectedObject != null) DeselectObject();
        selectedObject = obj;

        var renderer = selectedObject.GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material.color = new Color(0.5f, 0.8f, 1f);

        if (deleteButton != null)
            deleteButton.SetActive(true);
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            var renderer = selectedObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.white;
        }
        selectedObject = null;
        if (deleteButton != null)
            deleteButton.SetActive(false);
    }

    public void DeleteSelected()
    {
        if (selectedObject != null)
        {
            placedObjects.Remove(selectedObject);
            Destroy(selectedObject);
            selectedObject = null;
            if (deleteButton != null)
                deleteButton.SetActive(false);

            if (placedObjects.Count == 0)
            {
                SetPlanesVisible(true);
                SetDebug("🔍 Scanning for planes...");
            }
            else
            {
                SetDebug("🗑️ Deleted! " + placedObjects.Count + " object(s) remaining.");
            }
        }
    }

    public void SetFurniture(GameObject newPrefab)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i] == newPrefab)
            {
                currentPrefabIndex = i;
                break;
            }
        }
        SetDebug("✅ Selected: " + newPrefab.name + " - Tap floor to place!");
    }

    void SetDebug(string msg)
    {
        if (debugText != null)
            debugText.text = msg;
    }
}