using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 传送门控制器 - 管理传送门的交互和场景切换功能
/// 数据通过Lua脚本的InitializeFromLua方法初始化
/// </summary>
[XLua.LuaCallCSharp]
public class PortalController : MonoBehaviour
{
    [Header("配置方式")]
    [Tooltip("配置ID（用于标识，数据通过Lua的InitializeFromLua方法传入）")]
    public uint configID = 0;
    
    [Header("传送门信息（Inspector中的值，Lua初始化时会覆盖）")]
    [Tooltip("传送门名称")]
    public string portalName = "传送门";
    
    [Tooltip("目标场景名称（必须在Build Settings中）")]
    public string targetSceneName = "";
    
    [Tooltip("传送门描述")]
    [TextArea(2, 4)]
    public string portalDescription = "传送到另一个场景";
    
    [Header("交互设置")]
    [Tooltip("交互距离")]
    public float interactionDistance = 3f;
    
    [Tooltip("交互提示UI（在Inspector中指定，通常是Portal预制体的子对象）")]
    public GameObject interactionHint;
    
    [Tooltip("交互按键（默认E键）")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("传送设置")]
    [Tooltip("传送延迟（秒）")]
    public float teleportDelay = 0.5f;
    
    [Tooltip("是否显示加载提示")]
    public bool showLoadingHint = true;
    
    [Header("显示设置")]
    [Tooltip("是否显示交互范围（Scene视图中）")]
    public bool showGizmos = true;
    
    [Header("特效（可选）")]
    [Tooltip("传送门特效对象")]
    public GameObject portalEffect;
    
    private GameObject player;
    private bool isPlayerNearby = false;
    private bool isTeleporting = false;
    private Camera targetCamera;
    
    void Start()
    {
        // 初始化交互提示UI（如果Inspector中已指定）
        if (interactionHint != null)
        {
            UpdateHintText();
            interactionHint.SetActive(false);
        }
        
        // 验证场景名称（仅在非Lua初始化时检查）
        if (configID == 0 && string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[PortalController] 传送门 {portalName} 未设置目标场景名称！");
        }
    }
    
    /// <summary>
    /// 更新提示文本内容（显示交互按键）
    /// </summary>
    void UpdateHintText()
    {
        if (interactionHint == null) return;
        
        // 查找HintText子对象
        Transform hintTextTransform = interactionHint.transform.Find("HintText");
        if (hintTextTransform != null)
        {
            UnityEngine.UI.Text text = hintTextTransform.GetComponent<UnityEngine.UI.Text>();
            if (text != null)
            {
                text.text = $"按 {interactionKey} 进入 {portalName}";
            }
        }
    }
    
