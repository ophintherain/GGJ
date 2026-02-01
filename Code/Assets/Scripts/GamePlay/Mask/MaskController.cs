using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskController : MonoBehaviour
{
    [Header("Preplaced masks (children)")]
    public List<GameObject> masks = new List<GameObject>();

    [Header("Movement Bounds (BG)")]
    public RectTransform boundsBG;     // ✅ 这是“边界对象”，最终给 movement 用

    [Header("Movement")]
    public float moveSpeed = 200f;

    [Header("Idle Spawn")]
    public float spawnInterval = 0.2f;

    private Coroutine spawnCo;
    private bool spawning;
    private int idx;

    private void Awake()
    {
        // 自动收集子物体（你预先摆一堆 mask 的玩法很适合）
        if (masks.Count == 0)
        {
            foreach (Transform t in transform)
                masks.Add(t.gameObject);
        }


    }
    private void Start()
    {

        // 初始全部隐藏 + 注入 movement 的 bounds
        foreach (var m in masks)
        {
            if (!m) continue;
            m.SetActive(false);

            var mv = m.GetComponent<MaskMovement>();
            if (mv != null)
            {
                mv.Setup(boundsBG, moveSpeed);
                Debug.Log($"Setup mask: {m.name}");
            }
            else
            {
                Debug.LogError($"No MaskMovement component found on {m.name}");
            }
        }
    }

    public void StartIdlePhase()
    {
        spawning = true;
        idx = 0;

        if (spawnCo != null) StopCoroutine(spawnCo);
        spawnCo = StartCoroutine(SpawnRoutine());
    }

    public void StartFrenzyPhase()
    {
        // 燃阶段不再激活新的
        spawning = false;
        if (spawnCo != null) StopCoroutine(spawnCo);
        spawnCo = null;
    }

    public void StopAndHideAll()
    {
        spawning = false;
        if (spawnCo != null) StopCoroutine(spawnCo);
        spawnCo = null;

        foreach (var m in masks)
            if (m) m.SetActive(false);
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawning && idx < masks.Count)
        {
            var m = masks[idx];
            if (m) m.SetActive(true);  // ✅ 激活后 Movement 会开始动（OnEnable）
            idx++;

            yield return new WaitForSeconds(spawnInterval);
        }

        spawnCo = null;
    }
    
    public void DestroyAllMasks()
    {
        spawning = false;
        if (spawnCo != null)
        {
            StopCoroutine(spawnCo);
            spawnCo = null;
        }

        // 销毁所有mask子物体
        foreach (var m in masks)
        {
            if (m) Destroy(m);
        }

        // 清空列表
        masks.Clear();

        Debug.Log("All masks destroyed.");
    }
}
