using UnityEngine;
using System;
using System.Collections;
using System.IO;
using XLua;
using Framework.ResourceLoader;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Lua管理器 - 统一管理XLua的LuaEnv生命周期
/// </summary>
public class LuaManager : MonoBehaviour
{
    private static LuaManager m_instance;
    
    /// <summary>
    /// Lua环境实例
    /// </summary>
    private LuaEnv luaEnv;
    
    /// <summary>
    /// GC间隔时间（秒）
    /// </summary>
    private const float GCInterval = 1f;
    
    /// <summary>
    /// 上次GC时间
    /// </summary>
    private float lastGCTime = 0;
    
    /// <summary>
    /// Lua脚本加载器（使用统一的资源加载框架）
    /// </summary>
    private LuaLoader luaLoader;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static LuaManager Instance
    {
        get
        {
            if (m_instance == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("LuaManager只能在运行时使用！");
                    return null;
                }
#endif
                GameObject obj = new GameObject("LuaManager");
                GameObject.DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<LuaManager>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 获取Lua环境
    /// </summary>
    public LuaEnv LuaEnv
    {
        get
        {
            if (luaEnv == null)
            {
                Initialize();
            }
            return luaEnv;
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
    /// 初始化Lua环境
    /// </summary>
    private void Initialize()
    {
        if (luaEnv != null)
        {
            return;
        }
        
        luaEnv = new LuaEnv();
        
        // 设置自定义加载器
        luaEnv.AddLoader(CustomLoader);
        
        // 初始化Lua加载器（使用统一的资源加载框架）
        luaLoader = new LuaLoader();
        
        // 注册C#类到Lua全局表
        RegisterCSharpClasses();
        
        Debug.Log("LuaManager初始化完成");
    }
    
    
    /// <summary>
    /// 自定义Lua加载器 - 使用统一的资源加载框架
    /// </summary>
    private byte[] CustomLoader(ref string filepath)
    {
        if (luaLoader == null)
        {
            Debug.LogError("LuaLoader未初始化！");
            return null;
        }
        
        // 使用统一的LuaLoader加载
        byte[] bytes = luaLoader.LoadLuaBytes(filepath);
        
        if (bytes != null)
        {
            // 设置文件路径用于调试
            string path = "lua/" + filepath.Replace('.', '/') + ".lua";
            filepath = Path.Combine(Application.dataPath, "Resources", path);
            return bytes;
        }
        
        Debug.LogWarning($"未找到Lua脚本: {filepath}");
        return null;
    }
    
    /// <summary>
    /// 注册C#类到Lua全局表
    /// </summary>
    private void RegisterCSharpClasses()
    {
        // 注册Unity常用类
        luaEnv.Global.Set("UnityEngine", typeof(UnityEngine.Object));
        luaEnv.Global.Set("GameObject", typeof(GameObject));
        luaEnv.Global.Set("Transform", typeof(Transform));
        luaEnv.Global.Set("Vector3", typeof(Vector3));
        luaEnv.Global.Set("Vector2", typeof(Vector2));
        luaEnv.Global.Set("Quaternion", typeof(Quaternion));
        luaEnv.Global.Set("Time", typeof(Time));
        luaEnv.Global.Set("Debug", typeof(Debug));
        
        // 注册项目中的管理器
        luaEnv.Global.Set("ResManager", typeof(ResManager));
        luaEnv.Global.Set("LuaManager", typeof(LuaManager));
        
        // 注册UI框架
        luaEnv.Global.Set("UIManager", typeof(UI.UIManager));
        luaEnv.Global.Set("UIBase", typeof(UI.UIBase));
        
        // 注册资源加载框架
        luaEnv.Global.Set("IResourceLoader", typeof(Framework.ResourceLoader.IResourceLoader));
        luaEnv.Global.Set("ResourcesLoader", typeof(Framework.ResourceLoader.ResourcesLoader));
        
        // 注册Lua辅助类
        luaEnv.Global.Set("LuaHelper", typeof(LuaHelper));
        
        // 注册音频管理器
        luaEnv.Global.Set("AudioManager", typeof(AudioManager));
        
        // 注册数据存储管理器
        luaEnv.Global.Set("SaveManager", typeof(SaveManager));
        
        // 注册对象池管理器
        luaEnv.Global.Set("ObjectPool", typeof(ObjectPool));
        luaEnv.Global.Set("PoolInfo", typeof(PoolInfo));
        luaEnv.Global.Set("IPoolable", typeof(IPoolable));
        
        // 注册全局事件管理器
        luaEnv.Global.Set("EventManager", typeof(EventManager));
        luaEnv.Global.Set("GlobalEventNames", typeof(GlobalEventNames));
        
        // 注册RPG系统
        luaEnv.Global.Set("CharacterManager", typeof(CharacterManager));
        luaEnv.Global.Set("CharacterData", typeof(CharacterData));
        luaEnv.Global.Set("BattleManager", typeof(BattleManager));
        luaEnv.Global.Set("InventoryManager", typeof(InventoryManager));
        luaEnv.Global.Set("QuestManager", typeof(QuestManager));
        luaEnv.Global.Set("PlayerController", typeof(PlayerController));
        luaEnv.Global.Set("EnemyController", typeof(EnemyController));
        luaEnv.Global.Set("SkillSystem", typeof(SkillSystem));
        luaEnv.Global.Set("SkillData", typeof(SkillData));
        luaEnv.Global.Set("ComboManager", typeof(ComboManager));
    }
    
    /// <summary>
    /// 执行Lua代码字符串
    /// </summary>
    public object[] DoString(string chunk, string chunkName = "chunk")
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化！");
            return null;
        }
        
        try
        {
            return luaEnv.DoString(chunk, chunkName);
        }
        catch (Exception e)
        {
            Debug.LogError($"执行Lua代码失败: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
    
    /// <summary>
    /// 加载Lua文件（通过require）
    /// </summary>
    public LuaTable Require(string moduleName)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化！");
            return null;
        }
        
        try
        {
            object[] result = luaEnv.DoString($"return require('{moduleName}')", moduleName);
            if (result != null && result.Length > 0)
            {
                return result[0] as LuaTable;
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载Lua模块失败: {moduleName}\n{e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 获取Lua全局表
    /// </summary>
    public LuaTable GetGlobal()
    {
        return luaEnv?.Global;
    }
    
    /// <summary>
    /// 获取Lua全局变量
    /// </summary>
    public T GetGlobal<T>(string name)
    {
        if (luaEnv == null)
        {
            return default(T);
        }
        
        return luaEnv.Global.Get<T>(name);
    }
    
    /// <summary>
    /// 设置Lua全局变量
    /// </summary>
    public void SetGlobal<T>(string name, T value)
    {
        if (luaEnv == null)
        {
            Debug.LogError("LuaEnv未初始化！");
            return;
        }
        
        luaEnv.Global.Set(name, value);
    }
    
    void Update()
    {
        if (luaEnv != null)
        {
            // 定期执行GC
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }
    }
    
    void OnDestroy()
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"释放LuaEnv失败: {e.Message}");
            }
        }
        
        if (m_instance == this)
        {
            m_instance = null;
        }
    }
    
    void OnApplicationQuit()
    {
        // 在释放LuaEnv之前，先强制清理所有Lua驱动的UI
        // 注意：OnApplicationQuit时，UI可能已经被隐藏，但GameObject和事件监听器可能还在
        CleanupAllLuaUIs();
        
        // 强制触发GC，确保所有OnDestroy都被调用
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        
        // 等待一帧，让所有OnDestroy完成（但这在OnApplicationQuit时可能不工作）
        // 所以我们需要再次清理
        CleanupAllLuaUIs();
        
        if (luaEnv != null)
        {
            try
            {
                // 再次尝试清理（保险措施）
                CleanupAllLuaUIs();
                
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"释放LuaEnv失败: {e.Message}");
                // 如果还是失败，尝试强制清理所有按钮事件
                ForceCleanupAllButtons();
            }
        }
    }
    
    /// <summary>
    /// 清理所有Lua驱动的UI（释放前必须清理所有C#回调）
    /// </summary>
    private void CleanupAllLuaUIs()
    {
        int cleanedCount = 0;
        
        // 使用Resources.FindObjectsOfTypeAll查找所有按钮（包括非激活和隐藏的对象）
        // 这在OnApplicationQuit时更可靠，因为FindObjectsByType可能找不到已隐藏的对象
        UnityEngine.UI.Button[] allButtons = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>();
        foreach (var btn in allButtons)
        {
            // 跳过预制体资源（只处理场景中的对象）
            if (btn != null && btn.onClick != null && btn.gameObject.scene.IsValid())
            {
                try
                {
                    btn.onClick.RemoveAllListeners();
                    cleanedCount++;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[LuaManager] 清理按钮事件失败: {e.Message}");
                }
            }
        }
        
        // 也尝试查找LuaUIBase组件
        UI.LuaUIBase[] allLuaUIs = Resources.FindObjectsOfTypeAll<UI.LuaUIBase>();
        int luaUICount = 0;
        foreach (var luaUI in allLuaUIs)
        {
            if (luaUI != null && luaUI.gameObject.scene.IsValid())
            {
                luaUICount++;
            }
        }
        
        Debug.Log($"[LuaManager] 已清理 {cleanedCount} 个按钮的事件监听器（找到Lua UI: {luaUICount} 个，总按钮: {allButtons.Length} 个）");
    }
    
    /// <summary>
    /// 强制清理所有按钮事件（使用反射等底层方法）
    /// </summary>
    private void ForceCleanupAllButtons()
    {
        try
        {
            // 尝试通过Resources.FindObjectsOfTypeAll查找（包括非激活对象）
            UnityEngine.UI.Button[] allButtons = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>();
            int cleanedCount = 0;
            foreach (var btn in allButtons)
            {
                if (btn != null && btn.onClick != null)
                {
                    try
                    {
                        btn.onClick.RemoveAllListeners();
                        cleanedCount++;
                    }
                    catch { }
                }
            }
            Debug.Log($"[LuaManager] 强制清理了 {cleanedCount} 个按钮的事件监听器");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LuaManager] 强制清理按钮事件失败: {e.Message}");
        }
    }
}

