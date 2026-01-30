using UnityEngine;
using UnityEngine.EventSystems;

public class MaskSwipeInput : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler
{
    public MaskController frenzy;
    public BossController boss;

    public float damagePerSwipe = 1f; // 你后面再改数值逻辑
    public void OnPointerEnter(PointerEventData eventData)
    {

        // 只有按住时进入才算有效 swipe 流程
        if (Input.GetMouseButton(0))
        {
            if (!frenzy.canSwipe) return;

            // 👉 现在是“疯狂划动，划多少算多少”
            boss.DecreaseHealth(damagePerSwipe);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {

            if (!frenzy.canSwipe) return;

            // 👉 现在是“疯狂划动，划多少算多少”
            boss.DecreaseHealth(damagePerSwipe);
        }

    }

}

