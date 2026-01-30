using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GroupEvent
{
    [Tooltip("相对本组 Demo 开始的时间（秒）")]
    public float time;

    [Tooltip("要在这个时间点激活的 DialogBox（必须是本 Group 的子物体）")]
    public DialogBoxBase dialogBox;
}

public class DialogBoxGroup : MonoBehaviour
{

    public List<GroupEvent> events = new List<GroupEvent>();

    private void Awake()
    {
        // 确保所有音符一开始是隐藏的
        foreach (var e in events)
        {
            if (e.dialogBox != null)
                e.dialogBox.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 示范阶段：按 Group Inspector 里填写的时间激活
    /// </summary>
     // ✅ 现在只关心：示范结束到游玩的偏移（通常是一小节时长）
    public IEnumerator DemoSpawnByGroupEvents(float demoToPlayOffset, EventCounter counter)
    {
        float demoStartTime = Time.time;

        var ordered = events
            .Where(e => e.dialogBox != null)
            .OrderBy(e => e.time)
            .ToList();

        // 先把所有 playN 的“打点时刻”排好：就是每个音符的 hitTime
        // hitTime = demoStart + demoToPlayOffset + event.time
        var playTimes = ordered
            .Select(e => demoStartTime + demoToPlayOffset + e.time)
            .ToList();

        // ✅ 并行：负责在 hitTime 时刻依次触发 playN
        StartCoroutine(MarkPlayAtHitTimes(playTimes, counter));

        // ✅ Demo 阶段按 time 激活，触发 demoN
        foreach (var e in ordered)
        {
            float spawnAt = demoStartTime + e.time;
            float wait = spawnAt - Time.time;
            if (wait > 0f) yield return new WaitForSeconds(wait);

            counter?.MarkDemo(); // demoN：第 N 个生成时刻

            var box = e.dialogBox;
            box.gameObject.SetActive(true);
            box.Spawn(Time.time);

            box.hitTime = demoStartTime + demoToPlayOffset + e.time;
            JudgeQueue.Instance.Enqueue(box);
        }
    }

    private IEnumerator MarkPlayAtHitTimes(System.Collections.Generic.List<float> hitTimes, EventCounter counter)
    {
        // hitTimes 已经按 event.time 排过序，因此天然升序
        for (int i = 0; i < hitTimes.Count; i++)
        {
            float t = hitTimes[i];
            float wait = t - Time.time;
            if (wait > 0f) yield return new WaitForSeconds(wait);

            counter?.MarkPlay(); // playN：到第 N 个 hitTime 时刻
        }
    }

    public void BeginPlay()
    {
        foreach (var e in events)
        {
            if (e.dialogBox != null)
                e.dialogBox.EnableJudge();
        }
    }
}


