using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 传送门控制器 - 管理传送门的交互和场景切换功能
/// 支持从表格配置读取数据
/// </summary>
[XLua.LuaCallCSharp]
public class PortalController : MonoBehaviour
{
    [Header("配置方式")]
    [Tooltip("配置ID（0表示使用Inspector中的值，>0表示从表格读取）")]
    public uint configID = 0;
    
    [Tooltip("是否使用表格中的位置（如果为true，会覆盖当前GameObject的位置）")]
    public bool useTablePosition = true;
    
    [Header("传送门信息（如果configID为0则使用这些值）")]
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
    
    [Tooltip("交互提示UI（可选，会自动创建）")]
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
        // 从表格加载配置（如果configID有效）
        LoadConfigFromTable();
        
        // 查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"[PortalController] 未找到Player对象（Tag: Player），传送门: {portalName}");
        }
        
        // 获取相机引用
        targetCamera = GetMainCamera();
        
        // 创建交互提示（如果还没有）
        if (interactionHint == null)
        {
            CreateInteractionHint();
        }
        else
        {
            interactionHint.SetActive(false);
        }
        
        // 验证场景名称
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[PortalController] 传送门 {portalName} 未设置目标场景名称！");
        }
    }
    
    /// <summary>
    /// 从表格加载配置
    /// </summary>
    void LoadConfigFromTable()
    {
        if (configID == 0) return; // 使用Inspector中的值
        
        // 确保表格已加载
        if (Table.TableManager.Instance != null)
        {
            Table.TableManager.Instance.Load();
        }
        
        // 尝试从表格读取配置
        try
        {
            if (Table.PortalConfig.Contains(configID))
            {
                var config = Table.PortalConfig.Get(configID);
                portalName = config.Name;
                targetSceneName = config.TargetSceneName;
                portalDescription = config.Description;
                
                // 解析字符串类型的 float 值
                if (!string.IsNullOrEmpty(config.InteractionDistance))
                {
                    if (float.TryParse(config.InteractionDistance, out float distance))
                    {
                        interactionDistance = distance;
                    }
                    else
                    {
                        Debug.LogWarning($"[PortalController] 无效的交互距离: {config.InteractionDistance}，使用默认值");
                    }
                }
                
                if (!string.IsNullOrEmpty(config.TeleportDelay))
                {
                    if (float.TryParse(config.TeleportDelay, out float delay))
                    {
                        teleportDelay = delay;
                    }
                    else
                    {
                        Debug.LogWarning($"[PortalController] 无效的传送延迟: {config.TeleportDelay}，使用默认值");
                    }
                }
                
                // 解析交互按键字符串
                if (!string.IsNullOrEmpty(config.InteractionKey))
                {
                    if (System.Enum.TryParse<KeyCode>(config.InteractionKey, out KeyCode key))
                    {
                        interactionKey = key;
                    }
                    else
                    {
                        Debug.LogWarning($"[PortalController] 无效的交互按键: {config.InteractionKey}，使用默认值E");
                    }
                }
                
                // 设置位置（如果启用）
                if (useTablePosition)
                {
                    float posX = 0f, posY = 0f, posZ = 0f, rotY = 0f;
                    
                    if (!string.IsNullOrEmpty(config.PositionX))
                        float.TryParse(config.PositionX, out posX);
                    if (!string.IsNullOrEmpty(config.PositionY))
                        float.TryParse(config.PositionY, out posY);
                    if (!string.IsNullOrEmpty(config.PositionZ))
                        float.TryParse(config.PositionZ, out posZ);
                    if (!string.IsNullOrEmpty(config.RotationY))
                        float.TryParse(config.RotationY, out rotY);
                    
                    transform.position = new Vector3(posX, posY, posZ);
                    transform.rotation = Quaternion.Euler(0, rotY, 0);
                }
                
                Debug.Log($"[PortalController] 已从表格加载配置 ID={configID}, Name={portalName}, TargetScene={targetSceneName}, Position=({config.PositionX}, {config.PositionY}, {config.PositionZ})");
            }
            else
            {
                Debug.LogWarning($"[PortalController] 表格中未找到配置ID={configID}，使用Inspector中的值");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PortalController] 加载表格配置失败: {e.Message}，使用Inspector中的值");
        }
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
        if (player == null || isTeleporting) return;
        
        // 计算玩家距离
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= interactionDistance;
        
        // 显示/隐藏交互提示
        if (interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby);
        }
        
        // 检测交互输入
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            Teleport();
        }
        
        // 更新提示位置（跟随传送门）
        if (interactionHint != null && isPlayerNearby)
        {
            UpdateHintPosition();
        }
    }
    
    /// <summary>
    /// 创建交互提示UI
    /// </summary>
    void CreateInteractionHint()
    {
        // 创建一个简单的Canvas作为提示
        GameObject canvasObj = new GameObject("InteractionHint");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = targetCamera != null ? targetCamera : Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // 创建文本
        GameObject textObj = new GameObject("HintText");
        textObj.transform.SetParent(canvasObj.transform);
        textObj.transform.localPosition = new Vector3(0, 2.5f, 0);
        textObj.transform.localScale = Vector3.one * 0.01f;
        
        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = $"按 {interactionKey} 进入 {portalName}";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.color = Color.cyan;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 添加背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(textObj.transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;
        
        UnityEngine.UI.Image bg = bgObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0.5f, 0.5f, 0.7f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = new Vector2(20, 20);
        
        interactionHint = canvasObj;
        interactionHint.SetActive(false);
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

