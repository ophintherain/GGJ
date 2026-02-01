using System.Collections;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Header("Rhythm")]
    public float bpm = 130f;
    public int beatsPerBar = 4;

    [Header("Group Timeline")]
    public float startOffset = 2f;

    // ===== 自动计算出来的 =====
    [HideInInspector] public float beatInterval;
    [HideInInspector] public float barDuration;
    [HideInInspector] public float groupInterval;
    [HideInInspector] public float demoDuration;
    [HideInInspector] public float playDuration;

    public DialogBoxSpawner spawner;
    public EventCounter eventCounter;      // 你之前要 demo1/demo2/play1/play2 倒数的那个

    private DialogBoxGroup currentGroup;
    private Coroutine groupFlowCoroutine;

    [Header("Mask Timeline")]
    public MaskController maskController;
    public GameObject mask;
    public float idlePhaseDuration = 2f;
    public float frenzyPhaseDuration = 2f;

    private void Awake()
    {
        RecalculateTiming();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RecalculateTiming();
    }
#endif
    private void RecalculateTiming()
    {
        beatInterval = 60f / bpm;
        barDuration = beatInterval * beatsPerBar;
        groupInterval = barDuration * 2f;

        demoDuration = barDuration;
        playDuration = barDuration;
    }

    private void Start()
    {
        StartCoroutine(GroupTimelineLoop());
    }

    private IEnumerator GroupTimelineLoop()
    {
        Debug.Log("=== GROUP TIMELINE START ===");
        // 起始偏移
        if (startOffset > 0f)
            yield return new WaitForSeconds(startOffset);

        while (spawner.HasNextGroup)
        {
            CleanupCurrentBatch();

            currentGroup = spawner.SpawnNextGroup();
            if (currentGroup == null)
                break;

            groupFlowCoroutine = StartCoroutine(RunOneGroupFlow(currentGroup));

            yield return new WaitForSeconds(groupInterval);
        }

        // ===== 播完最后一个 group =====
        yield return StartCoroutine(OnAllGroupsFinished());
    }




    private IEnumerator RunOneGroupFlow(DialogBoxGroup group)
    {
        JudgeQueue.Instance.Clear();
        eventCounter?.ResetCounters();

        float demoDuration = groupInterval * 0.5f;
        float playDuration = groupInterval * 0.5f;

        // ✅ 让 Group 自己按事件表生成，并在生成/hitTime 打点 demoN/playN
        StartCoroutine(group.DemoSpawnByGroupEvents(demoDuration, eventCounter));

        // Demo 段持续 demoDuration
        yield return new WaitForSeconds(demoDuration);

        group.BeginPlay();

        // Play 段持续 playDuration
        yield return new WaitForSeconds(playDuration);
    }


    private void CleanupCurrentBatch()
    {
        // 1) 停止上一批流程协程
        if (groupFlowCoroutine != null)
        {
            StopCoroutine(groupFlowCoroutine);
            groupFlowCoroutine = null;
        }

        // 2) 清理队列（防止 current 卡住/误判）
        if (JudgeQueue.Instance != null)
            JudgeQueue.Instance.Clear();

        // 3) 销毁上一批 group（如果存在）
        if (currentGroup != null)
        {
            Destroy(currentGroup.gameObject);
            currentGroup = null;
        }
    }

    private IEnumerator OnAllGroupsFinished()
    {

        CleanupCurrentBatch();

        mask.SetActive(true);
        maskController.StartIdlePhase();

        // 沉寂阶段持续时间
        yield return new WaitForSeconds(idlePhaseDuration);

        // 切换到燃阶段
        maskController.StartFrenzyPhase();

        // 燃阶段持续时间
        yield return new WaitForSeconds(frenzyPhaseDuration);

        // 燃阶段结束后的逻辑
        OnFrenzyPhaseEnd();
    }

    private void OnFrenzyPhaseEnd()
    {
        // 燃阶段结束后的处理逻辑（例如结算、回到主界面等）
        Debug.Log("Frenzy Phase Ended. Triggering End Events...");
        // 在这里执行一些具体的事件
    }



}


