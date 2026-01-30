using System.Collections;
using UnityEngine;


public class MaskController : MonoBehaviour
{
    [Header("Timing")]
    public float appearDuration = 2f;    // 出现动画时间
    public float playableDuration = 2f;  // 可划动时间


    public bool canSwipe { get; private set; }

    private void Awake()
    {
        gameObject.SetActive(false);
        canSwipe = false;
    }

    public void StartShowMask()
    {
        StartCoroutine(FrenzyRoutine());
    }

    private IEnumerator FrenzyRoutine()
    {
        // === 出现阶段 ===
        gameObject.SetActive(true);
        canSwipe = false;

        Debug.Log("[Frenzy] Appear phase");
        // 👉 这里你可以触发 Animator
        // animator.SetTrigger("Appear");

        yield return new WaitForSeconds(appearDuration);

        // === 可划动阶段 ===
        canSwipe = true;
        Debug.Log("[Frenzy] Playable phase");

        yield return new WaitForSeconds(playableDuration);

        // === 结束 ===
        canSwipe = false;
        gameObject.SetActive(false);

        Debug.Log("[Frenzy] End");
    }
}
