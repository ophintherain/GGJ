using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultUIController : MonoBehaviour
{
    public static ResultUIController Instance { get; private set; }

    [Header("UI 内容")]
    public GameObject winSubPanel;   
    public GameObject loseSubPanel;  
    public Button restartButton;     
    public Button backButton;        
    private int currentLevelIndex; 
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowResult(int levelIndex, bool isWin)
    {
        currentLevelIndex = levelIndex;
        gameObject.SetActive(true);
        if (isWin)
        {
        LevelManager.Instance.UnlockNextLevel(levelIndex);
         }
        winSubPanel.SetActive(isWin);
        loseSubPanel.SetActive(!isWin);

        restartButton.gameObject.SetActive(!isWin); 
        backButton.gameObject.SetActive(true);

    restartButton.onClick.RemoveAllListeners();
    restartButton.onClick.AddListener(() => {
    Debug.Log("点击了重新开始按钮！"); 
    gameObject.SetActive(false);
    LevelManager.Instance.LoadLevel(currentLevelIndex);
});
       ;

    backButton.onClick.RemoveAllListeners();
    backButton.onClick.AddListener(() => {
    Debug.Log("点击了返回选关按钮！");
    gameObject.SetActive(false);
    LevelManager.Instance.BackToLevelSelect();
});
    }
}
