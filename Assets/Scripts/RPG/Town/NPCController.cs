using UnityEngine;
using UnityEngine.UI;
using XLua;
using System;

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
    
    
    [Header("NPC信息（如果configID为0则使用这些值）")]
    [Tooltip("NPC名称")]
    public string npcName = "测试NPC";
    
    [Tooltip("对话文本")]
    [TextArea(3, 5)]
    public string dialogueText = "你好，冒险者！";
    
    [Header("交互设置")]
    [Tooltip("交互距离")]
    public float interactionDistance = 2f;
    
    [Tooltip("交互提示UI（在Inspector中指定，通常是NPC预制体的子对象）")]
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
        // 初始化交互提示UI（如果Inspector中已指定）
        if (interactionHint != null)
        {
            UpdateHintText();
            interactionHint.SetActive(false);
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
                Debug.LogWarning($"[NPCController] 未找到Player对象（Tag: Player），NPC: {npcName}");
            }
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
                text.text = $"按 {interactionKey} 交互";
            }
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
            Debug.LogWarning($"[NPCController] 无效的交互按键: {keyStr}，使用默认值E");
            interactionKey = KeyCode.E;
        }
    }
    
    /// <summary>
    /// 从Lua初始化NPC配置（供Lua脚本调用）
    /// 核心逻辑：C#负责所有配置的应用和验证
    /// </summary>
    /// <param name="id">配置ID</param>
    /// <param name="name">NPC名称</param>
    /// <param name="dialogue">对话文本</param>
    /// <param name="distance">交互距离</param>
    /// <param name="key">交互按键字符串</param>
    /// <param name="posX">X坐标</param>
    /// <param name="posY">Y坐标</param>
    /// <param name="posZ">Z坐标</param>
    /// <param name="rotY">Y轴旋转角度</param>
    public void InitializeFromLua(uint id, string name, string dialogue, float distance, string key, float posX, float posY, float posZ, float rotY)
    {
        // 核心逻辑：C#负责所有配置的应用和验证
        configID = id;
        
        // 应用配置
        npcName = name;
        dialogueText = dialogue;
        interactionDistance = distance;
        ParseInteractionKey(key);
        
        // 设置位置和旋转（核心逻辑在C#）
        transform.localPosition = new Vector3(posX, posY, posZ);
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
        
        Debug.Log($"[NPCController] 已从Lua初始化配置 ID={id}, Name={name}, Position=({posX}, {posY}, {posZ}), RotationY={rotY}");
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
        // 延迟初始化玩家对象
        EnsurePlayerInitialized();
        if (player == null) return;
        
        // 计算玩家距离
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance;
        
        // 只在状态改变时更新UI（性能优化）
        if (wasNearby != isPlayerNearby && interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby && !isInteracting);
        }
        
        // 检测交互输入
        if (isPlayerNearby && !isInteracting && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
        
        // 更新提示位置（只在玩家附近时更新，减少计算）
        if (isPlayerNearby && interactionHint != null && interactionHint.activeSelf)
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
        
        // 显示对话（使用对话UI系统）
        ShowDialogue();
        
        // 注意：交互状态重置现在在对话关闭回调中处理
    }
    
    /// <summary>
    /// 显示对话
    /// </summary>
    void ShowDialogue()
    {
        // 直接调用Lua的对话管理器
        try
        {
            var luaManager = LuaManager.Instance;
            if (luaManager != null && luaManager.LuaEnv != null)
            {
                // 加载Lua模块
                var dialogueManager = luaManager.Require("rpg.dialogue_manager");
                if (dialogueManager != null)
                {
                    var showDialogueFunc = dialogueManager.Get<XLua.LuaFunction>("ShowDialogue");
                    if (showDialogueFunc != null)
                    {
                        // 调用Lua函数
                        showDialogueFunc.Call(
                            npcName,
                            dialogueText,
                            null, // 头像路径（可选）
                            (System.Action)(() =>
                            {
                                // 对话关闭后的回调
                                ResetInteraction();
                            })
                        );
                        showDialogueFunc.Dispose();
                        return;
                    }
                    dialogueManager.Dispose();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NPCController] 调用Lua对话管理器失败: {e.Message}");
        }
        
        // 后备方案：使用Debug输出
        Debug.LogWarning($"[NPCController] 无法调用Lua对话管理器，使用Debug输出");
        Debug.Log($"=== {npcName} ===");
        Debug.Log(dialogueText);
        // 延迟重置交互状态
        Invoke(nameof(ResetInteraction), 0.5f);
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

