using System.Collections;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Header("Rhythm")] public float bpm = 130f;
    public int beatsPerBar = 4;

    [Header("Group Timeline")] public float startOffset = 2f;

    // ===== 自动计算出来的 =====
    [HideInInspector] public float beatInterval;
    [HideInInspector] public float barDuration;
    [HideInInspector] public float groupInterval;
    [HideInInspector] public float demoDuration;
    [HideInInspector] public float playDuration;

    public DialogBoxSpawner spawner;
    public EventCounter eventCounter; // 你之前要 demo1/demo2/play1/play2 倒数的那个

    private DialogBoxGroup currentGroup;
    private Coroutine groupFlowCoroutine;

    [Header("Mask Timeline")] public MaskController maskController;
    public GameObject mask;
    public float idlePhaseDuration = 2f;
    public float frenzyPhaseDuration = 2f;

    private int groupIndex = 0;

    private int groupHit;
    private int groupMiss;
    private int groupTotal;

    // ✅ 添加 BossController 引用
    private BossController bossController;

    private void Awake()
    {
        RecalculateTiming();
        // 自动查找子物体中的 BossController
        bossController = GetComponentInChildren<BossController>();
        if (bossController != null)
        {
            Debug.Log($"Found BossController: {bossController.name}");
        }
        else
        {
            Debug.LogWarning("No BossController found in children");
        }
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

        // ===== 本组统计初始化 =====
        groupHit = 0;
        groupMiss = 0;
        groupTotal = group.GetTotalNotes();

        // ✅ 订阅判定结果
        DialogBoxBase.OnResolved += OnDialogResolved;

        float demoDuration = groupInterval * 0.5f;
        float playDuration = groupInterval * 0.5f;

        // ✅ 让 Group 自己按事件表生成，并在生成/hitTime 打点 demoN/playN
        StartCoroutine(group.DemoSpawnByGroupEvents(demoDuration, eventCounter));

        // Demo 段持续 demoDuration
        yield return new WaitForSeconds(demoDuration);

        group.BeginPlay();

        // Play 段持续 playDuration
        yield return new WaitForSeconds(playDuration);

        DialogBoxBase.OnResolved -= OnDialogResolved;

        // ✅ 判断是否"全对"（没有Miss）
        bool isPerfect = (groupMiss == 0) && (groupTotal > 0);

        // ✅ 根据结果改变Boss状态
        if (bossController != null)
        {
            if (isPerfect)
            {
                // 全对 → Angry
                bossController.SetCharacterState(CharacterState.Angry);
                Debug.Log($"Group {groupIndex}: PERFECT → Boss angry");
            }
            else
            {
                // 有错 → Laugh
                bossController.SetCharacterState(CharacterState.Laugh);
                Debug.Log($"Group {groupIndex}: NOT PERFECT → Boss laugh");
            }
        }

        OnGroupFinished(groupIndex, group, isPerfect, groupHit, groupMiss, groupTotal);

        groupIndex++;
    }

    private void OnDialogResolved(DialogBoxBase box, bool isHit)
    {
        // 只统计本组期间发生的（我们在本组协程开始订阅，结束取消订阅，所以这里天然是本组）
        if (isHit) groupHit++;
        else groupMiss++;

        // 你想看得更清楚可以加：
        // Debug.Log($"[Group#{groupIndex}] Resolved: {box.name}  hit={isHit}  hit={groupHit}/{groupTotal} miss={groupMiss}");
    }

    /// <summary>
    /// ✅ 你要写“这一组全对触发什么”，就写这里（尽量在 BeatManager）
    /// </summary>
    private void OnGroupPerfect(int index, DialogBoxGroup group)
    {
        Debug.Log($"=== GROUP PERFECT! index={index} ===");

    }

    /// <summary>
    /// （可选）每组结束都会走，方便你做统计/调试
    /// </summary>
    private void OnGroupFinished(int index, DialogBoxGroup group, bool perfect, int hit, int miss, int total)
    {
        Debug.Log($"=== GROUP FINISHED index={index} perfect={perfect} hit={hit}/{total} miss={miss} ===");
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
        if (maskController != null)
        {
            maskController.DestroyAllMasks();
        }

        // ✅ 隐藏mask容器（如果需要的话）
        if (mask != null)
        {
            mask.SetActive(false);
        }
        // 在这里执行一些具体的事件
        // 检查游戏结果
        CheckGameResult();
    }

    private void CheckGameResult()
    {
        int levelIndex = 1;
        bool isWin = false;

        // 获取当前关卡
        if (LevelManager.Instance != null)
        {
            levelIndex = LevelManager.Instance.GetCurrentLevelIndex();
        }

        // 检查是否胜利
        BossController boss = FindObjectOfType<BossController>();
        if (boss != null)
        {
            isWin = boss.GetCurrentHealth() <= 0;
        }
        
        // 显示结果
        if (ResultUIController.Instance != null)
        {
            ResultUIController.Instance.ShowResult(levelIndex, isWin);
        }
    }
}