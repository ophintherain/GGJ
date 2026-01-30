using UnityEngine;

public abstract class DialogBoxBase : MonoBehaviour
{
    public float spawnTime;
    public bool isActive;

    public int damageAmount = 1; // 每个音符被砍后对角色造成的伤害（血量减少的量）

    protected bool canBeJudged = false;

    // 判定点与窗口
    public float hitTime;
    public float earlyWindow = 0.3f;
    public float lateWindow = 0.3f;

    public virtual void Spawn(float spawnTime)
    {
        this.spawnTime = spawnTime;
        isActive = true;
        canBeJudged = false;

        // 默认判定点=生成时刻（之后你可以覆写或外部改）
        hitTime = spawnTime;
    }

    public void EnableJudge() => canBeJudged = true;
    public void DisableJudge() => canBeJudged = false;

    // ✅ 成功：一般是消除
    protected void ResolveHitAndDespawn()
    {
        isActive = false;
        canBeJudged = false;
        JudgeQueue.Instance.Consume(this);
        DecreaseHealth();
        Destroy(gameObject);
    }

    // ✅ 失败/超时：不消除，但要“失效 + 推进队列”
    protected void ResolveMissAndDisable()
    {
        isActive = false;
        canBeJudged = false;
        JudgeQueue.Instance.Consume(this);
        // 不 Destroy
    }

    // 队列超时会调用它
    public virtual void OnAutoMiss()
    {
        Debug.Log($"{name} AutoMiss");
        ResolveMissAndDisable();
    }
    // 玩家判定成功后掉血
    private void DecreaseHealth()
    {
        BossController bossController = FindObjectOfType<BossController>();
        if (bossController != null)
        {
            bossController.DecreaseHealth(damageAmount);
        }
    }

    public abstract void HandlePlayerInput();

    
}



