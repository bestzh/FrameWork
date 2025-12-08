using UnityEngine;
using System;
using System.Collections;
using System.IO;
using XLua;

/// <summary>
/// 热更新管理器 - 管理Lua脚本的热更新
/// </summary>
public class HotUpdateManager : MonoBehaviour
{
    private static HotUpdateManager m_instance;
    
    /// <summary>
    /// 热更新服务器地址（可以从配置读取）
    /// </summary>
    [Header("热更新设置")]
    [SerializeField] private string hotUpdateServerURL = "http://your-server.com/hotupdate/";
    
    /// <summary>
    /// 本地热更新目录（StreamingAssets或PersistentDataPath）
    /// </summary>
    [SerializeField] private string localHotUpdatePath = "";
    
    /// <summary>
    /// 是否启用热更新
    /// </summary>
    [SerializeField] private bool enableHotUpdate = true;
    
    /// <summary>
    /// 版本文件名称
    /// </summary>
    private const string VersionFileName = "lua_version.txt";
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static HotUpdateManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("HotUpdateManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<HotUpdateManager>();
            }
            return m_instance;
        }
    }
    
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
    /// 初始化
    /// </summary>
    private void Initialize()
    {
        // 确定本地热更新路径
        if (string.IsNullOrEmpty(localHotUpdatePath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            localHotUpdatePath = Application.persistentDataPath + "/LuaHotUpdate/";
#elif UNITY_IOS && !UNITY_EDITOR
            localHotUpdatePath = Application.persistentDataPath + "/LuaHotUpdate/";
#else
            localHotUpdatePath = Application.streamingAssetsPath + "/LuaHotUpdate/";
#endif
        }
        
        // 确保目录存在
        if (!Directory.Exists(localHotUpdatePath))
        {
            Directory.CreateDirectory(localHotUpdatePath);
        }
        
        Debug.Log($"热更新管理器初始化完成，本地路径: {localHotUpdatePath}");
    }
    
    /// <summary>
    /// 检查并执行热更新
    /// </summary>
    public void CheckAndUpdate(System.Action<bool> onComplete = null)
    {
        if (!enableHotUpdate)
        {
            Debug.Log("热更新已禁用");
            onComplete?.Invoke(false);
            return;
        }
        
        StartCoroutine(CheckUpdateCoroutine(onComplete));
    }
    
    /// <summary>
    /// 检查更新协程
    /// </summary>
    private IEnumerator CheckUpdateCoroutine(System.Action<bool> onComplete)
    {
        // 1. 获取本地版本
        string localVersion = GetLocalVersion();
        Debug.Log($"本地Lua版本: {localVersion}");
        
        // 2. 从服务器获取最新版本（这里需要实现HTTP请求）
        string serverVersion = "";
        bool hasUpdate = false;
        
        // TODO: 实现HTTP请求获取服务器版本
        // yield return StartCoroutine(GetServerVersion((version) => {
        //     serverVersion = version;
        //     hasUpdate = CompareVersion(serverVersion, localVersion) > 0;
        // }));
        
        // 临时：直接检查本地是否有更新文件
        hasUpdate = CheckLocalUpdateFiles();
        
        if (hasUpdate)
        {
            Debug.Log("发现Lua更新，开始下载...");
            yield return StartCoroutine(DownloadUpdateFiles(onComplete));
        }
        else
        {
            Debug.Log("Lua脚本已是最新版本");
            onComplete?.Invoke(false);
        }
    }
    
    /// <summary>
    /// 检查本地是否有更新文件
    /// </summary>
    private bool CheckLocalUpdateFiles()
    {
        // 检查StreamingAssets中是否有更新文件
        string streamingPath = Application.streamingAssetsPath + "/LuaHotUpdate/";
        if (Directory.Exists(streamingPath))
        {
            string[] files = Directory.GetFiles(streamingPath, "*.lua", SearchOption.AllDirectories);
            return files.Length > 0;
        }
        return false;
    }
    
    /// <summary>
    /// 下载更新文件
    /// </summary>
    private IEnumerator DownloadUpdateFiles(System.Action<bool> onComplete)
    {
        // TODO: 实现HTTP下载
        // 这里应该从服务器下载Lua文件到本地
        
        // 临时：从StreamingAssets复制到PersistentDataPath
        string sourcePath = Application.streamingAssetsPath + "/LuaHotUpdate/";
        string targetPath = localHotUpdatePath;
        
        if (Directory.Exists(sourcePath))
        {
            CopyDirectory(sourcePath, targetPath);
            Debug.Log("Lua文件更新完成");
            onComplete?.Invoke(true);
        }
        else
        {
            Debug.LogWarning("未找到更新文件");
            onComplete?.Invoke(false);
        }
        
        yield return null;
    }
    
    /// <summary>
    /// 获取本地版本
    /// </summary>
    private string GetLocalVersion()
    {
        string versionPath = Path.Combine(localHotUpdatePath, VersionFileName);
        if (File.Exists(versionPath))
        {
            return File.ReadAllText(versionPath).Trim();
        }
        return "1.0.0"; // 默认版本
    }
    
    /// <summary>
    /// 保存本地版本
    /// </summary>
    private void SaveLocalVersion(string version)
    {
        string versionPath = Path.Combine(localHotUpdatePath, VersionFileName);
        File.WriteAllText(versionPath, version);
    }
    
    /// <summary>
    /// 比较版本号
    /// </summary>
    private int CompareVersion(string version1, string version2)
    {
        string[] v1Parts = version1.Split('.');
        string[] v2Parts = version2.Split('.');
        
        int maxLength = Mathf.Max(v1Parts.Length, v2Parts.Length);
        for (int i = 0; i < maxLength; i++)
        {
            int v1Part = i < v1Parts.Length ? int.Parse(v1Parts[i]) : 0;
            int v2Part = i < v2Parts.Length ? int.Parse(v2Parts[i]) : 0;
            
            if (v1Part > v2Part) return 1;
            if (v1Part < v2Part) return -1;
        }
        
        return 0;
    }
    
    /// <summary>
    /// 复制目录
    /// </summary>
    private void CopyDirectory(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(targetDir, fileName);
            File.Copy(file, destFile, true);
        }
        
        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dir);
            string destDir = Path.Combine(targetDir, dirName);
            CopyDirectory(dir, destDir);
        }
    }
    
    /// <summary>
    /// 重新加载Lua脚本（热更新后调用）
    /// </summary>
    public void ReloadLuaScripts()
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            // 清除Lua环境中的已加载模块（可选）
            // luaManager.LuaEnv.DoString("package.loaded = {}");
            
            Debug.Log("Lua脚本重新加载完成");
        }
    }
}

