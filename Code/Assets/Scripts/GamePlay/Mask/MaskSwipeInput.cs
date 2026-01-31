using UnityEngine;
using UnityEngine.EventSystems;

public class MaskSwipeInput : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler
{
    public MaskController mask;
    public BossController boss;

    public float damagePerSwipe = 1f; // 你后面再改数值逻辑

    private void Start()
    {
        // 获取父物体的 MaskController
        mask = GetComponentInParent<MaskController>();

        // 获取父物体的父物体中的 BossController
        boss = GameObject.Find("boss").GetComponent<BossController>();


        // 检查是否成功获取到组件
        if (mask == null)
            Debug.LogError("MaskController not found!");
        if (boss == null)
            Debug.LogError("BossController not found!");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 只有按住时进入才算有效 swipe 流程
        if (Input.GetMouseButton(0))
        {
            // 👉 现在是“疯狂划动，划多少算多少”
            boss.DecreaseHealth(damagePerSwipe);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            // 👉 现在是“疯狂划动，划多少算多少”
            boss.DecreaseHealth(damagePerSwipe);
        }

    }

}

