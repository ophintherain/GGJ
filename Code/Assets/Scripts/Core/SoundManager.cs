using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Background Music (BGM)")]
    public AudioSource bgmAudioSource;  // 用于播放背景音乐的音频源
    public AudioClip defaultBgmClip;    // 默认背景音乐
    private AudioClip currentBgmClip;   // 当前播放的背景音乐

    [Header("Sound Effects (SFX)")]
    public AudioSource sfxAudioSource;  // 用于播放音效的音频源
    private Dictionary<string, AudioClip> sfxClips; // 用字典来管理音效资源，避免重复加载

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f; // 背景音乐音量
    [Range(0f, 1f)] public float sfxVolume = 1f; // 音效音量

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 如果已有实例就销毁重复的
        }

        // 初始化音效字典
        sfxClips = new Dictionary<string, AudioClip>();


    }

    // 播放背景音乐
    public void PlayBGM(string bgmName)
    {
        AudioClip clip = AudioResourceManager.Instance.GetBGM(bgmName);
        if (bgmAudioSource == null || clip == null) return;


        // 如果当前正在播放的就是该音乐，不做任何操作
        if (currentBgmClip == clip)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
        currentBgmClip = clip;
        bgmAudioSource.volume = bgmVolume; // 调整音量
    }

    // 暂停背景音乐
    public void PauseBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    // 恢复背景音乐
    public void ResumeBGM()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    // 停止背景音乐
    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            currentBgmClip = null;
        }
    }

    // 播放音效（SFX）
    public void PlaySFX(string sfxName)
    {
        if (sfxAudioSource == null || !sfxClips.ContainsKey(sfxName)) return;

        sfxAudioSource.PlayOneShot(sfxClips[sfxName], sfxVolume);  // 播放音效并应用音量
    }

    // 加载音效（一次性）
    public void LoadSFX(string sfxName, AudioClip clip)
    {
        if (sfxClips.ContainsKey(sfxName)) return;

        sfxClips.Add(sfxName, clip);
    }

    // 清理音效资源
    public void UnloadSFX(string sfxName)
    {
        if (sfxClips.ContainsKey(sfxName))
        {
            sfxClips.Remove(sfxName);
        }
    }

    // 设置背景音乐音量
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmAudioSource.volume = volume;
    }

    // 设置音效音量
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}

