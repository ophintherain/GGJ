using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxSpawner : MonoBehaviour
{
    public GameObject tapDialogBoxPrefab;

    private List<TapDialogBox> spawnedTaps = new List<TapDialogBox>();

    public DialogBoxGroup SpawnGroup()
    {
        GameObject go = Instantiate(
            tapDialogBoxPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        return go.GetComponent<DialogBoxGroup>();
    }

    private IEnumerator SpawnCoroutine(int count, float beatInterval)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnTap();
            yield return new WaitForSeconds(beatInterval);
        }
    }

    private void SpawnTap()
    {
        var go = Instantiate(tapDialogBoxPrefab, transform.position, Quaternion.identity, transform);
        var tap = go.GetComponent<TapDialogBox>();
        tap.Spawn(Time.time);
        spawnedTaps.Add(tap);
    }
}

