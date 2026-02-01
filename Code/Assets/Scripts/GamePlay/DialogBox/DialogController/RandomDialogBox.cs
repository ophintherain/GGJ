using UnityEngine;

public class RandomDialogBox : DialogBoxBase
{
    public KeyCode[] sequence = new KeyCode[]
    {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
    };

    private int currentIndex = 0;
    private bool resolved = false;

    private void Awake()
    {
        // Random 属于键盘输入
        //（如果你之后用 channel 分类，这里已经语义正确）
    }

    private void Update()
    {
        HandlePlayerInput();
    }

    public override void Spawn(float spawnTime)
    {
        base.Spawn(spawnTime);

        resolved = false;
        currentIndex = 0;

        // 如果你想每次随机顺序，可以在这里打乱 sequence
        // Shuffle(sequence);
    }

    public override void HandlePlayerInput()
    {
        if (resolved) return;
        if (!isActive || !canBeJudged) return;

        // 必须轮到我
        if (!JudgeQueue.Instance.IsMyTurn(this)) return;

        KeyCode expectedKey = sequence[currentIndex];

        // 1️⃣ 正确按键
        if (Input.GetKeyDown(expectedKey))
        {
            SoundManager.Instance.PlaySFX("WASDSfx");
            currentIndex++;
            Debug.Log($"Random step {currentIndex}/{sequence.Length}");

            if (currentIndex >= sequence.Length)
            {
                resolved = true;
                Debug.Log("Random Hit!");
                ResolveHitAndDespawn();
            }

            return;
        }

        // 2️⃣ 检查“是否按了 WASD 中的其它键”
        // ❗只检查键盘，不检查鼠标
        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D))
        {
            // 但不是当前期望的那个
            resolved = true;
            Debug.Log("Random Miss (wrong WASD key)");
            ResolveMissAndDisable();
        }
    }


    public override void OnAutoMiss()
    {
        if (resolved) return;
        resolved = true;

        Debug.Log("Random Miss (timeout)");
        ResolveMissAndDisable();
    }

    // ===== 可选：打乱顺序 =====
    /*
    private void Shuffle(KeyCode[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
    */
}
