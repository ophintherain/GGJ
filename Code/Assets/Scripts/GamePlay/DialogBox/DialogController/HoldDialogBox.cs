using UnityEngine;

public class HoldDialogBox : DialogBoxBase
{
    [Header("Hold Settings")]
    public float HoldInterval = 1f;            // 一拍时长（建议由 BeatManager 注入）
    public float pressWindow = 0.25f;          // 按下允许的时间窗（围绕 hitTime）
    public float releaseWindow = 0.25f;        // 松开允许的时间窗（围绕 hitTime + beatInterval）
    public KeyCode holdKey = KeyCode.Space;

    private bool resolved = false;
    private bool holding = false;

    private float pressTime = -1f;

    // hold 的目标：按下目标点、松开目标点
    private float PressTarget => hitTime;
    private float ReleaseTarget => hitTime + HoldInterval;

    private void Update()
    {
        HandlePlayerInput();
    }

    public override void Spawn(float spawnTime)
    {
        base.Spawn(spawnTime);

        resolved = false;
        holding = false;
        pressTime = -1f;

        // 这里不改 hitTime，让 Group 来设置：
        // - 示范生成时：hitTime = spawnTime + 4*beatInterval（玩家小节开始的对应拍）
        // - 需要时你也可以单独给某个 hold 设 hitTime
    }

    public override void HandlePlayerInput()
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;

        // ✅ 只允许队首的 Hold 响应空格（按顺序清）
        if (!JudgeQueue.Instance.IsMyTurn(this)) return;

        float now = Time.time;

        // 1) 按下：必须发生在 PressTarget 附近窗口
        if (!holding && Input.GetKeyDown(holdKey))
        {
            if (Mathf.Abs(now - PressTarget) <= pressWindow)
            {
                holding = true;
                pressTime = now;
                // Debug.Log("Hold press accepted");
            }
            else
            {
                // 过早/过晚按下：直接判 Miss（但不销毁，只失效），并推进队列
                resolved = true;
                Debug.Log("Hold Miss (bad press timing)");
                ResolveMissAndDisable();
            }
        }

        // 2) 松开：必须发生在 ReleaseTarget 附近窗口
        if (holding && Input.GetKeyUp(holdKey))
        {
            holding = false;

            if (Mathf.Abs(now - ReleaseTarget) <= releaseWindow)
            {
                resolved = true;
                Debug.Log("Hold Hit!");
                ResolveHitAndDespawn(); // ✅ 命中才消失
            }
            else
            {
                resolved = true;
                Debug.Log("Hold Miss (bad release timing)");
                ResolveMissAndDisable(); // ✅ Miss 只失效不消失
            }
        }

        // 3) 兜底：如果已经按住了，但超过 ReleaseTarget+releaseWindow 还没松开，也算 Miss
        if (holding && now > ReleaseTarget + releaseWindow)
        {
            holding = false;
            resolved = true;
            Debug.Log("Hold Miss (overheld)");
            ResolveMissAndDisable();
        }
    }

    public override void OnAutoMiss()
    {
        // ✅ 超时 Miss：不销毁，只失效，推进队列
        if (resolved) return;
        resolved = true;

        holding = false;
        Debug.Log("Hold AutoMiss (disable only)");
        ResolveMissAndDisable();
    }
}



