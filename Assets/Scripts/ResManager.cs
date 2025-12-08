using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.ResourceLoader;

public class ResManager : MonoBehaviour
{
    public static ResManager m_instance;
    
    // 资源加载器（默认使用 ResourcesLoader）
    private static IResourceLoader resourceLoader;

    public static ResManager Instance
    {
        get
        {
            if (m_instance == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    m_instance = new ResManager();
                    return m_instance;
                }
#endif
                GameObject obj = new GameObject("ResManager");
                GameObject.DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<ResManager>();

            }
            return m_instance;
        }
    }

    // 标记是否已经初始化过（防止重复初始化）
    private static bool isInitialized = false;
    
    void Awake()
    {
        // ResManager只负责提供默认加载器，不自动切换
        // 实际的加载器切换应该由GameInitializer统一控制
        if (resourceLoader == null)
        {
            // 默认使用ResourcesLoader
            resourceLoader = new ResourcesLoader();
            Debug.Log("[ResManager] 已初始化默认ResourcesLoader（等待GameInitializer配置）");
        }
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    /// <summary>
    /// 设置资源加载器（统一入口，确保唯一性）
    /// </summary>
    public static void SetResourceLoader(IResourceLoader loader)
    {
        if (loader == null)
        {
            Debug.LogWarning("[ResManager] 尝试设置null加载器，已忽略");
            return;
        }
        
        // 如果已经初始化过，记录警告
        if (isInitialized && resourceLoader != null && resourceLoader.GetType() != loader.GetType())
        {
            Debug.LogWarning($"[ResManager] 资源加载器已设置为 {resourceLoader.GetType().Name}，正在切换为 {loader.GetType().Name}");
        }
        
        resourceLoader = loader;
        isInitialized = true;
        Debug.Log($"[ResManager] ✓ 资源加载器已设置为: {loader.GetType().Name}");
    }

    /// <summary>
    /// 获取当前资源加载器
    /// </summary>
    public static IResourceLoader GetResourceLoader()
    {
        if (resourceLoader == null)
        {
            resourceLoader = new ResourcesLoader();
        }
        return resourceLoader;
    }

    /// <summary>
    /// 加载资源（兼容旧接口）
    /// </summary>
    public static Object Load(string path)
    {
        string resourcePath = GetResourcesName(path);
        return GetResourceLoader().Load<Object>(resourcePath);
    }

    /// <summary>
    /// 加载资源（泛型版本，推荐使用）
    /// </summary>
    public static T Load<T>(string path) where T : Object
    {
        string resourcePath = GetResourcesName(path);
        return GetResourceLoader().Load<T>(resourcePath);
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public static IEnumerator LoadAsync<T>(string path, System.Action<T> onComplete) where T : Object
    {
        string resourcePath = GetResourcesName(path);
        yield return GetResourceLoader().LoadAsync<T>(resourcePath, onComplete);
    }

    /// <summary>
    /// 获取资源路径（移除扩展名）
    /// </summary>
    public static string GetResourcesName(string path)
    {
        if (Path.HasExtension(path))
            path = path.Replace(Path.GetExtension(path), "");

        return path;
    }
}
