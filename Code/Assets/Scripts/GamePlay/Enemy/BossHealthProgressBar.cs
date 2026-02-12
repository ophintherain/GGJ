using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UnityEngine.UI;

public class BossHealthProgressBar : MonoBehaviour
{
    public Slider slider; // 使用 Unity 的 Slider 来作为进度条

    [Header("血量事件")]
    public UnityEvent onHealthZero; // 血量归零时触发的事件

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
        float newHealth = slider.value - amount;
        if (newHealth < 0) newHealth = 0;

        slider.value = newHealth;

        // 检查血量是否归零
        if (slider.value <= 0)
        {
            Debug.Log("Boss血量归零！");
            onHealthZero?.Invoke();
        }
    }

    // 获取当前血量
    public float GetCurrentHealth()
    {
        return slider.value;
    }
}


