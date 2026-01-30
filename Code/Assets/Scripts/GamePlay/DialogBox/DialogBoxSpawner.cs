using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxSpawner : MonoBehaviour
{
    public GameObject[] groupPrefabs;
    private int index = 0;

    public bool HasNextGroup => groupPrefabs != null && index < groupPrefabs.Length;

    public DialogBoxGroup SpawnNextGroup()
    {
        if (!HasNextGroup)
        {
            Debug.Log("[Spawner] No more groups to spawn.");
            return null;
        }

        GameObject prefab = groupPrefabs[index];
        index++;

        if (prefab == null)
        {
            Debug.LogError($"[Spawner] groupPrefabs[{index - 1}] is NULL!");
            return null;
        }

        GameObject go = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        DialogBoxGroup group = go.GetComponent<DialogBoxGroup>();

        if (group == null)
        {
            Debug.LogError($"[Spawner] Prefab {prefab.name} has no DialogBoxGroup!");
            Destroy(go);
            return null;
        }

        Debug.Log($"[Spawner] Spawned group {index}/{groupPrefabs.Length}: {prefab.name}");
        return group;
    }
}



