using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// NPC控制器 - 管理NPC的交互功能
/// 支持对话、显示交互提示等功能
/// 支持从表格配置读取数据
/// </summary>
[XLua.LuaCallCSharp]
public class NPCController : MonoBehaviour
{
    [Header("配置方式")]
    [Tooltip("配置ID（0表示使用Inspector中的值，>0表示从表格读取）")]
    public uint configID = 0;
    
    [Tooltip("是否使用表格中的位置（如果为true，会覆盖当前GameObject的位置）")]
    public bool useTablePosition = true;
    
    [Header("NPC信息（如果configID为0则使用这些值）")]
    [Tooltip("NPC名称")]
    public string npcName = "测试NPC";
    
    [Tooltip("对话文本")]
    [TextArea(3, 5)]
    public string dialogueText = "你好，冒险者！";
    
    [Header("交互设置")]
    [Tooltip("交互距离")]
    public float interactionDistance = 2f;
    
    [Tooltip("交互提示UI（可选，会自动创建）")]
    public GameObject interactionHint;
    
    [Tooltip("交互按键（默认E键）")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("显示设置")]
    [Tooltip("是否显示交互范围（Scene视图中）")]
    public bool showGizmos = true;
    
    private GameObject player;
    private bool isPlayerNearby = false;
    private bool isInteracting = false;
    private Camera targetCamera;
    
    void Start()
    {
        // 从表格加载配置（如果configID有效）
        LoadConfigFromTable();
        
        // 查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"[NPCController] 未找到Player对象（Tag: Player），NPC: {npcName}");
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
            if (Table.NPCConfig.Contains(configID))
            {
                var config = Table.NPCConfig.Get(configID);
                npcName = config.Name;
                dialogueText = config.DialogueText;
                interactionDistance = float.Parse(config.InteractionDistance);
                
                // 解析交互按键字符串
                if (!string.IsNullOrEmpty(config.InteractionKey))
                {
                    if (System.Enum.TryParse<KeyCode>(config.InteractionKey, out KeyCode key))
                    {
                        interactionKey = key;
                    }
                    else
                    {
                        Debug.LogWarning($"[NPCController] 无效的交互按键: {config.InteractionKey}，使用默认值E");
                    }
                }
                
                // 设置位置（如果启用）
                if (useTablePosition)
                {
                    transform.position = new Vector3(float.Parse(config.PositionX), float.Parse(config.PositionY), float.Parse(config.PositionZ));
                    transform.rotation = Quaternion.Euler(0, float.Parse(config.RotationY), 0);
                }
                
                Debug.Log($"[NPCController] 已从表格加载配置 ID={configID}, Name={npcName}, Position=({config.PositionX}, {config.PositionY}, {config.PositionZ})");
            }
            else
            {
                Debug.LogWarning($"[NPCController] 表格中未找到配置ID={configID}，使用Inspector中的值");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NPCController] 加载表格配置失败: {e.Message}，使用Inspector中的值");
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
        if (player == null) return;
        
        // 计算玩家距离
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance;
        
        // 显示/隐藏交互提示
        if (interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby && !isInteracting);
        }
        
        // 检测交互输入
        if (isPlayerNearby && !isInteracting && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
        
        // 更新提示位置（跟随NPC）
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
        textObj.transform.localPosition = new Vector3(0, 2f, 0);
        textObj.transform.localScale = Vector3.one * 0.01f;
        
        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = $"按 {interactionKey} 交互";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 添加背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(textObj.transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;
        
        UnityEngine.UI.Image bg = bgObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
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
        
        // 将提示放在NPC上方
        interactionHint.transform.position = transform.position + Vector3.up * 2f;
    }
    
    /// <summary>
    /// 与NPC交互
    /// </summary>
    public void Interact()
    {
        if (isInteracting) return;
        
        isInteracting = true;
        
        Debug.Log($"[NPC] {npcName}: {dialogueText}");
        
        // 发送交互事件
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent("NPC_Interact", this);
        }
        
        // 显示对话（简单版：使用Debug.Log，后续可以集成UI系统）
        ShowDialogue();
        
        // 重置交互状态（可以改为等待对话关闭）
        Invoke(nameof(ResetInteraction), 0.5f);
    }
    
    /// <summary>
    /// 显示对话
    /// </summary>
    void ShowDialogue()
    {
        // TODO: 集成UI系统显示对话界面
        // 目前使用简单的Debug输出
        Debug.Log($"=== {npcName} ===");
        Debug.Log(dialogueText);
    }
    
    /// <summary>
    /// 重置交互状态
    /// </summary>
    void ResetInteraction()
    {
        isInteracting = false;
    }
    
    /// <summary>
    /// 在Scene视图中显示交互范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // 绘制朝向
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 1f);
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
}

