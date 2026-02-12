using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public abstract class DialogBoxBase : MonoBehaviour
{
    [Header("Success Sprite Animation")]
    [SerializeField] private Image targetImage;
    [SerializeField] private GameObject DialogueBoxDeco;
    [SerializeField] private Sprite[] successSprites;
    [SerializeField] private float frameTime = 0.02f;


    private bool isPlaying = false;

    private Coroutine playCo;

    public float spawnTime;
    public bool isActive;

    public int damageAmount = 1; // 每个音符被砍后对角色造成的伤害（血量减少的量）

    protected bool canBeJudged = false;

    // 判定点与窗口
    public float hitTime;
    public float earlyWindow = 0.3f;
    public float lateWindow = 0.3f;

    // ✅ 新增：单个音符的判定结果事件（true=Hit, false=Miss）
    public static event Action<DialogBoxBase, bool> OnResolved;

    // ✅ 防止重复上报（有些类内部也有 resolved）
    private bool reported = false;

    protected virtual void Awake()
    {
        // ✅ 初始确保 Image 子物体是隐藏的
        if (targetImage != null)
            targetImage.gameObject.SetActive(false);
    }

    public virtual void Spawn(float spawnTime)
    {
        this.spawnTime = spawnTime;
        isActive = true;
        canBeJudged = false;

        // 默认判定点=生成时刻（之后你可以覆写或外部改）
        hitTime = spawnTime;

        reported = false; // 重置
    }

    public void EnableJudge() => canBeJudged = true;
    public void DisableJudge() => canBeJudged = false;

    private void ReportResolved(bool isHit)
    {
        if (reported) return;
        reported = true;
        OnResolved?.Invoke(this, isHit);
    }

    // ✅ 成功：一般是消除
    protected void ResolveHitAndDespawn()
    {

        ReportResolved(true);
        isActive = false;
        canBeJudged = false;
        JudgeQueue.Instance.Consume(this);
        DecreaseHealth();
        OnSuccess();
        //Destroy(gameObject);
    }

    // ✅ 失败/超时：不消除，但要“失效 + 推进队列”
    protected void ResolveMissAndDisable()
    {
        ReportResolved(false);
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

    public void OnSuccess()
    {
        if (!isPlaying)
            StartCoroutine(SuccessFlow());
    }

    private IEnumerator SuccessFlow()
    {
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        DialogueBoxDeco.SetActive(false);
        targetImage.gameObject.SetActive(true);
        isPlaying = true;

        yield return StartCoroutine(PlaySpritesCo());

        // 播完再销毁
        Destroy(gameObject);
    }

    private IEnumerator PlaySpritesCo()
    {
        if (targetImage == null || successSprites == null || successSprites.Length == 0)
            yield break;

        targetImage.enabled = true;

        for (int i = 0; i < successSprites.Length; i++)
        {
            targetImage.sprite = successSprites[i];
            yield return new WaitForSeconds(frameTime);
        }

        targetImage.sprite = null;
        targetImage.enabled = false;
    }

}



