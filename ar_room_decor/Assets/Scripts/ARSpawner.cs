using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARSpawner : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public PrefabManager prefabManager;

    private GameObject spawnedObject;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public void SpawnObject(string modelName)
    {
        if (raycastManager.Raycast(
            new Vector2(Screen.width / 2, Screen.height / 2),
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            GameObject prefab = prefabManager.GetPrefab(modelName);

            if (prefab != null)
            {
                if (spawnedObject != null)
                    Destroy(spawnedObject);

                spawnedObject = Instantiate(prefab, hitPose.position, hitPose.rotation);
            }
        }
    }
}