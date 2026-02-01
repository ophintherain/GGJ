using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManualTutorial : MonoBehaviour
{
    [Header("UI 引用")]
    public RectTransform content;
    public Button prevBtn;
    public Button nextBtn;
    public Button returnBtn;

    [Header("设置")]
    public float pageWidth = 1000f; // 每一页的宽度
    public float scrollSpeed = 0.3f; // 滑动耗时

    private int currentIndex = 0;
    private int totalPages;
    private Coroutine scrollCoroutine;

    void Start()
    {
        totalPages = content.childCount;
        
        // 绑定按钮事件
        prevBtn.onClick.AddListener(() => SwitchPage(-1));
        nextBtn.onClick.AddListener(() => SwitchPage(1));
        returnBtn.onClick.AddListener(() => gameObject.SetActive(false));

        UpdateButtons();
    }

    void SwitchPage(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, totalPages - 1);
        
        if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);
        
        // 计算目标位置：负的索引值 * 宽度
        Vector2 targetPos = new Vector2(-currentIndex * pageWidth, 0);
        scrollCoroutine = StartCoroutine(DoScroll(targetPos));
        
        UpdateButtons();
    }

    IEnumerator DoScroll(Vector2 target)
    {
        Vector2 startPos = content.anchoredPosition;
        float elapsed = 0;

        while (elapsed < scrollSpeed)
        {
            elapsed += Time.deltaTime;
            // 使用 Lerp 实现平滑过渡
            content.anchoredPosition = Vector2.Lerp(startPos, target, elapsed / scrollSpeed);
            yield return null;
        }
        content.anchoredPosition = target;
    }

    void UpdateButtons()
    {
        // 第一页不显示左箭头，最后一页不显示右箭头
        prevBtn.gameObject.SetActive(currentIndex > 0);
        nextBtn.gameObject.SetActive(currentIndex < totalPages - 1);
    }
}