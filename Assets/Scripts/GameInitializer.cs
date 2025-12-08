using UnityEngine;
using Framework.ResourceLoader;
using System;
using XLua;

/// <summary>
/// 游戏初始化脚本 - 在场景启动时初始化AddressablesLoader
/// 将此脚本添加到场景中的任意GameObject上（建议添加到场景根节点或GameManager）
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("资源加载器配置")]
    [Tooltip("是否使用Addressables加载器（勾选后使用Addressables，否则使用Resources）")]
    public bool useAddressables = true;
    
    [Header("初始化顺序")]
    [Tooltip("执行顺序（数字越小越先执行，建议设置为-100）")]
    public int executionOrder = -100;
    
    /// <summary>
    /// 检查Addressables包是否可用（运行时检查）
    /// </summary>
    private bool IsAddressablesAvailable()
    {
        // 检查Addressables类型是否存在
        Type addressablesType = Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables");
        return addressablesType != null;
    }
    
    void Awake()
    {
        // GameInitializer是唯一控制资源加载器的地方
        // 确保ResManager已初始化
        var resManager = ResManager.Instance;
        
        // 根据配置选择资源加载器
        if (useAddressables)
        {
            if (IsAddressablesAvailable())
            {
                try
                {
                    ResManager.SetResourceLoader(new AddressablesLoader());
                    Debug.Log("[GameInitializer] ✓ 已切换到AddressablesLoader（这是唯一的加载器切换点）");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameInitializer] 切换到AddressablesLoader失败: {ex.Message}");
                    Debug.LogWarning("[GameInitializer] 保持使用默认ResourcesLoader");
                }
            }
            else
            {
                Debug.LogWarning("[GameInitializer] Addressables包未安装或不可用，使用默认ResourcesLoader");
                Debug.LogWarning("[GameInitializer] 请安装Addressables包: Window > Package Manager > 搜索 'Addressables' > Install");
            }
        }
        else
        {
            Debug.Log("[GameInitializer] 配置为使用ResourcesLoader（保持默认）");
        }
        
        Debug.Log($"[GameInitializer] 资源加载器初始化完成: {ResManager.GetResourceLoader().GetType().Name}");
    }
    
    void Start()
    {
        // 初始化Lua系统并启动主脚本
        StartLuaSystem();
        
        // 可以在这里添加其他初始化逻辑
        // 例如：加载启动配置、初始化UI等
    }
    
    /// <summary>
    /// 启动Lua系统
    /// </summary>
    private void StartLuaSystem()
    {
        try
        {
            // 确保LuaManager已初始化
            var luaManager = LuaManager.Instance;
            if (luaManager == null)
            {
                Debug.LogError("[GameInitializer] LuaManager初始化失败！");
                return;
            }
            
            Debug.Log("[GameInitializer] Lua系统已初始化");
            
            // 启动主Lua脚本
            // 注意：Lua脚本路径是相对于Resources/lua/的模块名
            // 例如：Resources/lua/LuaMain.lua.txt 对应模块名 "LuaMain"
            // 先加载LuaMain模块（即使没有return，也会执行脚本中的代码）
            var mainModule = luaManager.Require("LuaMain");
            
            // 尝试从全局获取Start函数（因为LuaMain.lua中Start是全局函数）
            var startFunc = luaManager.GetGlobal<XLua.LuaFunction>("Start");
            if (startFunc != null)
            {
                Debug.Log("[GameInitializer] ✓ Lua主脚本启动成功，调用Start函数");
                startFunc.Call();
                startFunc.Dispose();
            }
            else if (mainModule != null)
            {
                // 如果全局没有，尝试从模块中获取
                startFunc = mainModule.Get<XLua.LuaFunction>("Start");
                if (startFunc != null)
                {
                    Debug.Log("[GameInitializer] ✓ Lua主脚本启动成功，从模块调用Start函数");
                    startFunc.Call();
                    startFunc.Dispose();
                }
                else
                {
                    Debug.LogWarning("[GameInitializer] Lua主脚本已加载，但未找到Start函数");
                }
            }
            else
            {
                Debug.LogWarning("[GameInitializer] Lua主脚本未找到，请检查Resources/lua/LuaMain.lua.txt是否存在");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameInitializer] 启动Lua系统失败: {ex.Message}\n{ex.StackTrace}");
        }
    }
}

