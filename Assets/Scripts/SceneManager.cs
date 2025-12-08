using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Reflection;
using Framework.ResourceLoader;

/// <summary>
/// 场景管理器 - 统一管理场景加载和切换
/// 支持两种加载方式：
/// 1. Build Settings方式（场景必须在Build Settings中）
/// 2. Addressables方式（场景标记为Addressable）
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager m_instance;
    
    /// <summary>
    /// 当前加载的场景名称
    /// </summary>
    private string currentSceneName;
    
    /// <summary>
    /// 是否正在加载场景
    /// </summary>
    private bool isLoading = false;
    
    /// <summary>
    /// 是否使用Addressables加载场景
    /// </summary>
    private bool useAddressables = false;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static GameSceneManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("GameSceneManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<GameSceneManager>();
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
            currentSceneName = SceneManager.GetActiveScene().name;
            
            // 检查是否使用Addressables
            var loader = ResManager.GetResourceLoader();
            useAddressables = loader != null && loader.GetType().Name == "AddressablesLoader";
            
            Debug.Log($"[GameSceneManager] 场景管理器初始化完成，使用方式: {(useAddressables ? "Addressables" : "Build Settings")}");
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 同步加载场景
    /// 注意：场景必须满足以下条件之一：
    /// 1. 在Build Settings的Scenes In Build列表中（使用Build Settings方式）
    /// 2. 标记为Addressable资源（使用Addressables方式）
    /// </summary>
    /// <param name="sceneName">场景名称或Addressable地址</param>
    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning($"[GameSceneManager] 场景正在加载中，无法加载新场景: {sceneName}");
            return;
        }
        
        try
        {
            isLoading = true;
            Debug.Log($"[GameSceneManager] 开始加载场景: {sceneName} (方式: {(useAddressables ? "Addressables" : "Build Settings")})");
            
            if (useAddressables)
            {
                // 使用Addressables加载场景（异步，但等待完成）
                StartCoroutine(LoadSceneWithAddressablesSync(sceneName));
            }
            else
            {
                // 使用Build Settings方式加载场景
                SceneManager.LoadScene(sceneName);
                currentSceneName = sceneName;
                isLoading = false;
                Debug.Log($"[GameSceneManager] ✓ 场景加载完成: {sceneName}");
            }
        }
        catch (Exception e)
        {
            isLoading = false;
            Debug.LogError($"[GameSceneManager] 加载场景失败: {sceneName}\n{e.Message}");
        }
    }
    
    /// <summary>
    /// 使用Addressables同步加载场景（内部协程）
    /// </summary>
    private IEnumerator LoadSceneWithAddressablesSync(string sceneName)
    {
        // 尝试使用Addressables加载场景
        var loader = ResManager.GetResourceLoader() as AddressablesLoader;
        bool useAddressablesSuccess = false;
        
        if (loader != null)
        {
            // 检查Addressables是否可用（通过反射）
            Type addressablesType = Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables");
            if (addressablesType != null)
            {
                try
                {
                    // 使用反射调用Addressables.LoadSceneAsync
                    MethodInfo loadSceneMethod = addressablesType.GetMethod("LoadSceneAsync", 
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[] { typeof(string), typeof(LoadSceneMode) },
                        null);
                    
                    if (loadSceneMethod != null)
                    {
                        object handle = loadSceneMethod.Invoke(null, new object[] { sceneName, LoadSceneMode.Single });
                        if (handle != null)
                        {
                            // 等待场景加载完成
                            Type handleType = handle.GetType();
                            MethodInfo waitMethod = handleType.GetMethod("WaitForCompletion");
                            if (waitMethod != null)
                            {
                                waitMethod.Invoke(handle, null);
                                
                                // 检查状态
                                PropertyInfo statusProp = handleType.GetProperty("Status");
                                if (statusProp != null)
                                {
                                    object status = statusProp.GetValue(handle);
                                    Type statusType = Type.GetType("UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus, Unity.ResourceManager");
                                    if (statusType != null)
                                    {
                                        object succeededStatus = Enum.Parse(statusType, "Succeeded");
                                        if (status.Equals(succeededStatus))
                                        {
                                            currentSceneName = sceneName;
                                            Debug.Log($"[GameSceneManager] ✓ 场景加载完成: {sceneName} (Addressables)");
                                            useAddressablesSuccess = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[GameSceneManager] Addressables加载场景失败，回退到Build Settings方式: {e.Message}");
                }
            }
        }
        
        // 如果Addressables加载失败，回退到Build Settings方式
        if (!useAddressablesSuccess)
        {
            Debug.LogWarning($"[GameSceneManager] Addressables不可用，回退到Build Settings方式加载场景: {sceneName}");
            SceneManager.LoadScene(sceneName);
            currentSceneName = sceneName;
            Debug.Log($"[GameSceneManager] ✓ 场景加载完成: {sceneName} (Build Settings)");
        }
        
        isLoading = false;
        yield return null;
    }
    
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="onProgress">进度回调（0-1）</param>
    /// <param name="onComplete">完成回调</param>
    public void LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action onComplete = null)
    {
        if (isLoading)
        {
            Debug.LogWarning($"[GameSceneManager] 场景正在加载中，无法加载新场景: {sceneName}");
            onComplete?.Invoke();
            return;
        }
        
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onProgress, onComplete));
    }
    
    /// <summary>
    /// 异步加载场景协程
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action<float> onProgress, Action onComplete)
    {
        isLoading = true;
        Debug.Log($"[GameSceneManager] 开始异步加载场景: {sceneName} (方式: {(useAddressables ? "Addressables" : "Build Settings")})");
        
        if (useAddressables)
        {
            // 尝试使用Addressables加载场景
            var loader = ResManager.GetResourceLoader() as AddressablesLoader;
            bool useAddressablesSuccess = false;
            object addressablesHandle = null;
            Type addressablesHandleType = null;
            Type statusType = null;
            object succeededStatus = null;
            object failedStatus = null;
            Exception addressablesException = null;
            
            // 在try块中只做反射调用，不包含yield return
            if (loader != null)
            {
                Type addressablesType = Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables");
                if (addressablesType != null)
                {
                    try
                    {
                        // 使用反射调用Addressables.LoadSceneAsync
                        MethodInfo loadSceneMethod = addressablesType.GetMethod("LoadSceneAsync", 
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            new Type[] { typeof(string), typeof(LoadSceneMode) },
                            null);
                        
                        if (loadSceneMethod != null)
                        {
                            addressablesHandle = loadSceneMethod.Invoke(null, new object[] { sceneName, LoadSceneMode.Single });
                            if (addressablesHandle != null)
                            {
                                addressablesHandleType = addressablesHandle.GetType();
                                statusType = Type.GetType("UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus, Unity.ResourceManager");
                                if (statusType != null)
                                {
                                    succeededStatus = Enum.Parse(statusType, "Succeeded");
                                    failedStatus = Enum.Parse(statusType, "Failed");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        addressablesException = e;
                        Debug.LogWarning($"[GameSceneManager] Addressables加载场景异常，回退到Build Settings方式: {e.Message}");
                    }
                }
            }
            
            // 如果成功获取了handle，等待加载完成（yield return在try-catch之外）
            if (addressablesHandle != null && addressablesHandleType != null && statusType != null)
            {
                float fakeProgress = 0f;
                bool sceneLoaded = false;
                while (true)
                {
                    try
                    {
                        PropertyInfo statusProp = addressablesHandleType.GetProperty("Status");
                        if (statusProp != null)
                        {
                            object status = statusProp.GetValue(addressablesHandle);
                            
                            if (status.Equals(succeededStatus))
                            {
                                sceneLoaded = true;
                                break;
                            }
                            else if (status.Equals(failedStatus))
                            {
                                Debug.LogWarning($"[GameSceneManager] Addressables加载场景失败，回退到Build Settings方式: {sceneName}");
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[GameSceneManager] 检查Addressables状态异常，回退到Build Settings方式: {e.Message}");
                        break;
                    }
                    
                    fakeProgress = Mathf.Min(fakeProgress + 0.05f, 0.9f);
                    onProgress?.Invoke(fakeProgress);
                    yield return null;
                }
                
                // 如果场景加载成功，等待两帧确保场景完全激活
                if (sceneLoaded)
                {
                    yield return null;
                    yield return null;
                    
                    currentSceneName = sceneName;
                    Debug.Log($"[GameSceneManager] ✓ 场景异步加载完成: {sceneName} (Addressables)");
                    onProgress?.Invoke(1.0f);
                    onComplete?.Invoke();
                    useAddressablesSuccess = true;
                }
            }
            
            // 如果Addressables加载失败，回退到Build Settings方式
            if (!useAddressablesSuccess)
            {
                if (addressablesException == null)
                {
                    Debug.LogWarning($"[GameSceneManager] Addressables不可用，回退到Build Settings方式异步加载场景: {sceneName}");
                }
                
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                asyncOperation.allowSceneActivation = true;
                
                while (!asyncOperation.isDone)
                {
                    float progress = asyncOperation.progress;
                    onProgress?.Invoke(progress);
                    
                    if (progress >= 0.9f)
                    {
                        break;
                    }
                    
                    yield return null;
                }
                
                yield return null;
                
                currentSceneName = sceneName;
                onProgress?.Invoke(1.0f);
                onComplete?.Invoke();
                Debug.Log($"[GameSceneManager] ✓ 场景异步加载完成: {sceneName} (Build Settings)");
            }
        }
        else
        {
            // 使用Build Settings方式加载场景
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = true;
            
            while (!asyncOperation.isDone)
            {
                float progress = asyncOperation.progress;
                onProgress?.Invoke(progress);
                
                if (progress >= 0.9f)
                {
                    // 进度达到90%时，可以激活场景
                    break;
                }
                
                yield return null;
            }
            
            // 等待一帧确保场景完全加载
            yield return null;
            // 再等待一帧确保场景完全激活
            yield return null;
            
            currentSceneName = sceneName;
            onProgress?.Invoke(1.0f);
            onComplete?.Invoke();
            Debug.Log($"[GameSceneManager] ✓ 场景异步加载完成: {sceneName}");
        }
        
        isLoading = false;
    }
    
    /// <summary>
    /// 预加载场景（不激活）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="onComplete">完成回调</param>
    public void PreloadScene(string sceneName, Action onComplete = null)
    {
        StartCoroutine(PreloadSceneCoroutine(sceneName, onComplete));
    }
    
    /// <summary>
    /// 预加载场景协程
    /// </summary>
    private IEnumerator PreloadSceneCoroutine(string sceneName, Action onComplete)
    {
        Debug.Log($"[GameSceneManager] 开始预加载场景: {sceneName}");
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;
        
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        
        Debug.Log($"[GameSceneManager] ✓ 场景预加载完成: {sceneName}");
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 卸载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void UnloadScene(string sceneName, Action onComplete = null)
    {
        StartCoroutine(UnloadSceneCoroutine(sceneName, onComplete));
    }
    
    /// <summary>
    /// 卸载场景协程
    /// </summary>
    private IEnumerator UnloadSceneCoroutine(string sceneName, Action onComplete)
    {
        Debug.Log($"[GameSceneManager] 开始卸载场景: {sceneName}");
        
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        Debug.Log($"[GameSceneManager] ✓ 场景卸载完成: {sceneName}");
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    
    /// <summary>
    /// 检查场景是否正在加载
    /// </summary>
    public bool IsLoading()
    {
        return isLoading;
    }
    
    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        LoadScene(currentSceneName);
    }
    
    /// <summary>
    /// 异步重新加载当前场景
    /// </summary>
    public void ReloadCurrentSceneAsync(Action<float> onProgress = null, Action onComplete = null)
    {
        LoadSceneAsync(currentSceneName, onProgress, onComplete);
    }
}

