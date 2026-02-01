// 简化的 SoundManager.cs
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Background Music (BGM)")]
    public AudioSource bgmAudioSource;
    
    [Header("Sound Effects (SFX)")]
    public AudioSource sfxAudioSource;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioClip currentBgmClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 播放背景音乐
    public void PlayBGM(string bgmName)
    {
        if (bgmAudioSource == null) return;
        
        AudioClip clip = AudioResourceManager.Instance.GetBGM(bgmName);
        if (clip == null) return;

        // 如果当前正在播放的就是该音乐，不做任何操作
        if (currentBgmClip == clip && bgmAudioSource.isPlaying)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.Play();
        currentBgmClip = clip;
    }

    // 播放音效
    public void PlaySFX(string sfxName)
    {
        if (sfxAudioSource == null) return;
        
        AudioClip clip = AudioResourceManager.Instance.GetSFX(sfxName);
        if (clip == null)
        {
            Debug.LogWarning($"SFX '{sfxName}' not found!");
            return;
        }

        sfxAudioSource.PlayOneShot(clip, sfxVolume);
    }

    // 其他方法保持不变...
    public void PauseBGM() { /* ... */ }
    public void ResumeBGM() { /* ... */ }
    public void StopBGM() { /* ... */ }
    public void SetBGMVolume(float volume) { /* ... */ }
    public void SetSFXVolume(float volume) { /* ... */ }
}