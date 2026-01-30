using System;
using UnityEngine;

public class EventCounter : MonoBehaviour
{
    public event Action<string, int> OnMark;

    private int demoIndex = 0;
    private int playIndex = 0;

    public void ResetCounters()
    {
        demoIndex = 0;
        playIndex = 0;
    }

    public void MarkDemo()
    {
        demoIndex++;
        Debug.Log($"demo{demoIndex}");
        OnMark?.Invoke("demo", demoIndex);
    }

    public void MarkPlay()
    {
        playIndex++;
        Debug.Log($"play{playIndex}");
        OnMark?.Invoke("play", playIndex);
    }
}

