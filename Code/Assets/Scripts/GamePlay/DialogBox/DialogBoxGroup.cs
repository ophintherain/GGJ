using System.Collections;
using UnityEngine;

public class DialogBoxGroup : MonoBehaviour
{
    private TapDialogBox[] taps;

    private void Awake()
    {
        taps = GetComponentsInChildren<TapDialogBox>(true);

        // 一开始全部隐藏
        foreach (var tap in taps)
        {
            tap.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 每隔 beatInterval 依次生成 Tap
    /// </summary>
    public IEnumerator SpawnByBeat(float beatInterval)
    {
        for (int i = 0; i < taps.Length; i++)
        {
            TapDialogBox tap = taps[i];
            tap.gameObject.SetActive(true);
            tap.Spawn(Time.time);

            yield return new WaitForSeconds(beatInterval);
        }
    }

    /// <summary>
    /// 进入游玩阶段，允许点击消除
    /// </summary>
    public void EnableJudgeAll()
    {
        foreach (var tap in taps)
        {
            tap.EnableJudge();
        }
    }
}

