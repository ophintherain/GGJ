using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeDialogBox : DialogBoxBase,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private bool resolved = false;

    // 是否在这个框内
    private bool isInside = false;

    // 是否正在按住左键（不要求必须从框上按下）
    private bool isHoldingMouse = false;

    // 是否已经在按住的情况下“进入过”
    private bool enteredWhileHolding = false;

    private void Awake()
    {
        // Swipe 属于 Pointer 通道
        // （如果你之后要区分 channel，可以在这里设置）
    }

    private void Update()
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;

        // 必须轮到我（按顺序清）
        if (!JudgeQueue.Instance.IsMyTurn(this)) return;

        // 如果已经过了时间窗，这个对象会被 JudgeQueue.Update() 调用 OnAutoMiss()
        // 这里不用额外处理超时

        // 记录是否按住鼠标
        // 只要按住就算 holding，不要求必须在框上按下（符合你描述的“过程要保持点击”）
        if (Input.GetMouseButtonDown(0)) isHoldingMouse = true;
        if (Input.GetMouseButtonUp(0))
        {
            // 松开还没完成 Swipe -> Miss（失效不消失）
            if (!resolved)
            {
                resolved = true;
                Debug.Log("Swipe Miss (released early)");

                DisableRaycast();
                ResolveMissAndDisable();
            }
            return;
        }

        // 如果正在按住且已经进入过，并且现在不在框内（意味着 Exit 已发生但可能因事件丢失）
        // 一般 Exit 会走事件，这里留作兜底
        if (isHoldingMouse && enteredWhileHolding && !isInside)
        {
            TryResolveSwipeHit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;
        if (!JudgeQueue.Instance.IsMyTurn(this)) return;

        // 必须在判定窗口内才开始认这个 Swipe
        if (!JudgeQueue.Instance.CanJudgeNow(this)) return;

        isInside = true;

        // 只有按住时进入才算有效 swipe 流程
        if (Input.GetMouseButton(0))
        {
            isHoldingMouse = true;
            enteredWhileHolding = true;
            // Debug.Log("Swipe: entered while holding");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;
        if (!JudgeQueue.Instance.IsMyTurn(this)) return;

        isInside = false;

        // 只有“按住期间进入过”，并且仍在按住，Exit 才算成功
        if (enteredWhileHolding && Input.GetMouseButton(0))
        {
            TryResolveSwipeHit();
        }
    }

    private void TryResolveSwipeHit()
    {
        if (resolved) return;

        // 再次确认：仍在判定窗口内
        if (!JudgeQueue.Instance.CanJudgeNow(this))
            return;

        resolved = true;
        Debug.Log("Swipe Hit!");
        ResolveHitAndDespawn();
    }

    public override void OnAutoMiss()
    {
        if (resolved) return;
        resolved = true;

        Debug.Log("Swipe Miss (timeout, disable only)");

        DisableRaycast();
        ResolveMissAndDisable();
    }

    private void DisableRaycast()
    {
        // Miss 后“互动无效” —— UI 用 raycastTarget 关掉最直观
        var g = GetComponent<Graphic>();
        if (g != null) g.raycastTarget = false;
    }

    public override void HandlePlayerInput() { }
}

