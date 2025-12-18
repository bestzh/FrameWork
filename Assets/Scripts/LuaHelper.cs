using UnityEngine;
using System.Collections;
using System.IO;
using Framework.ResourceLoader;
using XLua;

/// <summary>
/// Lua辅助类 - 提供Lua友好的API接口
/// 由于Lua不能直接调用C#泛型方法，这里提供非泛型版本
/// </summary>
[XLua.LuaCallCSharp]
public static class LuaHelper
{
    #region UI框架封装
    
    /// <summary>
    /// 从Lua table创建LuaTable（辅助方法）
    /// </summary>
    public static XLua.LuaTable CreateLuaTable(object luaTable)
    {
        // 如果已经是LuaTable，直接返回
        if (luaTable is XLua.LuaTable)
        {
            return luaTable as XLua.LuaTable;
        }
        
        // 否则尝试转换（XLua会自动处理）
        return luaTable as XLua.LuaTable;
    }
    
    /// <summary>
    /// 加载Lua驱动的UI（推荐用于热更新）
    /// 使用通用的LuaUIBase，无需创建单独的C#脚本
    /// </summary>
    /// <param name="uiName">UI名称（用于缓存和标识）</param>
    /// <param name="uiPath">UI预制体路径</param>
    /// <param name="luaCallbacks">Lua回调函数表（包含OnInitialize、OnShow、OnHide等），可以是LuaTable或普通object（会自动转换）</param>
    /// <param name="showImmediately">是否立即显示</param>
    /// <returns>UI的GameObject</returns>
    public static GameObject LoadLuaUI(string uiName, string uiPath, object luaCallbacks = null, bool showImmediately = false)
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return null;
        }
        
        // 注意：UIManager使用类型名作为缓存key，所以我们需要确保每个UI实例都有唯一的标识
        // 但由于都使用LuaUIBase类型，我们需要特殊处理缓存
        
        // 先检查是否已加载（通过GameObject名称）
        var existingUI = GameObject.Find(uiName);
        if (existingUI != null)
        {
            var luaUI = existingUI.GetComponent<UI.LuaUIBase>();
            if (luaUI != null)
            {
                if (showImmediately) luaUI.Show();
                return existingUI;
            }
        }
        
        // 使用LuaUIBase加载
        // 注意：由于UIManager的缓存机制，我们需要确保每个UI有唯一标识
        // 这里我们通过修改预制体名称来区分
        var resourceLoader = ResManager.GetResourceLoader();
        if (resourceLoader == null)
        {
            Debug.LogError($"[LuaHelper] 资源加载器未初始化！无法加载UI预制体: {uiPath}");
            return null;
        }
        
        Debug.Log($"[LuaHelper] 尝试加载UI预制体: {uiPath} (使用加载器: {resourceLoader.GetType().Name})");
        var prefab = ResManager.Load<GameObject>(uiPath);
        if (prefab == null)
        {
            Debug.LogError($"[LuaHelper] ✗ 无法加载UI预制体: {uiPath}");
            Debug.LogError($"[LuaHelper] 使用的资源加载器: {resourceLoader.GetType().Name}");
            Debug.LogError($"[LuaHelper] 请检查：");
            Debug.LogError($"  1. 资源路径是否正确: {uiPath}");
            Debug.LogError($"  2. 如果使用Addressables，请确认资源已标记为Addressable");
            Debug.LogError($"  3. 如果使用Addressables，请确认已构建Addressables资源");
            Debug.LogError($"  4. 查看上方的详细错误信息（来自资源加载器）");
            return null;
        }
        
        Debug.Log($"[LuaHelper] ✓ 成功加载UI预制体: {uiPath}");
        
        // 获取主Canvas（UI应该挂载到这个Canvas下）
        var mainCanvas = uiManager.MainCanvas;
        if (mainCanvas == null)
        {
            Debug.LogError("[LuaHelper] UIManager的主Canvas未初始化！");
            return null;
        }
        
        // 实例化UI，直接设置父节点为主Canvas
        var uiObject = Object.Instantiate(prefab, mainCanvas.transform);
        uiObject.name = uiName; // 设置唯一名称
        
        // 添加或获取LuaUIBase组件
        var ui = uiObject.GetComponent<UI.LuaUIBase>();
        if (ui == null)
        {
            ui = uiObject.AddComponent<UI.LuaUIBase>();
        }
        
        // 设置Lua回调
        if (luaCallbacks != null)
        {
            // 确保转换为LuaTable
            XLua.LuaTable callbacksTable = luaCallbacks as XLua.LuaTable;
            if (callbacksTable == null && luaCallbacks is System.Collections.IDictionary)
            {
                // 如果不是LuaTable，尝试从LuaManager获取LuaEnv来创建
                var luaManager = LuaManager.Instance;
                if (luaManager != null && luaManager.LuaEnv != null)
                {
                    // 创建一个新的LuaTable并复制值
                    callbacksTable = luaManager.LuaEnv.NewTable();
                    var dict = luaCallbacks as System.Collections.IDictionary;
                    foreach (System.Collections.DictionaryEntry entry in dict)
                    {
                        callbacksTable.Set(entry.Key, entry.Value);
                    }
                }
            }
            
            if (callbacksTable != null)
            {
                ui.SetLuaCallbacks(callbacksTable);
            }
        }
        
        // 初始化
        ui.Initialize();
        
        // 设置UI层级（默认Normal层）
        UI.UIHierarchyManager.Instance.SetUILayer(ui, UI.UIHierarchyManager.UILayer.Normal);
        
        if (showImmediately)
        {
            ui.Show();
        }
        else
        {
            ui.HideImmediate();
        }
        
        return uiObject;
    }
    
    /// <summary>
    /// 加载UI（通过字符串类型名）
    /// 注意：需要对应的C#类，不支持热更新
    /// </summary>
    /// <param name="uiTypeName">UI类型名称（如"MainMenuUI"）</param>
    /// <param name="uiPath">UI预制体路径</param>
    /// <param name="showImmediately">是否立即显示</param>
    /// <returns>UI的GameObject</returns>
    public static GameObject LoadUI(string uiTypeName, string uiPath, bool showImmediately = false)
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return null;
        }
        
        // 使用反射调用泛型方法
        var method = typeof(UI.UIManager).GetMethod("LoadUI", new System.Type[] { typeof(string), typeof(bool) });
        if (method == null)
        {
            Debug.LogError($"找不到LoadUI方法");
            return null;
        }
        
        // 获取UI类型
        var uiType = System.Type.GetType($"UI.{uiTypeName}");
        if (uiType == null)
        {
            Debug.LogError($"找不到UI类型: {uiTypeName}");
            return null;
        }
        
        // 创建泛型方法
        var genericMethod = method.MakeGenericMethod(uiType);
        var ui = genericMethod.Invoke(uiManager, new object[] { uiPath, showImmediately });
        
        if (ui != null && ui is UI.UIBase)
        {
            return (ui as UI.UIBase).gameObject;
        }
        
        return null;
    }
    
    /// <summary>
    /// 显示UI（通过字符串类型名）
    /// </summary>
    public static void ShowUI(string uiTypeName)
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return;
        }
        
        var method = typeof(UI.UIManager).GetMethod("ShowUI");
        if (method == null) return;
        
        var uiType = System.Type.GetType($"UI.{uiTypeName}");
        if (uiType == null)
        {
            Debug.LogError($"找不到UI类型: {uiTypeName}");
            return;
        }
        
        var genericMethod = method.MakeGenericMethod(uiType);
        genericMethod.Invoke(uiManager, null);
    }
    
    /// <summary>
    /// 隐藏Lua驱动的UI（通过UI名称）
    /// </summary>
    public static void HideLuaUI(string uiName)
    {
        // 通过GameObject名称查找UI
        var uiObject = GameObject.Find(uiName);
        if (uiObject != null)
        {
            var luaUI = uiObject.GetComponent<UI.LuaUIBase>();
            if (luaUI != null)
            {
                luaUI.Hide();
                Debug.Log($"[LuaHelper] 已隐藏Lua UI: {uiName}");
            }
            else
            {
                Debug.LogWarning($"[LuaHelper] GameObject '{uiName}' 上未找到LuaUIBase组件");
            }
        }
        else
        {
            Debug.LogWarning($"[LuaHelper] 未找到名为 '{uiName}' 的UI");
        }
    }
    
    /// <summary>
    /// 显示Lua驱动的UI（通过UI名称）
    /// </summary>
    public static void ShowLuaUI(string uiName)
    {
        // 通过GameObject名称查找UI
        var uiObject = GameObject.Find(uiName);
        if (uiObject != null)
        {
            var luaUI = uiObject.GetComponent<UI.LuaUIBase>();
            if (luaUI != null)
            {
                luaUI.Show();
                Debug.Log($"[LuaHelper] 已显示Lua UI: {uiName}");
            }
            else
            {
                Debug.LogWarning($"[LuaHelper] GameObject '{uiName}' 上未找到LuaUIBase组件");
            }
        }
        else
        {
            Debug.LogWarning($"[LuaHelper] 未找到名为 '{uiName}' 的UI");
        }
    }
    
    /// <summary>
    /// 隐藏UI（通过字符串类型名，需要对应的C#类）
    /// </summary>
    public static void HideUI(string uiTypeName)
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return;
        }
        
        var method = typeof(UI.UIManager).GetMethod("HideUI");
        if (method == null) return;
        
        var uiType = System.Type.GetType($"UI.{uiTypeName}");
        if (uiType == null)
        {
            Debug.LogError($"找不到UI类型: {uiTypeName}");
            return;
        }
        
        var genericMethod = method.MakeGenericMethod(uiType);
        genericMethod.Invoke(uiManager, null);
    }
    
    /// <summary>
    /// 卸载Lua驱动的UI（通过UI名称，会销毁GameObject）
    /// </summary>
    public static void UnloadLuaUI(string uiName)
    {
        // 通过GameObject名称查找UI
        var uiObject = GameObject.Find(uiName);
        if (uiObject != null)
        {
            var luaUI = uiObject.GetComponent<UI.LuaUIBase>();
            if (luaUI != null)
            {
                // 先隐藏UI（会触发OnHide回调）
                luaUI.Hide();
                
                // 销毁GameObject（会触发OnDestroy回调，自动清理按钮事件和Lua回调）
                Object.Destroy(uiObject);
                Debug.Log($"[LuaHelper] 已卸载Lua UI: {uiName}");
            }
            else
            {
                Debug.LogWarning($"[LuaHelper] GameObject '{uiName}' 上未找到LuaUIBase组件，直接销毁");
                Object.Destroy(uiObject);
            }
        }
        else
        {
            Debug.LogWarning($"[LuaHelper] 未找到名为 '{uiName}' 的UI");
        }
    }
    
    /// <summary>
    /// 卸载UI（通过字符串类型名，需要对应的C#类）
    /// </summary>
    public static void UnloadUI(string uiTypeName)
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return;
        }
        
        var method = typeof(UI.UIManager).GetMethod("UnloadUI");
        if (method == null) return;
        
        var uiType = System.Type.GetType($"UI.{uiTypeName}");
        if (uiType == null)
        {
            Debug.LogError($"找不到UI类型: {uiTypeName}");
            return;
        }
        
        var genericMethod = method.MakeGenericMethod(uiType);
        genericMethod.Invoke(uiManager, null);
    }
    
    /// <summary>
    /// 隐藏所有UI
    /// </summary>
    public static void HideAllUI()
    {
        var uiManager = UI.UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager未初始化！");
            return;
        }
        
        uiManager.HideAllUI();
    }
    
    #endregion
    
    #region 资源加载框架封装
    
    /// <summary>
    /// 通用资源加载方法（通过类型名）
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="typeName">资源类型名称（如"GameObject", "Texture2D", "Sprite", "AudioClip", "TextAsset", "Material"等）</param>
    /// <returns>加载的资源对象</returns>
    public static Object LoadResource(string path, string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            typeName = "GameObject";
        }
        
        switch (typeName)
        {
            case "GameObject":
                return ResManager.Load<GameObject>(path);
            case "Texture2D":
                return ResManager.Load<Texture2D>(path);
            case "Sprite":
                return ResManager.Load<Sprite>(path);
            case "AudioClip":
                return ResManager.Load<AudioClip>(path);
            case "TextAsset":
                return ResManager.Load<TextAsset>(path);
            case "Material":
                return ResManager.Load<Material>(path);
            case "Texture":
                return ResManager.Load<Texture>(path);
            case "Mesh":
                return ResManager.Load<Mesh>(path);
            case "Font":
                return ResManager.Load<Font>(path);
            case "Shader":
                return ResManager.Load<Shader>(path);
            case "AnimationClip":
                return ResManager.Load<AnimationClip>(path);
            default:
                Debug.LogWarning($"[LuaHelper] 未知的资源类型: {typeName}，使用Object类型加载");
                return ResManager.Load<Object>(path);
        }
    }
    
    /// <summary>
    /// 通用异步资源加载方法（通过类型名）
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="typeName">资源类型名称</param>
    /// <param name="onComplete">加载完成回调</param>
    public static void LoadResourceAsync(string path, string typeName, System.Action<Object> onComplete)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager == null)
        {
            Debug.LogError("LuaManager未初始化！");
            onComplete?.Invoke(null);
            return;
        }
        
        if (string.IsNullOrEmpty(typeName))
        {
            typeName = "GameObject";
        }
        
        MonoBehaviour mb = luaManager as MonoBehaviour;
        
        switch (typeName)
        {
            case "GameObject":
                mb.StartCoroutine(ResManager.LoadAsync<GameObject>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Texture2D":
                mb.StartCoroutine(ResManager.LoadAsync<Texture2D>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Sprite":
                mb.StartCoroutine(ResManager.LoadAsync<Sprite>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "AudioClip":
                mb.StartCoroutine(ResManager.LoadAsync<AudioClip>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "TextAsset":
                mb.StartCoroutine(ResManager.LoadAsync<TextAsset>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Material":
                mb.StartCoroutine(ResManager.LoadAsync<Material>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Texture":
                mb.StartCoroutine(ResManager.LoadAsync<Texture>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Mesh":
                mb.StartCoroutine(ResManager.LoadAsync<Mesh>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Font":
                mb.StartCoroutine(ResManager.LoadAsync<Font>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "Shader":
                mb.StartCoroutine(ResManager.LoadAsync<Shader>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            case "AnimationClip":
                mb.StartCoroutine(ResManager.LoadAsync<AnimationClip>(path, (obj) => onComplete?.Invoke(obj)));
                break;
            default:
                Debug.LogWarning($"[LuaHelper] 未知的资源类型: {typeName}，使用Object类型加载");
                mb.StartCoroutine(ResManager.LoadAsync<Object>(path, (obj) => onComplete?.Invoke(obj)));
                break;
        }
    }
    
    /// <summary>
    /// 加载GameObject资源
    /// </summary>
    public static GameObject LoadGameObject(string path)
    {
        return ResManager.Load<GameObject>(path);
    }
    
    /// <summary>
    /// 加载Texture2D资源
    /// </summary>
    public static Texture2D LoadTexture(string path)
    {
        return ResManager.Load<Texture2D>(path);
    }
    
    /// <summary>
    /// 加载Sprite资源
    /// </summary>
    public static Sprite LoadSprite(string path)
    {
        return ResManager.Load<Sprite>(path);
    }
    
    /// <summary>
    /// 加载AudioClip资源
    /// </summary>
    public static AudioClip LoadAudioClip(string path)
    {
        return ResManager.Load<AudioClip>(path);
    }
    
    /// <summary>
    /// 加载TextAsset资源
    /// </summary>
    public static TextAsset LoadTextAsset(string path)
    {
        return ResManager.Load<TextAsset>(path);
    }
    
    /// <summary>
    /// 加载Material资源
    /// </summary>
    public static Material LoadMaterial(string path)
    {
        return ResManager.Load<Material>(path);
    }
    
    /// <summary>
    /// 异步加载GameObject资源
    /// </summary>
    public static void LoadGameObjectAsync(string path, System.Action<GameObject> onComplete)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(ResManager.LoadAsync<GameObject>(path, onComplete));
        }
        else
        {
            Debug.LogError("LuaManager未初始化！");
            onComplete?.Invoke(null);
        }
    }
    
    /// <summary>
    /// 异步加载Texture2D资源
    /// </summary>
    public static void LoadTextureAsync(string path, System.Action<Texture2D> onComplete)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(ResManager.LoadAsync<Texture2D>(path, onComplete));
        }
        else
        {
            Debug.LogError("LuaManager未初始化！");
            onComplete?.Invoke(null);
        }
    }
    
    /// <summary>
    /// 异步加载Sprite资源
    /// </summary>
    public static void LoadSpriteAsync(string path, System.Action<Sprite> onComplete)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(ResManager.LoadAsync<Sprite>(path, onComplete));
        }
        else
        {
            Debug.LogError("LuaManager未初始化！");
            onComplete?.Invoke(null);
        }
    }
    
    /// <summary>
    /// 异步加载AudioClip资源
    /// </summary>
    public static void LoadAudioClipAsync(string path, System.Action<AudioClip> onComplete)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(ResManager.LoadAsync<AudioClip>(path, onComplete));
        }
        else
        {
            Debug.LogError("LuaManager未初始化！");
            onComplete?.Invoke(null);
        }
    }
    
    /// <summary>
    /// 实例化GameObject
    /// </summary>
    public static GameObject Instantiate(GameObject prefab)
    {
        if (prefab == null) return null;
        return Object.Instantiate(prefab);
    }
    
    /// <summary>
    /// 实例化GameObject到指定父节点
    /// </summary>
    public static GameObject Instantiate(GameObject prefab, Transform parent)
    {
        if (prefab == null) return null;
        return Object.Instantiate(prefab, parent);
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 打印日志
    /// </summary>
    public static void Log(object message)
    {
        Debug.Log(message);
    }
    
    /// <summary>
    /// 打印警告
    /// </summary>
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }
    
    /// <summary>
    /// 打印错误
    /// </summary>
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }
    
    /// <summary>
    /// 创建GameObject
    /// </summary>
    public static GameObject CreateGameObject(string name)
    {
        return new GameObject(name);
    }
    
    /// <summary>
    /// 销毁GameObject
    /// </summary>
    public static void DestroyGameObject(GameObject obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj);
        }
    }
    
    /// <summary>
    /// 查找GameObject（通过名称）
    /// </summary>
    public static GameObject FindGameObject(string name)
    {
        return GameObject.Find(name);
    }
    
    /// <summary>
    /// 查找GameObject（通过标签）
    /// </summary>
    public static GameObject FindGameObjectWithTag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }
    
    /// <summary>
    /// 延迟执行回调（等待指定秒数）
    /// </summary>
    /// <param name="callback">回调函数</param>
    /// <param name="delay">延迟时间（秒，默认0.1秒）</param>
    public static void DelayCall(System.Action callback, float delay = 0.1f)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && callback != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(DelayCallCoroutine(callback, delay));
        }
    }
    
    /// <summary>
    /// 延迟执行协程
    /// </summary>
    private static IEnumerator DelayCallCoroutine(System.Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
    
    #endregion
    
    #region 场景管理封装
    
    /// <summary>
    /// 同步加载场景
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("GameSceneManager未初始化！");
        }
    }
    
    /// <summary>
    /// 异步加载场景
    /// </summary>
    public static void LoadSceneAsync(string sceneName, System.Action<float> onProgress = null, System.Action onComplete = null)
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.LoadSceneAsync(sceneName, onProgress, onComplete);
        }
        else
        {
            Debug.LogError("GameSceneManager未初始化！");
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 预加载场景
    /// </summary>
    public static void PreloadScene(string sceneName, System.Action onComplete = null)
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.PreloadScene(sceneName, onComplete);
        }
        else
        {
            Debug.LogError("GameSceneManager未初始化！");
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 卸载场景
    /// </summary>
    public static void UnloadScene(string sceneName, System.Action onComplete = null)
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.UnloadScene(sceneName, onComplete);
        }
        else
        {
            Debug.LogError("GameSceneManager未初始化！");
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public static string GetCurrentSceneName()
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            return sceneManager.GetCurrentSceneName();
        }
        return "";
    }
    
    /// <summary>
    /// 检查场景是否正在加载
    /// </summary>
    public static bool IsSceneLoading()
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            return sceneManager.IsLoading();
        }
        return false;
    }
    
    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public static void ReloadCurrentScene()
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.ReloadCurrentScene();
        }
    }
    
    /// <summary>
    /// 异步重新加载当前场景
    /// </summary>
    public static void ReloadCurrentSceneAsync(System.Action<float> onProgress = null, System.Action onComplete = null)
    {
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            sceneManager.ReloadCurrentSceneAsync(onProgress, onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    #endregion
    
    #region 音频管理封装
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public static void PlayMusic(string clipName, bool loop = true, bool fadeIn = false, float fadeTime = 1.0f)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlayMusic(clipName, loop, fadeIn, fadeTime);
        }
        else
        {
            Debug.LogError("AudioManager未初始化！");
        }
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public static void StopMusic(bool fadeOut = false, float fadeTime = 1.0f)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.StopMusic(fadeOut, fadeTime);
        }
    }
    
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public static void PauseMusic()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PauseMusic();
        }
    }
    
    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public static void ResumeMusic()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.ResumeMusic();
        }
    }
    
    /// <summary>
    /// 切换背景音乐（淡出旧音乐，淡入新音乐）
    /// </summary>
    public static void SwitchMusic(string newClipName, bool loop = true, float fadeTime = 1.0f)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.SwitchMusic(newClipName, loop, fadeTime);
        }
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    public static void PlaySound(string clipName, float volume = -1, float pitch = 1.0f)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySound(clipName, volume, pitch);
        }
    }
    
    /// <summary>
    /// 停止所有音效
    /// </summary>
    public static void StopAllSounds()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.StopAllSounds();
        }
    }
    
    /// <summary>
    /// 停止指定音效
    /// </summary>
    public static void StopSound(string clipName)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.StopSound(clipName);
        }
    }
    
    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    public static void SetMusicVolume(float volume)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(volume);
        }
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    public static void SetSoundVolume(float volume)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.SetSoundVolume(volume);
        }
    }
    
    /// <summary>
    /// 获取背景音乐音量
    /// </summary>
    public static float GetMusicVolume()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.GetMusicVolume();
        }
        return 1.0f;
    }
    
    /// <summary>
    /// 获取音效音量
    /// </summary>
    public static float GetSoundVolume()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.GetSoundVolume();
        }
        return 1.0f;
    }
    
    /// <summary>
    /// 静音/取消静音背景音乐
    /// </summary>
    public static void SetMusicMuted(bool muted)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.SetMusicMuted(muted);
        }
    }
    
    /// <summary>
    /// 静音/取消静音音效
    /// </summary>
    public static void SetSoundMuted(bool muted)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.SetSoundMuted(muted);
        }
    }
    
    /// <summary>
    /// 检查音乐是否静音
    /// </summary>
    public static bool IsMusicMuted()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.IsMusicMuted();
        }
        return false;
    }
    
    /// <summary>
    /// 检查音效是否静音
    /// </summary>
    public static bool IsSoundMuted()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.IsSoundMuted();
        }
        return false;
    }
    
    /// <summary>
    /// 预加载音频资源
    /// </summary>
    public static void PreloadAudio(string clipName, System.Action<bool> onComplete = null)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PreloadAudio(clipName, onComplete);
        }
        else
        {
            onComplete?.Invoke(false);
        }
    }
    
    /// <summary>
    /// 卸载音频资源
    /// </summary>
    public static void UnloadAudio(string clipName)
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.UnloadAudio(clipName);
        }
    }
    
    /// <summary>
    /// 清空音频缓存
    /// </summary>
    public static void ClearAudioCache()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.ClearAudioCache();
        }
    }
    
    /// <summary>
    /// 检查音乐是否正在播放
    /// </summary>
    public static bool IsMusicPlaying()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.IsMusicPlaying();
        }
        return false;
    }
    
    /// <summary>
    /// 获取当前播放的音乐名称
    /// </summary>
    public static string GetCurrentMusicName()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.GetCurrentMusicName();
        }
        return "";
    }
    
    /// <summary>
    /// 获取活动音效数量
    /// </summary>
    public static int GetActiveSoundCount()
    {
        var audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            return audioManager.GetActiveSoundCount();
        }
        return 0;
    }
    
    #endregion
    
    #region 数据存储封装
    
    /// <summary>
    /// 保存整数
    /// </summary>
    public static void SaveInt(string key, int value)
    {
        SaveManager.Instance.SaveInt(key, value);
    }
    
    /// <summary>
    /// 加载整数
    /// </summary>
    public static int LoadInt(string key, int defaultValue = 0)
    {
        return SaveManager.Instance.LoadInt(key, defaultValue);
    }
    
    /// <summary>
    /// 保存浮点数
    /// </summary>
    public static void SaveFloat(string key, float value)
    {
        SaveManager.Instance.SaveFloat(key, value);
    }
    
    /// <summary>
    /// 加载浮点数
    /// </summary>
    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        return SaveManager.Instance.LoadFloat(key, defaultValue);
    }
    
    /// <summary>
    /// 保存字符串
    /// </summary>
    public static void SaveString(string key, string value)
    {
        SaveManager.Instance.SaveString(key, value);
    }
    
    /// <summary>
    /// 加载字符串
    /// </summary>
    public static string LoadString(string key, string defaultValue = "")
    {
        return SaveManager.Instance.LoadString(key, defaultValue);
    }
    
    /// <summary>
    /// 保存布尔值
    /// </summary>
    public static void SaveBool(string key, bool value)
    {
        SaveManager.Instance.SaveBool(key, value);
    }
    
    /// <summary>
    /// 加载布尔值
    /// </summary>
    public static bool LoadBool(string key, bool defaultValue = false)
    {
        return SaveManager.Instance.LoadBool(key, defaultValue);
    }
    
    /// <summary>
    /// 保存对象到JSON文件（Lua传递JSON字符串）
    /// </summary>
    public static void SaveToJson(string fileName, string jsonString, bool encrypt = false)
    {
        // Lua端应该先调用TableToJson转换为JSON字符串，然后传递过来
        SaveManager.Instance.SaveToJson(fileName, jsonString, encrypt);
    }
    
    /// <summary>
    /// 从JSON文件加载对象（返回JSON字符串，Lua端自行解析）
    /// </summary>
    public static string LoadFromJson(string fileName, bool encrypted = false)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SaveData", fileName + ".json");
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        return "";
    }
    
    /// <summary>
    /// 删除数据
    /// </summary>
    public static void DeleteData(string key)
    {
        SaveManager.Instance.DeleteData(key);
    }
    
    /// <summary>
    /// 检查数据是否存在
    /// </summary>
    public static bool HasData(string key)
    {
        return SaveManager.Instance.HasData(key);
    }
    
    /// <summary>
    /// 删除所有PlayerPrefs数据
    /// </summary>
    public static void DeleteAll()
    {
        SaveManager.Instance.DeleteAll();
    }
    
    /// <summary>
    /// 设置是否启用加密
    /// </summary>
    public static void SetEncryptionEnabled(bool enabled)
    {
        SaveManager.Instance.SetEncryptionEnabled(enabled);
    }
    
    /// <summary>
    /// 获取保存数据路径
    /// </summary>
    public static string GetSaveDataPath()
    {
        return SaveManager.Instance.GetSaveDataPath();
    }
    
    #endregion
    
    #region 对象池封装
    
    /// <summary>
    /// 创建对象池
    /// </summary>
    public static void CreatePool(string poolName, GameObject prefab, int maxSize = -1, float defaultAutoReleaseTime = -1f)
    {
        ObjectPool.Instance.CreatePool(poolName, prefab, maxSize, defaultAutoReleaseTime);
    }
    
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public static GameObject GetFromPool(string poolName, GameObject prefab = null, float autoReleaseTime = -2f)
    {
        return ObjectPool.Instance.Get(poolName, prefab, autoReleaseTime);
    }
    
    /// <summary>
    /// 将对象回收到对象池
    /// </summary>
    public static void ReleaseToPool(string poolName, GameObject obj)
    {
        ObjectPool.Instance.Release(poolName, obj);
    }
    
    /// <summary>
    /// 将对象回收到对象池（自动识别池名）
    /// </summary>
    public static void ReleaseToPool(GameObject obj)
    {
        ObjectPool.Instance.Release(obj);
    }
    
    /// <summary>
    /// 预加载对象到对象池
    /// </summary>
    public static void PreloadPool(string poolName, GameObject prefab, int count)
    {
        ObjectPool.Instance.Preload(poolName, prefab, count, null);
    }
    
    /// <summary>
    /// 延迟自动回收对象
    /// </summary>
    public static void AutoReleasePool(string poolName, GameObject obj, float delay)
    {
        ObjectPool.Instance.AutoRelease(poolName, obj, delay);
    }
    
    /// <summary>
    /// 销毁对象池
    /// </summary>
    public static void DestroyPool(string poolName)
    {
        ObjectPool.Instance.DestroyPool(poolName);
    }
    
    /// <summary>
    /// 清空对象池
    /// </summary>
    public static void ClearPool(string poolName)
    {
        ObjectPool.Instance.ClearPool(poolName);
    }
    
    /// <summary>
    /// 获取对象池信息
    /// </summary>
    public static PoolInfo GetPoolInfo(string poolName)
    {
        return ObjectPool.Instance.GetPoolInfo(poolName);
    }
    
    /// <summary>
    /// 设置对象池最大大小
    /// </summary>
    public static void SetPoolMaxSize(string poolName, int maxSize)
    {
        ObjectPool.Instance.SetPoolMaxSize(poolName, maxSize);
    }
    
    /// <summary>
    /// 设置对象池默认自动回收时间
    /// </summary>
    public static void SetPoolDefaultAutoReleaseTime(string poolName, float autoReleaseTime)
    {
        ObjectPool.Instance.SetPoolDefaultAutoReleaseTime(poolName, autoReleaseTime);
    }
    
    /// <summary>
    /// 回收所有活跃对象
    /// </summary>
    public static void ReleaseAllFromPool(string poolName = null)
    {
        ObjectPool.Instance.ReleaseAll(poolName);
    }
    
    #endregion
    
    #region 事件系统封装
    
    /// <summary>
    /// 注册事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public static void RegisterEvent(string eventName, System.Action callback)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.RegisterEvent(eventName, callback);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 注册事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public static void RegisterEvent(string eventName, System.Action<object> callback)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.RegisterEvent(eventName, callback);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 注册优先级事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级（数字越大优先级越高）</param>
    public static void RegisterEventWithPriority(string eventName, System.Action callback, int priority)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.RegisterEvent(eventName, callback, priority);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 注册优先级事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级（数字越大优先级越高）</param>
    public static void RegisterEventWithPriority(string eventName, System.Action<object> callback, int priority)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.RegisterEvent(eventName, callback, priority);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 注销事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public static void UnregisterEvent(string eventName, System.Action callback)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.UnregisterEvent(eventName, callback);
        }
    }
    
    /// <summary>
    /// 注销事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public static void UnregisterEvent(string eventName, System.Action<object> callback)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.UnregisterEvent(eventName, callback);
        }
    }
    
    /// <summary>
    /// 注销优先级事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级</param>
    public static void UnregisterEventWithPriority(string eventName, System.Action callback, int priority)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.UnregisterEvent(eventName, callback, priority);
        }
    }
    
    /// <summary>
    /// 注销优先级事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级</param>
    public static void UnregisterEventWithPriority(string eventName, System.Action<object> callback, int priority)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.UnregisterEvent(eventName, callback, priority);
        }
    }
    
    /// <summary>
    /// 注销所有指定事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public static void UnregisterAllEvents(string eventName)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.UnregisterAll(eventName);
        }
    }
    
    /// <summary>
    /// 触发事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public static void TriggerEvent(string eventName)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.TriggerEvent(eventName);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 触发事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="data">事件数据</param>
    public static void TriggerEvent(string eventName, object data)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.TriggerEvent(eventName, data);
        }
        else
        {
            Debug.LogError("[LuaHelper] EventManager未初始化！");
        }
    }
    
    /// <summary>
    /// 检查事件是否已注册
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>是否已注册</returns>
    public static bool HasEvent(string eventName)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            return eventManager.HasEvent(eventName);
        }
        return false;
    }
    
    /// <summary>
    /// 获取事件监听者数量
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>监听者数量</returns>
    public static int GetEventListenerCount(string eventName)
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            return eventManager.GetListenerCount(eventName);
        }
        return 0;
    }
    
    /// <summary>
    /// 清空所有事件
    /// </summary>
    public static void ClearAllEvents()
    {
        var eventManager = EventManager.Instance;
        if (eventManager != null)
        {
            eventManager.ClearAllEvents();
        }
    }
    
    #endregion
    
    #region 协程辅助封装
    
    /// <summary>
    /// 启动协程（从Lua函数创建）
    /// </summary>
    /// <param name="coroutineFunc">协程函数（返回IEnumerator的函数）</param>
    /// <returns>协程对象</returns>
    public static System.Collections.IEnumerator StartCoroutine(System.Func<System.Collections.IEnumerator> coroutineFunc)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && coroutineFunc != null)
        {
            var coroutine = coroutineFunc();
            (luaManager as MonoBehaviour).StartCoroutine(coroutine);
            return coroutine;
        }
        return null;
    }
    
    /// <summary>
    /// 启动协程（从Lua函数创建，带参数）
    /// </summary>
    /// <param name="coroutineFunc">协程函数</param>
    /// <param name="param">参数</param>
    /// <returns>协程对象</returns>
    public static System.Collections.IEnumerator StartCoroutineWithParam(System.Func<object, System.Collections.IEnumerator> coroutineFunc, object param)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && coroutineFunc != null)
        {
            var coroutine = coroutineFunc(param);
            (luaManager as MonoBehaviour).StartCoroutine(coroutine);
            return coroutine;
        }
        return null;
    }
    
    /// <summary>
    /// 启动协程（不返回IEnumerator，避免XLua配置问题）
    /// 直接启动协程并返回object类型的句柄
    /// </summary>
    /// <param name="coroutineFunc">协程函数（返回IEnumerator的函数）</param>
    /// <returns>协程句柄对象（用于停止）</returns>
    [XLua.LuaCallCSharp]
    public static object StartCoroutineWithoutReturn(System.Func<System.Collections.IEnumerator> coroutineFunc)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && coroutineFunc != null)
        {
            var coroutine = coroutineFunc();
            var mb = luaManager as MonoBehaviour;
            if (mb != null)
            {
                mb.StartCoroutine(coroutine);
                return coroutine;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="coroutine">协程对象</param>
    public static void StopCoroutine(System.Collections.IEnumerator coroutine)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && coroutine != null)
        {
            (luaManager as MonoBehaviour).StopCoroutine(coroutine);
        }
    }
    
    /// <summary>
    /// 停止协程（使用object类型，避免IEnumerator类型问题）
    /// </summary>
    /// <param name="coroutine">协程对象</param>
    [XLua.LuaCallCSharp]
    public static void StopCoroutineObject(object coroutine)
    {
        if (coroutine is System.Collections.IEnumerator enumerator)
        {
            StopCoroutine(enumerator);
        }
    }
    
    /// <summary>
    /// 停止所有协程
    /// </summary>
    public static void StopAllCoroutines()
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null)
        {
            (luaManager as MonoBehaviour).StopAllCoroutines();
        }
    }
    
    /// <summary>
    /// 创建等待秒数的协程对象
    /// </summary>
    /// <param name="seconds">等待秒数</param>
    /// <returns>WaitForSeconds对象</returns>
    public static UnityEngine.WaitForSeconds WaitForSeconds(float seconds)
    {
        return new UnityEngine.WaitForSeconds(seconds);
    }
    
    /// <summary>
    /// 创建等待帧的协程对象
    /// </summary>
    /// <param name="frames">等待帧数</param>
    /// <returns>WaitForSeconds对象（近似）</returns>
    public static UnityEngine.WaitForSeconds WaitForFrames(int frames)
    {
        // Unity没有WaitForFrames，我们用WaitForSeconds近似
        return new UnityEngine.WaitForSeconds(frames * Time.deltaTime);
    }
    
    /// <summary>
    /// 创建等待一帧的协程对象
    /// </summary>
    /// <returns>null（表示等待一帧）</returns>
    public static object WaitForEndOfFrame()
    {
        return new UnityEngine.WaitForEndOfFrame();
    }
    
    /// <summary>
    /// 创建等待固定更新的协程对象
    /// </summary>
    /// <returns>WaitForFixedUpdate对象</returns>
    public static UnityEngine.WaitForFixedUpdate WaitForFixedUpdate()
    {
        return new UnityEngine.WaitForFixedUpdate();
    }
    
    /// <summary>
    /// 等待指定秒数后执行回调
    /// </summary>
    /// <param name="seconds">等待秒数</param>
    /// <param name="callback">回调函数</param>
    public static void WaitSeconds(float seconds, System.Action callback)
    {
        DelayCall(callback, seconds);
    }
    
    /// <summary>
    /// 等待指定帧数后执行回调
    /// </summary>
    /// <param name="frames">等待帧数</param>
    /// <param name="callback">回调函数</param>
    public static void WaitFrames(int frames, System.Action callback)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && callback != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(WaitFramesCoroutine(frames, callback));
        }
    }
    
    /// <summary>
    /// 等待帧数协程
    /// </summary>
    private static IEnumerator WaitFramesCoroutine(int frames, System.Action callback)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        callback?.Invoke();
    }
    
    /// <summary>
    /// 等待条件满足后执行回调
    /// </summary>
    /// <param name="condition">条件函数（返回bool）</param>
    /// <param name="callback">回调函数</param>
    /// <param name="timeout">超时时间（秒，-1表示不超时）</param>
    public static void WaitUntil(System.Func<bool> condition, System.Action callback, float timeout = -1f)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && condition != null && callback != null)
        {
            (luaManager as MonoBehaviour).StartCoroutine(WaitUntilCoroutine(condition, callback, timeout));
        }
    }
    
    /// <summary>
    /// 等待条件协程
    /// </summary>
    private static IEnumerator WaitUntilCoroutine(System.Func<bool> condition, System.Action callback, float timeout)
    {
        float elapsed = 0f;
        while (!condition())
        {
            if (timeout > 0 && elapsed >= timeout)
            {
                Debug.LogWarning("[LuaHelper] WaitUntil超时");
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        callback?.Invoke();
    }
    
    /// <summary>
    /// 重复执行回调（类似InvokeRepeating）
    /// </summary>
    /// <param name="callback">回调函数</param>
    /// <param name="interval">间隔时间（秒）</param>
    /// <param name="repeatCount">重复次数（-1表示无限重复）</param>
    /// <returns>协程对象</returns>
    public static System.Collections.IEnumerator Repeat(System.Action callback, float interval, int repeatCount = -1)
    {
        var luaManager = LuaManager.Instance;
        if (luaManager != null && callback != null)
        {
            var coroutine = RepeatCoroutine(callback, interval, repeatCount);
            (luaManager as MonoBehaviour).StartCoroutine(coroutine);
            return coroutine;
        }
        return null;
    }
    
    /// <summary>
    /// 重复执行协程
    /// </summary>
    private static IEnumerator RepeatCoroutine(System.Action callback, float interval, int repeatCount)
    {
        int count = 0;
        while (repeatCount < 0 || count < repeatCount)
        {
            yield return new WaitForSeconds(interval);
            callback?.Invoke();
            count++;
        }
    }
    
    /// <summary>
    /// 停止重复执行
    /// </summary>
    /// <param name="coroutine">协程对象</param>
    public static void StopRepeat(System.Collections.IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }
    
    #endregion
}

