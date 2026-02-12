using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex = -1;

    [Header("Level Objects")]
    public GameObject levelSelectPanel;
    public GameObject[] levelPrefabs; // 改为存储关卡Prefab数组
    public Transform levelContainer; // 新增：用于放置实例化关卡的父节点

    private GameObject currentLevelInstance; // 当前实例化的关卡对象
    private const string UNLOCKED_LEVEL_KEY = "MaxUnlockedLevel";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：如果希望跨场景保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UnlockNextLevel(1); // 初始解锁第一关  
        UnlockNextLevel(2);
        UnlockNextLevel(3);
    }



    // 获取当前关卡索引
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
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
        if (levelIndex < 1 || levelIndex > levelPrefabs.Length)
        {
            Debug.LogError($"无效的关卡索引: {levelIndex}");
            return;
        }

        // 隐藏关卡选择面板
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);

        // 清理当前关卡实例（如果有）
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        // 实例化新关卡
        GameObject levelPrefab = levelPrefabs[levelIndex - 1];
        if (levelPrefab == null)
        {
            Debug.LogError($"第 {levelIndex} 关的Prefab为空！");
            return;
        }

        // 实例化关卡
        if (levelContainer == null)
        {
            // 如果没有指定容器，就实例化在LevelManager对象下
            currentLevelInstance = Instantiate(levelPrefab, transform);
        }
        else
        {
            currentLevelInstance = Instantiate(levelPrefab, levelContainer);
        }
        currentLevelIndex = levelIndex;



        Debug.Log($"正在加载第 {levelIndex} 关 (实例化Prefab)");
    }

    public void BackToLevelSelect()
    {
        // 清理当前关卡实例
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }

        // 显示关卡选择面板
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }

        // 激活关卡选择UI控制器
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

    // 新增：重新开始当前关卡
    public void RestartCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            // 获取当前关卡索引（通过查找当前关卡实例对应哪个Prefab）
            // 这个方法需要你知道当前是哪个关卡
            // 或者你可以在实例化时记录当前关卡索引
            Debug.Log("重新开始当前关卡功能需要额外实现");
        }
    }

    // 新增：卸载当前关卡
    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
            Debug.Log("当前关卡已卸载");
        }
    }
}
