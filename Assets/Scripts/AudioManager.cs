using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.ResourceLoader;

/// <summary>
/// 音频管理器 - 统一管理背景音乐和音效
/// 支持音量控制、音频池、淡入淡出等功能
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager m_instance;
    
    [Header("音频设置")]
    [SerializeField] private float musicVolume = 1.0f;
    [SerializeField] private float soundVolume = 1.0f;
    [SerializeField] private bool musicMuted = false;
    [SerializeField] private bool soundMuted = false;
    
    [Header("音频池设置")]
    [SerializeField] private int maxSoundPoolSize = 20; // 音效对象池最大数量
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static AudioManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("AudioManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<AudioManager>();
            }
            return m_instance;
        }
    }
    
    // 背景音乐相关
    private AudioSource musicSource;
    private string currentMusicName = "";
    private AudioClip currentMusicClip = null;
    
    // 音效相关（使用通用对象池）
    private List<AudioSource> activeSounds = new List<AudioSource>();
    private Dictionary<string, AudioClip> soundClipCache = new Dictionary<string, AudioClip>();
    private GameObject soundSourcePrefab;  // AudioSource预制体（动态创建）
    private const string SOUND_POOL_NAME = "AudioSource";
    
    // 资源加载器
    private IResourceLoader resourceLoader;
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化音频管理器
    /// </summary>
    private void Initialize()
    {
        // 创建背景音乐AudioSource
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        
        // 创建AudioSource预制体用于对象池
        soundSourcePrefab = new GameObject("SoundSourcePrefab");
        soundSourcePrefab.AddComponent<AudioSource>();
        soundSourcePrefab.SetActive(false);
        DontDestroyOnLoad(soundSourcePrefab);
        
        // 创建音效对象池（使用通用对象池）
        ObjectPool.Instance.CreatePool(SOUND_POOL_NAME, soundSourcePrefab, maxSoundPoolSize);
        
        // 初始化资源加载器
        resourceLoader = ResManager.GetResourceLoader();
        
        // 从PlayerPrefs加载音量设置
        LoadAudioSettings();
        
        Debug.Log("[AudioManager] 音频管理器初始化完成（已使用通用对象池）");
    }
    
    #region 背景音乐管理
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clipName">音频资源名称（路径）</param>
    /// <param name="loop">是否循环</param>
    /// <param name="fadeIn">是否淡入</param>
    /// <param name="fadeTime">淡入时间（秒）</param>
    public void PlayMusic(string clipName, bool loop = true, bool fadeIn = false, float fadeTime = 1.0f)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning("[AudioManager] 音乐名称不能为空");
            return;
        }
        
        // 如果正在播放同一首音乐，直接返回
        if (currentMusicName == clipName && musicSource.isPlaying)
        {
            Debug.Log($"[AudioManager] 音乐已在播放: {clipName}");
            return;
        }
        
        // 加载音频资源
        AudioClip clip = LoadAudioClip(clipName);
        if (clip == null)
        {
            Debug.LogError($"[AudioManager] 无法加载音乐: {clipName}");
            return;
        }
        
        // 停止当前音乐
        StopMusic(fadeIn, fadeTime);
        
        // 设置新音乐
        currentMusicName = clipName;
        currentMusicClip = clip;
        musicSource.clip = clip;
        musicSource.loop = loop;
        
        // 播放音乐
        if (fadeIn)
        {
            StartCoroutine(PlayMusicWithFadeIn(fadeTime));
        }
        else
        {
            musicSource.volume = musicMuted ? 0 : musicVolume;
            musicSource.Play();
            Debug.Log($"[AudioManager] ✓ 开始播放音乐: {clipName}");
        }
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    /// <param name="fadeOut">是否淡出</param>
    /// <param name="fadeTime">淡出时间（秒）</param>
    public void StopMusic(bool fadeOut = false, float fadeTime = 1.0f)
    {
        if (musicSource.isPlaying)
        {
            if (fadeOut)
            {
                StartCoroutine(StopMusicWithFadeOut(fadeTime));
            }
            else
            {
                musicSource.Stop();
                Debug.Log("[AudioManager] 音乐已停止");
            }
        }
        
        currentMusicName = "";
        currentMusicClip = null;
    }
    
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            Debug.Log("[AudioManager] 音乐已暂停");
        }
    }
    
    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource.clip != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
            Debug.Log("[AudioManager] 音乐已恢复");
        }
    }
    
    /// <summary>
    /// 切换背景音乐（淡出旧音乐，淡入新音乐）
    /// </summary>
    public void SwitchMusic(string newClipName, bool loop = true, float fadeTime = 1.0f)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(SwitchMusicCoroutine(newClipName, loop, fadeTime));
        }
        else
        {
            PlayMusic(newClipName, loop, true, fadeTime);
        }
    }
    
    /// <summary>
    /// 切换音乐协程
    /// </summary>
    private IEnumerator SwitchMusicCoroutine(string newClipName, bool loop, float fadeTime)
    {
        // 淡出当前音乐
        yield return StartCoroutine(StopMusicWithFadeOut(fadeTime));
        
        // 淡入新音乐
        PlayMusic(newClipName, loop, true, fadeTime);
    }
    
    /// <summary>
    /// 淡入播放音乐
    /// </summary>
    private IEnumerator PlayMusicWithFadeIn(float fadeTime)
    {
        musicSource.volume = 0;
        musicSource.Play();
        
        float elapsed = 0;
        float targetVolume = musicMuted ? 0 : musicVolume;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, targetVolume, elapsed / fadeTime);
            yield return null;
        }
        
        musicSource.volume = targetVolume;
        Debug.Log($"[AudioManager] ✓ 音乐淡入完成: {currentMusicName}");
    }
    
    /// <summary>
    /// 淡出停止音乐
    /// </summary>
    private IEnumerator StopMusicWithFadeOut(float fadeTime)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / fadeTime);
            yield return null;
        }
        
        musicSource.Stop();
        musicSource.volume = startVolume; // 恢复音量设置
        Debug.Log("[AudioManager] 音乐淡出完成");
    }
    
    #endregion
    
    #region 音效管理
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clipName">音频资源名称（路径）</param>
    /// <param name="volume">音量（0-1），如果为-1则使用全局音效音量</param>
    /// <param name="pitch">音调（1.0为正常）</param>
    public void PlaySound(string clipName, float volume = -1, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning("[AudioManager] 音效名称不能为空");
            return;
        }
        
        if (soundMuted)
        {
            return; // 音效已静音，直接返回
        }
        
        // 加载音频资源
        AudioClip clip = LoadAudioClip(clipName);
        if (clip == null)
        {
            Debug.LogError($"[AudioManager] 无法加载音效: {clipName}");
            return;
        }
        
        // 从对象池获取或创建AudioSource
        AudioSource soundSource = GetSoundSource();
        if (soundSource == null)
        {
            Debug.LogWarning("[AudioManager] 无法获取音效AudioSource（对象池已满）");
            return;
        }
        
        // 设置音频
        soundSource.clip = clip;
        soundSource.volume = volume >= 0 ? volume : soundVolume;
        soundSource.pitch = pitch;
        soundSource.loop = false;
        soundSource.Play();
        
        // 添加到活动列表
        activeSounds.Add(soundSource);
        
        // 播放完成后自动回收
        StartCoroutine(ReturnSoundSourceToPool(soundSource, clip.length / pitch));
    }
    
    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        foreach (var soundSource in activeSounds)
        {
            if (soundSource != null && soundSource.isPlaying)
            {
                soundSource.Stop();
            }
            ReturnSoundSourceToPool(soundSource, 0);
        }
        activeSounds.Clear();
        Debug.Log("[AudioManager] 所有音效已停止");
    }
    
    /// <summary>
    /// 停止指定音效（通过clipName）
    /// </summary>
    public void StopSound(string clipName)
    {
        for (int i = activeSounds.Count - 1; i >= 0; i--)
        {
            var soundSource = activeSounds[i];
            if (soundSource != null && soundSource.clip != null && soundSource.clip.name == clipName)
            {
                soundSource.Stop();
                ReturnSoundSourceToPool(soundSource, 0);
                activeSounds.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 从对象池获取AudioSource（使用通用对象池）
    /// </summary>
    private AudioSource GetSoundSource()
    {
        // 从通用对象池获取
        GameObject soundObj = ObjectPool.Instance.Get(SOUND_POOL_NAME, soundSourcePrefab);
        if (soundObj == null)
        {
            return null;
        }
        
        // 确保AudioSource组件存在
        AudioSource source = soundObj.GetComponent<AudioSource>();
        if (source == null)
        {
            source = soundObj.AddComponent<AudioSource>();
        }
        
        // 设置父节点和基本属性
        soundObj.transform.SetParent(transform);
        source.playOnAwake = false;
        
        return source;
    }
    
    /// <summary>
    /// 将AudioSource回收到对象池（使用通用对象池）
    /// </summary>
    private IEnumerator ReturnSoundSourceToPool(AudioSource source, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        if (source != null && source.gameObject != null)
        {
            source.Stop();
            source.clip = null;
            
            // 从活动列表中移除
            activeSounds.Remove(source);
            
            // 回收到通用对象池
            ObjectPool.Instance.Release(SOUND_POOL_NAME, source.gameObject);
        }
    }
    
    #endregion
    
    #region 音量控制
    
    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="volume">音量（0-1）</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null && !musicMuted)
        {
            musicSource.volume = musicVolume;
        }
        SaveAudioSettings();
        Debug.Log($"[AudioManager] 音乐音量已设置为: {musicVolume}");
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量（0-1）</param>
    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        
        // 更新所有活动音效的音量
        foreach (var soundSource in activeSounds)
        {
            if (soundSource != null && !soundMuted)
            {
                soundSource.volume = soundVolume;
            }
        }
        
        SaveAudioSettings();
        Debug.Log($"[AudioManager] 音效音量已设置为: {soundVolume}");
    }
    
    /// <summary>
    /// 获取背景音乐音量
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    
    /// <summary>
    /// 获取音效音量
    /// </summary>
    public float GetSoundVolume()
    {
        return soundVolume;
    }
    
    /// <summary>
    /// 静音/取消静音背景音乐
    /// </summary>
    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;
        if (musicSource != null)
        {
            musicSource.volume = muted ? 0 : musicVolume;
        }
        SaveAudioSettings();
        Debug.Log($"[AudioManager] 音乐静音: {muted}");
    }
    
    /// <summary>
    /// 静音/取消静音音效
    /// </summary>
    public void SetSoundMuted(bool muted)
    {
        soundMuted = muted;
        
        // 更新所有活动音效
        foreach (var soundSource in activeSounds)
        {
            if (soundSource != null)
            {
                soundSource.volume = muted ? 0 : soundVolume;
            }
        }
        
        SaveAudioSettings();
        Debug.Log($"[AudioManager] 音效静音: {muted}");
    }
    
    /// <summary>
    /// 检查音乐是否静音
    /// </summary>
    public bool IsMusicMuted()
    {
        return musicMuted;
    }
    
    /// <summary>
    /// 检查音效是否静音
    /// </summary>
    public bool IsSoundMuted()
    {
        return soundMuted;
    }
    
    #endregion
    
    #region 资源加载
    
    /// <summary>
    /// 加载音频资源（带缓存）
    /// </summary>
    private AudioClip LoadAudioClip(string clipName)
    {
        // 检查缓存
        if (soundClipCache.TryGetValue(clipName, out AudioClip cachedClip))
        {
            return cachedClip;
        }
        
        // 从资源加载器加载
        if (resourceLoader == null)
        {
            resourceLoader = ResManager.GetResourceLoader();
        }
        
        AudioClip clip = resourceLoader.Load<AudioClip>(clipName);
        if (clip != null)
        {
            soundClipCache[clipName] = clip;
            Debug.Log($"[AudioManager] ✓ 已加载音频: {clipName}");
        }
        else
        {
            Debug.LogError($"[AudioManager] ✗ 无法加载音频: {clipName}");
        }
        
        return clip;
    }
    
    /// <summary>
    /// 预加载音频资源
    /// </summary>
    public void PreloadAudio(string clipName, System.Action<bool> onComplete = null)
    {
        if (soundClipCache.ContainsKey(clipName))
        {
            onComplete?.Invoke(true);
            return;
        }
        
        StartCoroutine(PreloadAudioCoroutine(clipName, onComplete));
    }
    
    /// <summary>
    /// 预加载音频协程
    /// </summary>
    private IEnumerator PreloadAudioCoroutine(string clipName, System.Action<bool> onComplete)
    {
        if (resourceLoader == null)
        {
            resourceLoader = ResManager.GetResourceLoader();
        }
        
        AudioClip clip = null;
        yield return resourceLoader.LoadAsync<AudioClip>(clipName, (loadedClip) =>
        {
            clip = loadedClip;
        });
        
        if (clip != null)
        {
            soundClipCache[clipName] = clip;
            Debug.Log($"[AudioManager] ✓ 预加载音频完成: {clipName}");
            onComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError($"[AudioManager] ✗ 预加载音频失败: {clipName}");
            onComplete?.Invoke(false);
        }
    }
    
    /// <summary>
    /// 卸载音频资源（从缓存中移除）
    /// </summary>
    public void UnloadAudio(string clipName)
    {
        if (soundClipCache.ContainsKey(clipName))
        {
            soundClipCache.Remove(clipName);
            Debug.Log($"[AudioManager] 已卸载音频: {clipName}");
        }
    }
    
    /// <summary>
    /// 清空音频缓存
    /// </summary>
    public void ClearAudioCache()
    {
        soundClipCache.Clear();
        Debug.Log("[AudioManager] 音频缓存已清空");
    }
    
    #endregion
    
    #region 设置保存/加载
    
    /// <summary>
    /// 保存音频设置到PlayerPrefs
    /// </summary>
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("AudioManager_MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("AudioManager_SoundVolume", soundVolume);
        PlayerPrefs.SetInt("AudioManager_MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.SetInt("AudioManager_SoundMuted", soundMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 从PlayerPrefs加载音频设置
    /// </summary>
    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("AudioManager_MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("AudioManager_MusicVolume", 1.0f);
        }
        if (PlayerPrefs.HasKey("AudioManager_SoundVolume"))
        {
            soundVolume = PlayerPrefs.GetFloat("AudioManager_SoundVolume", 1.0f);
        }
        if (PlayerPrefs.HasKey("AudioManager_MusicMuted"))
        {
            musicMuted = PlayerPrefs.GetInt("AudioManager_MusicMuted", 0) == 1;
        }
        if (PlayerPrefs.HasKey("AudioManager_SoundMuted"))
        {
            soundMuted = PlayerPrefs.GetInt("AudioManager_SoundMuted", 0) == 1;
        }
        
        // 应用设置
        if (musicSource != null)
        {
            musicSource.volume = musicMuted ? 0 : musicVolume;
        }
        
        Debug.Log($"[AudioManager] 音频设置已加载 - 音乐: {musicVolume} (静音: {musicMuted}), 音效: {soundVolume} (静音: {soundMuted})");
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 检查音乐是否正在播放
    /// </summary>
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }
    
    /// <summary>
    /// 获取当前播放的音乐名称
    /// </summary>
    public string GetCurrentMusicName()
    {
        return currentMusicName;
    }
    
    /// <summary>
    /// 获取活动音效数量
    /// </summary>
    public int GetActiveSoundCount()
    {
        return activeSounds.Count;
    }
    
    #endregion
    
    void OnDestroy()
    {
        if (m_instance == this)
        {
            // 停止所有音频
            StopMusic();
            StopAllSounds();
            
            // 清理缓存
            ClearAudioCache();
            
            m_instance = null;
        }
    }
}

