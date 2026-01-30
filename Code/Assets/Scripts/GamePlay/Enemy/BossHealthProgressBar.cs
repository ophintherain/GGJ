using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class BossHealthProgressBar : MonoBehaviour
{
    public Slider slider; // 使用 Unity 的 Slider 来作为进度条

    private void Start()
    {
        slider.maxValue = 100;  // 假设最大血量是 100
        slider.value = 100;      // 初始化血量为最大值
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    public void DecreaseHealth(float amount)
    {
        slider.value -= amount;
    }
}


