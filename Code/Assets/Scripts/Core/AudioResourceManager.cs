using UnityEngine;
using System.Collections.Generic;

public class AudioResourceManager : MonoBehaviour
{
    public static AudioResourceManager Instance { get; private set; }

    [Header("Background Music (BGM)")]
    public AudioClip[] bgmClips; // 所有背景音乐
    private Dictionary<string, AudioClip> bgmDict; // 用字典管理背景音乐资源

    [Header("Sound Effects (SFX)")]
    public AudioClip[] sfxClips; // 所有音效
    private Dictionary<string, AudioClip> sfxDict; // 用字典管理音效资源

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化字典
        bgmDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();

        // 加载背景音乐
        foreach (var clip in bgmClips)
        {
            bgmDict[clip.name] = clip;
        }

        // 加载音效
        foreach (var clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }
    }

    // 获取背景音乐（根据名称）
    public AudioClip GetBGM(string bgmName)
    {
        if (bgmDict.ContainsKey(bgmName))
        {
            return bgmDict[bgmName];
        }
        else
        {
            Debug.LogWarning($"BGM '{bgmName}' not found!");
            return null;
        }
    }

    // 获取音效（根据名称）
    public AudioClip GetSFX(string sfxName)
    {
        if (sfxDict.ContainsKey(sfxName))
        {
            return sfxDict[sfxName];
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' not found!");
            return null;
        }
    }
}

