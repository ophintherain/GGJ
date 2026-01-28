using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatManager : MonoBehaviour
{
    public float beatInterval = 1f;
    public DialogBoxSpawner spawner;

    private List<TapDialogBox> currentGroup;

    private enum RhythmPhase
    {
        Demo,
        Play
    }

    private RhythmPhase phase;

    private void Start()
    {
        StartCoroutine(RhythmLoop());
    }

    private IEnumerator RhythmLoop()
    {
        // ===== Demo =====
        phase = RhythmPhase.Demo;
        Debug.Log("Demo Phase");

        DialogBoxGroup group = spawner.SpawnGroup();

        // ⭐ Demo 阶段：每秒生成一个 Tap
        yield return StartCoroutine(group.SpawnByBeat(beatInterval));

        // ===== Play =====
        phase = RhythmPhase.Play;
        Debug.Log("Play Phase");

        group.EnableJudgeAll();
    }


}
