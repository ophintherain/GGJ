using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelUIController : MonoBehaviour
{
    public static LevelUIController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject levelSelectPanel;

    [Header("Level Buttons")]
    public Button[] levelButtons;

    private void Start()
    {
        ShowStartScreen();
        foreach (var button in levelButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    public void ShowStartScreen()
    {
        startPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        startPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        RefreshLevelButtons();
    }

    private void RefreshLevelButtons()
    {
        int unlockedCount = LevelManager.Instance.GetMaxUnlockedLevel();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1;
            bool isUnlocked = levelNum <= unlockedCount;
            levelButtons[i].interactable = isUnlocked;

            // 添加 Hover 事件
            AddHoverEvents(levelButtons[i]);

            // 更新按钮显示
            SetButtonState(levelButtons[i], isUnlocked);
        }
    }

    private void SetButtonState(Button button, bool isUnlocked)
    {
        Image buttonImage = button.GetComponent<Image>();
        Animator animator = button.GetComponent<Animator>();

        if (isUnlocked)
        {
            // 解锁时设置动画播放
            if (animator != null)
            {
                animator.SetBool("IsUnlocked", true); // 设置解锁状态
            }
            buttonImage.color = Color.white; // 正常颜色
        }
        else
        {
            // 未解锁时设置动画播放
            if (animator != null)
            {
                animator.SetBool("IsUnlocked", false); // 设置未解锁状态
            }

        }
    }

    // 添加 Hover 事件来控制动画暂停
    private void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => OnPointerEnter(button));

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => OnPointerExit(button));

        trigger.triggers.Add(pointerEnterEntry);
        trigger.triggers.Add(pointerExitEntry);
    }

    private void OnPointerEnter(Button button)
    {
        // 停止按钮动画播放
        Animator animator = button.GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 0; // 停止动画
        }
    }

    private void OnPointerExit(Button button)
    {
        // 恢复按钮动画播放
        Animator animator = button.GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 1; // 恢复动画
        }
    }

    // 点击时按钮下陷效果
    private void OnButtonClicked(Button button)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        Vector3 originalScale = rt.localScale;
        rt.localScale = originalScale * 0.95f; // 按下时缩小按钮

        // 延时恢复大小
        Invoke("RestoreButtonScale", 0.1f);
    }

    private void RestoreButtonScale()
    {
        foreach (var button in levelButtons)
        {
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.localScale = Vector3.one; // 恢复原始大小
        }
    }


}