    /// <summary>
    /// 延迟初始化玩家对象（在需要时调用）
    /// </summary>
    void EnsurePlayerInitialized()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // 不输出警告，因为玩家可能在稍后才创建
                return;
            }
            Debug.Log($"[PortalController] 已找到玩家对象，传送门: {portalName}");
        }
    }
    
    /// <summary>
    /// 延迟初始化相机（在需要时调用）
    /// </summary>
    void EnsureCameraInitialized()
    {
        if (targetCamera == null)
        {
            targetCamera = GetMainCamera();
        }
    }
    
    /// <summary>
    /// 解析交互按键字符串
    /// </summary>
    void ParseInteractionKey(string keyStr)
    {
        if (string.IsNullOrEmpty(keyStr)) return;
        
        if (System.Enum.TryParse<KeyCode>(keyStr, out KeyCode keyCode))
        {
            interactionKey = keyCode;
        }
        else
        {
            Debug.LogWarning($"[PortalController] 无效的交互按键: {keyStr}，使用默认值E");
            interactionKey = KeyCode.E;
        }
    }
    
    /// <summary>
    /// 从Lua初始化传送门配置（供Lua脚本调用）
    /// 核心逻辑：C#负责所有配置的应用和验证
    /// </summary>
    /// <param name="id">配置ID</param>
    /// <param name="name">传送门名称</param>
    /// <param name="targetScene">目标场景名称</param>
    /// <param name="description">传送门描述</param>
    /// <param name="distance">交互距离</param>
    /// <param name="delay">传送延迟</param>
    /// <param name="key">交互按键字符串</param>
    /// <param name="posX">X坐标</param>
    /// <param name="posY">Y坐标</param>
    /// <param name="posZ">Z坐标</param>
    /// <param name="rotY">Y轴旋转角度</param>
    public void InitializeFromLua(uint id, string name, string targetScene, string description, float distance, float delay, string key, float posX, float posY, float posZ, float rotY)
    {
        // 核心逻辑：C#负责所有配置的应用和验证
        configID = id;
        
        // 应用配置
        portalName = name;
        targetSceneName = targetScene;
        portalDescription = description;
        interactionDistance = distance;
        teleportDelay = delay;
        ParseInteractionKey(key);
        
        // 设置位置和旋转（核心逻辑在C#）
        transform.localPosition = new Vector3(posX, posY, posZ);
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
        
        // 更新提示文本（如果UI已存在）
        if (interactionHint != null)
        {
            UpdateHintText();
        }
        
        Debug.Log($"[PortalController] 已从Lua初始化配置 ID={id}, Name={name}, TargetScene={targetScene}, Position=({posX}, {posY}, {posZ}), RotationY={rotY}");
    }
    
    
    /// <summary>
    /// 获取主相机（支持Cinemachine）
    /// </summary>
    Camera GetMainCamera()
    {
        // 优先查找CinemachineBrain（Cinemachine的主组件）
        var cinemachineBrain = FindFirstObjectByType<Unity.Cinemachine.CinemachineBrain>();
        if (cinemachineBrain != null && cinemachineBrain.OutputCamera != null)
        {
            return cinemachineBrain.OutputCamera;
        }
        
        // 如果没有Cinemachine，使用Camera.main
        if (Camera.main != null)
        {
            return Camera.main;
        }
        
        // 最后尝试查找任何激活的相机
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                return cam;
            }
        }
        
        return null;
    }
    
    void Update()
    {
        // 延迟初始化玩家和相机
        EnsurePlayerInitialized();
        EnsureCameraInitialized();
        
        if (player == null || isTeleporting) return;
        
        // 计算玩家距离
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance;
        
        // 只在状态改变时更新UI（性能优化）
        if (wasNearby != isPlayerNearby && interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby && !isTeleporting);
        }
        
        // 检测交互输入
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            Teleport();
        }
        
        // 更新提示位置（跟随传送门）
        if (interactionHint != null && isPlayerNearby && interactionHint.activeSelf)
        {
            UpdateHintPosition();
        }
    }
    
    
    /// <summary>
    /// 更新提示位置（始终面向相机）
    /// </summary>
    void UpdateHintPosition()
    {
        if (interactionHint == null) return;
        
        // 重新获取相机（以防相机在运行时改变）
        if (targetCamera == null || !targetCamera.gameObject.activeInHierarchy)
        {
            targetCamera = GetMainCamera();
        }
        
        if (targetCamera == null) return;
        
        // 让提示始终面向相机
        Vector3 directionToCamera = targetCamera.transform.position - interactionHint.transform.position;
        directionToCamera.y = 0; // 保持水平
        if (directionToCamera != Vector3.zero)
        {
            interactionHint.transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
        
        // 将提示放在传送门上方
        interactionHint.transform.position = transform.position + Vector3.up * 2.5f;
    }
    
    /// <summary>
    /// 传送玩家
    /// </summary>
    public void Teleport()
    {
        if (isTeleporting) return;
        
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"[PortalController] 传送门 {portalName} 未设置目标场景名称！");
            return;
        }
        
        isTeleporting = true;
        
        Debug.Log($"[PortalController] 开始传送: {portalName} -> {targetSceneName}");
        
        // 发送传送事件
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent("Portal_Enter", this);
        }
        
        // 延迟传送（可以播放特效）
        Invoke(nameof(LoadTargetScene), teleportDelay);
    }
    
    /// <summary>
    /// 加载目标场景
    /// </summary>
    void LoadTargetScene()
    {
        // 获取场景管理器
        GameSceneManager sceneManager = GameSceneManager.Instance;
        
        if (sceneManager == null)
        {
            Debug.LogError("[PortalController] 未找到GameSceneManager！");
            isTeleporting = false;
            return;
        }
        
        // 显示加载提示
        if (showLoadingHint)
        {
            Debug.Log($"[PortalController] 正在加载场景: {targetSceneName}...");
        }
        
        // 加载场景（异步）
        sceneManager.LoadSceneAsync(
            targetSceneName,
            onProgress: (progress) =>
            {
                if (showLoadingHint)
                {
                    Debug.Log($"[PortalController] 加载进度: {progress * 100:F1}%");
                }
            },
            onComplete: () =>
            {
                Debug.Log($"[PortalController] ✓ 场景加载完成: {targetSceneName}");
                isTeleporting = false;
            }
        );
    }
    
    /// <summary>
    /// 在Scene视图中显示交互范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // 绘制朝向
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
    
    /// <summary>
    /// 检查玩家是否在交互范围内
    /// </summary>
    public bool IsPlayerInRange()
    {
        return isPlayerNearby;
    }
    
    /// <summary>
    /// 获取玩家距离
    /// </summary>
    public float GetPlayerDistance()
    {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, player.transform.position);
    }
    
    /// <summary>
    /// 设置目标场景
    /// </summary>
    public void SetTargetScene(string sceneName)
    {
        targetSceneName = sceneName;
    }
}

