using System.Collections.Generic;
using UnityEngine;

public class JudgeQueue : MonoBehaviour
{
    public static JudgeQueue Instance { get; private set; }

    private readonly Queue<DialogBoxBase> q = new Queue<DialogBoxBase>();
    public DialogBoxBase Current => q.Count > 0 ? q.Peek() : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Clear() => q.Clear();

    public void Enqueue(DialogBoxBase box)
    {
        if (box == null) return;
        q.Enqueue(box);
    }

    public void Consume(DialogBoxBase box)
    {
        if (q.Count == 0) return;
        if (q.Peek() != box) return;
        q.Dequeue();
    }

    // 严格顺序：必须轮到你
    public bool IsMyTurn(DialogBoxBase box) => Current == box;

    // 轮到你 + 在窗口内
    public bool CanJudgeNow(DialogBoxBase box)
    {
        if (box == null) return false;
        if (Current != box) return false;

        float now = Time.time;
        return now >= box.hitTime - box.earlyWindow && now <= box.hitTime + box.lateWindow;
    }

    private void Update()
    {
        if (q.Count == 0) return;

        var cur = q.Peek();
        if (cur == null)
        {
            q.Dequeue();
            return;
        }

        // ✅ 超过最晚窗口：自动 Miss，但不销毁物体（只让它失效），并推进队列
        float now = Time.time;
        if (now > cur.hitTime + cur.lateWindow)
        {
            cur.OnAutoMiss();
        }
    }
}


