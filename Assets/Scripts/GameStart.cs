using UnityEngine;
using System.Collections;
using Framework.ResourceLoader;

/// <summary>
/// 游戏启动脚本 - 统一管理游戏启动流程和系统初始化
/// 将此脚本添加到启动场景的GameObject上（建议命名为GameManager）
/// 执行顺序建议设置为-200（确保在所有其他脚本之前执行）
/// </summary>
public class GameStart : MonoBehaviour
{
    [Header("启动配置")]
    [Tooltip("是否在启动时自动初始化所有系统")]
    public bool autoInitialize = true;
    
    [Tooltip("启动后加载的初始场景名称（为空则保持在当前场景）")]
    public string initialSceneName = "";
    
    [Tooltip("是否显示初始化日志")]
    public bool showInitLogs = true;
    
    [Header("初始化顺序设置")]
    [Tooltip("执行顺序（数字越小越先执行，建议设置为-200）")]
    public int executionOrder = -200;
    
    /// <summary>
    /// 游戏是否已启动
    /// </summary>
    public static bool IsGameStarted { get; private set; } = false;
    
    /// <summary>
    /// 初始化进度（0-1）
    /// </summary>
    public static float InitProgress { get; private set; } = 0f;
    
    private static GameStart m_instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static GameStart Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<GameStart>();
                if (m_instance == null)
                {
                    GameObject go = new GameObject("GameStart");
                    m_instance = go.AddComponent<GameStart>();
                    DontDestroyOnLoad(go);
                }
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
            
