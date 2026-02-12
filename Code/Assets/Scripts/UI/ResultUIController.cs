using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultUIController : SingletonPersistent<ResultUIController>
{
    [Header("UI 面板")]
    public GameObject winSubPanel;
    public GameObject loseSubPanel;
    
    [Header("按钮")]
    public Button restartButton;
    public Button backButton;

    [Header("动画设置")]
    public float fadeInDuration = 0.5f;
    public float scaleDuration = 0.3f;
    public Ease showEase = Ease.OutBack;
    
    private int currentLevelIndex;
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;

    protected override void Awake()
    {
        base.Awake();
        
        // 获取组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        panelRect = GetComponent<RectTransform>();
        
        // 初始状态
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        // 初始化按钮事件
        InitializeButtons();
    }
    
    private void InitializeButtons()
    {
        // 重新开始按钮
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        // 返回选关按钮
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }
    }
    
    public void ShowResult(int levelIndex, bool isWin)
    {
        currentLevelIndex = levelIndex;
        
        // 从BeatManager获取判定数据
        BeatManager beatManager = FindObjectOfType<BeatManager>();
        EventCounter eventCounter = beatManager?.eventCounter;
        
        // 设置胜负面板
        winSubPanel.SetActive(isWin);
        loseSubPanel.SetActive(!isWin);
        
        // 按钮显示逻辑
        restartButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        
        // 播放进入动画
        PlayShowAnimation();
    }
    
    private void PlayShowAnimation()
    {
        gameObject.SetActive(true);
        
        // 重置状态
        canvasGroup.alpha = 0f;
        if (panelRect != null)
        {
            panelRect.localScale = Vector3.zero;
        }
        
        // DOTween动画序列
        Sequence showSequence = DOTween.Sequence();
        
        // 淡入
        showSequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
        
        // 缩放弹入
        if (panelRect != null)
        {
            showSequence.Join(panelRect.DOScale(1f, scaleDuration)
                .SetEase(showEase));
        }
        
        // 播放
        showSequence.Play();
    }
    
    private void OnRestartClicked()
    {
        Debug.Log("重新开始关卡: " + currentLevelIndex);
        
        HideUI();

        LevelManager.Instance?.LoadLevel(currentLevelIndex);
    }

    private void OnBackClicked()
    {
        Debug.Log("返回选关界面");
        
        HideUI();
        // 返回选关界面
        LevelManager.Instance.BackToLevelSelect();
    }
    
    private void HideUI()
    {
        // 淡出动画
        canvasGroup.DOFade(0f, 0.2f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}