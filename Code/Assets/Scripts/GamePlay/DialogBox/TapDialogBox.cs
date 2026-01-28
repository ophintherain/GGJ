using UnityEngine;
using UnityEngine.EventSystems;

public class TapDialogBox : DialogBoxBase, IPointerClickHandler
{
    public float hitWindow = 0.3f;
    private bool judged = false;

    // ✅ 只有点到这个 UI 元素才会触发
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive || !canBeJudged || judged) return;

        float delta = Mathf.Abs(Time.time - spawnTime);

        if (delta <= hitWindow)
        {
            Debug.Log("Perfect!");
        }
        else
        {
            Debug.Log("Miss!");
        }

        judged = true;
        isActive = false;
        Destroy(gameObject);
    }

    public override void HandlePlayerInput() { }
}



