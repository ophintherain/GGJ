using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }


    [Header("Level Objects")]
    public GameObject levelSelectPanel;
    public GameObject[] levelContents;

    private const string UNLOCKED_LEVEL_KEY = "MaxUnlockedLevel";

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UnlockNextLevel(1); // 初始解锁第一关  
        UnlockNextLevel(2);
        UnlockNextLevel(3);
    }

    public int GetMaxUnlockedLevel()
    {
        return PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 1);
    }


    public void UnlockNextLevel(int currentLevelIndex)
    {
        int nextLevel = currentLevelIndex + 1;
        int currentlyUnlocked = GetMaxUnlockedLevel();

        if (nextLevel > currentlyUnlocked)
        {
            PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, nextLevel);
            PlayerPrefs.Save();
            Debug.Log($"进度已更新！现在最大解锁关卡为: {nextLevel}");
        }

    }

    public void LoadLevel(int levelIndex)
    {
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);

        for (int i = 0; i < levelContents.Length; i++)
        {
            if (levelContents[i] != null)
            {
                levelContents[i].SetActive(i == (levelIndex - 1));
            }
        }
        Debug.Log($"正在加载第 {levelIndex} 关");
    }
    public void BackToLevelSelect()
    {
        foreach (var level in levelContents)
        {
            if (level != null) level.SetActive(false);
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }

        if (LevelUIController.Instance != null)
        {
            LevelUIController.Instance.gameObject.SetActive(true);
            LevelUIController.Instance.OpenLevelSelect();
        }
        else
        {
            Debug.LogError("找不到 LevelUIController 实例！请检查场景中是否挂载了该脚本。");
        }
    }
}
