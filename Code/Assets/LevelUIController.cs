using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
        if (levelNum <= unlockedCount)
        {
            levelButtons[i].interactable = true;
            levelButtons[i].image.color = Color.white; 
        }
        else
        {
            levelButtons[i].interactable = false;
            levelButtons[i].image.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
        }
    }
    }
}

