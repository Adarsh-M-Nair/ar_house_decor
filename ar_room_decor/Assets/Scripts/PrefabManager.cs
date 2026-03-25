using UnityEngine;
using System.Collections.Generic;

public class PrefabManager : MonoBehaviour
{
    public List<GameObject> prefabs;

    private Dictionary<string, GameObject> prefabDict;

    void Awake()
    {
        prefabDict = new Dictionary<string, GameObject>();

        foreach (var prefab in prefabs)
        {
            prefabDict[prefab.name] = prefab;
        }
    }

    public GameObject GetPrefab(string name)
    {
        if (prefabDict.ContainsKey(name))
            return prefabDict[name];

        Debug.LogError("Prefab not found: " + name);
        return null;
    }
}