            if (showInitLogs)
            {
                Debug.Log("[GameStart] ========== 游戏启动脚本初始化 ==========");
            }
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 如果启用自动初始化，在Start中执行
        if (!autoInitialize)
        {
            if (showInitLogs)
            {
                Debug.Log("[GameStart] 自动初始化已禁用，请手动调用InitializeGame()");
            }
        }
    }
    
    void Start()
    {
        if (autoInitialize && !IsGameStarted)
        {
            StartCoroutine(InitializeGameCoroutine());
        }
    }
    
    /// <summary>
    /// 初始化游戏（协程版本，支持进度回调）
    /// </summary>
    public IEnumerator InitializeGameCoroutine()
    {
        if (IsGameStarted)
        {
            if (showInitLogs)
            {
                Debug.LogWarning("[GameStart] 游戏已经启动，跳过初始化");
            }
            yield break;
        }
        
        InitProgress = 0f;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ========== 开始游戏初始化流程 ==========");
        }
        
        // 步骤1: 初始化资源管理器（必须最先初始化）
        yield return StartCoroutine(InitializeResourceManager());
        InitProgress = 0.1f;
        
        // 步骤2: 初始化事件系统（其他系统可能需要使用事件）
        yield return StartCoroutine(InitializeEventSystem());
        InitProgress = 0.2f;
        
        // 步骤3: 初始化表格系统
        yield return StartCoroutine(InitializeTableSystem());
        InitProgress = 0.4f;
        
        // 步骤4: 初始化音频系统
        yield return StartCoroutine(InitializeAudioSystem());
        InitProgress = 0.5f;
        
        // 步骤5: 初始化场景管理器
        yield return StartCoroutine(InitializeSceneManager());
        InitProgress = 0.6f;
        
        // 步骤6: 初始化Lua系统（最后初始化，因为可能依赖其他系统）
        yield return StartCoroutine(InitializeLuaSystem());
        InitProgress = 0.8f;
        
        // 步骤7: 加载初始场景（如果需要）
        if (!string.IsNullOrEmpty(initialSceneName))
        {
            yield return StartCoroutine(LoadInitialScene());
            InitProgress = 0.9f;
        }
        
        // 步骤8: 完成初始化
        CompleteInitialization();
        InitProgress = 1.0f;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ========== 游戏初始化完成 ==========");
        }
    }
    
    /// <summary>
    /// 初始化资源管理器
    /// </summary>
    private IEnumerator InitializeResourceManager()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [1/8] 初始化资源管理器...");
        }
        
        // 确保ResManager已初始化
        var resManager = ResManager.Instance;
        
        // 检查是否有GameInitializer（它负责设置资源加载器）
        var initializer = FindFirstObjectByType<GameInitializer>();
        if (initializer == null)
        {
            if (showInitLogs)
            {
                Debug.LogWarning("[GameStart] 未找到GameInitializer，使用默认资源加载器");
            }
        }
        
        yield return null; // 等待一帧，确保GameInitializer的Awake执行完成
        
        if (showInitLogs)
        {
            Debug.Log($"[GameStart] ✓ 资源管理器初始化完成: {ResManager.GetResourceLoader().GetType().Name}");
        }
    }
    
    /// <summary>
    /// 初始化事件系统
    /// </summary>
    private IEnumerator InitializeEventSystem()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [2/8] 初始化事件系统...");
        }
        
        // 确保EventManager已初始化
        var eventManager = EventManager.Instance;
        
        yield return null;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ✓ 事件系统初始化完成");
        }
    }
    
    /// <summary>
    /// 初始化表格系统
    /// </summary>
    private IEnumerator InitializeTableSystem()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [3/8] 初始化表格系统...");
        }
        
        // 确保TableManager已初始化并加载表格
        var tableManager = Table.TableManager.Instance;
        if (tableManager != null)
        {
            tableManager.Load();
        }
        
        yield return null;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ✓ 表格系统初始化完成");
        }
    }
    
    /// <summary>
    /// 初始化音频系统
    /// </summary>
    private IEnumerator InitializeAudioSystem()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [4/8] 初始化音频系统...");
        }
        
        // 确保AudioManager已初始化
        var audioManager = AudioManager.Instance;
        
        yield return null;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ✓ 音频系统初始化完成");
        }
    }
    
    /// <summary>
    /// 初始化场景管理器
    /// </summary>
    private IEnumerator InitializeSceneManager()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [5/8] 初始化场景管理器...");
        }
        
        // 确保GameSceneManager已初始化
        var sceneManager = GameSceneManager.Instance;
        
        yield return null;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ✓ 场景管理器初始化完成");
        }
    }
    
    /// <summary>
    /// 初始化Lua系统
    /// </summary>
    private IEnumerator InitializeLuaSystem()
    {
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [6/8] 初始化Lua系统...");
        }
        
        // 确保LuaManager已初始化
        var luaManager = LuaManager.Instance;
        if (luaManager == null)
        {
            Debug.LogError("[GameStart] LuaManager初始化失败！");
            yield break;
        }
        
        // 检查是否有GameInitializer来处理Lua启动
        var initializer = FindFirstObjectByType<GameInitializer>();
        if (initializer == null)
        {
            if (showInitLogs)
            {
                Debug.LogWarning("[GameStart] 未找到GameInitializer，Lua系统可能未启动");
            }
        }
        
        yield return null;
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] ✓ Lua系统初始化完成");
        }
    }
    
    /// <summary>
    /// 加载初始场景
    /// </summary>
    private IEnumerator LoadInitialScene()
    {
        if (showInitLogs)
        {
            Debug.Log($"[GameStart] [7/8] 加载初始场景: {initialSceneName}...");
        }
        
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            bool loadComplete = false;
            
            sceneManager.LoadSceneAsync(
                initialSceneName,
                onProgress: (progress) =>
                {
                    if (showInitLogs)
                    {
                        Debug.Log($"[GameStart] 场景加载进度: {progress * 100:F1}%");
                    }
                },
                onComplete: () =>
                {
                    loadComplete = true;
                    if (showInitLogs)
                    {
                        Debug.Log($"[GameStart] ✓ 初始场景加载完成: {initialSceneName}");
                    }
                }
            );
            
            // 等待场景加载完成
            while (!loadComplete)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("[GameStart] GameSceneManager未初始化，无法加载初始场景");
        }
    }
    
    /// <summary>
    /// 完成初始化
    /// </summary>
    private void CompleteInitialization()
    {
        IsGameStarted = true;
        InitProgress = 1.0f;
        
        // 触发游戏开始事件
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(GlobalEventNames.GAME_START);
        }
        
        if (showInitLogs)
        {
            Debug.Log("[GameStart] [8/8] ✓ 游戏启动完成！");
        }
    }
    
    /// <summary>
    /// 手动初始化游戏（同步版本）
    /// </summary>
    public void InitializeGame()
    {
        if (IsGameStarted)
        {
            Debug.LogWarning("[GameStart] 游戏已经启动，跳过初始化");
            return;
        }
        
        StartCoroutine(InitializeGameCoroutine());
    }
    
    /// <summary>
    /// 重启游戏
    /// </summary>
    public void RestartGame()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(GlobalEventNames.GAME_RESTART);
        }
        
        IsGameStarted = false;
        InitProgress = 0f;
        
        // 重新加载当前场景
        var sceneManager = GameSceneManager.Instance;
        if (sceneManager != null)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneManager.LoadScene(currentScene);
        }
        
        // 重新初始化
        StartCoroutine(InitializeGameCoroutine());
    }
    
    void OnDestroy()
    {
        if (m_instance == this)
        {
            IsGameStarted = false;
            InitProgress = 0f;
        }
    }
}

