using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterState
{
    Idle,    // 正常状态
    Angry,     // 破防状态
    Laugh,   // 嘲笑状态
    Lost,     // 失败状态
    // 更多状态可以根据需求添加
}

public class BossController : MonoBehaviour
{
    public Image characterImage;  // 角色的 Image 组件
    public BossHealthProgressBar healthBar; // 角色的 ProgressBar
    public Sprite[] characterStatesSprites;  // 不同状态的角色图像（按顺序设置）

    public int BossHealth;

    private CharacterState currentState = CharacterState.Idle;

    public delegate void OnBossDefeated();
    public event OnBossDefeated onBossDefeated;

    private void Start()
    {
        // 初始化角色状态和血量
        //SetCharacterState(currentState);
        healthBar.SetMaxHealth(BossHealth);  // 假设角色最大血量是100

        if (healthBar != null)
        {
            healthBar.onHealthZero.AddListener(OnHealthZero);
        }
    }

    public void SetCharacterState(CharacterState newState)
    {
        currentState = newState;

        // 更新角色的图像
        // switch (newState)
        // {
        //     case CharacterState.Idle:
        //         characterImage.sprite = characterStatesSprites[0]; 
        //         break;
        //     case CharacterState.Angry:
        //         characterImage.sprite = characterStatesSprites[1]; 
        //         break;
        //     case CharacterState.Laugh:
        //         characterImage.sprite = characterStatesSprites[2];  
        //         break;
        //         // 继续添加其他状态
        // }
    }

    // 角色血量减少
    public void DecreaseHealth(float amount)
    {
        healthBar.DecreaseHealth(amount);
    }
    private void OnHealthZero()
    {
        Debug.Log("Boss被击败！");
        //SetCharacterState(CharacterState.Lost);

        // 触发Boss被击败事件
        onBossDefeated?.Invoke();

    }

    public float GetCurrentHealth()
    {
        return healthBar.GetCurrentHealth();
    }
}
