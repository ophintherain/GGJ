using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapDialogBox : DialogBoxBase, IPointerClickHandler
{
    private bool resolved = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;

        // ✅ 必须轮到我 + 必须在窗口内
        if (!JudgeQueue.Instance.CanJudgeNow(this)) return;

        resolved = true;
        Debug.Log("Tap Hit!");
        ResolveHitAndDespawn();
    }

    public override void OnAutoMiss()
    {
        if (resolved) return;
        resolved = true;

        Debug.Log("Tap Miss (disable only, no despawn)");

        // ✅ 禁用 UI 点击（仍然留在屏幕上）
        var g = GetComponent<Graphic>();
        if (g != null) g.raycastTarget = false;

        ResolveMissAndDisable();
    }

    public override void HandlePlayerInput() { }
}




