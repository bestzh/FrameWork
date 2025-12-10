using UnityEngine;
using XLua;

/// <summary>
/// Lua驱动的行为基类 - 支持热更新
/// 所有Lua控制的GameObject行为都使用这个类，核心逻辑在Lua中实现
/// </summary>
[XLua.LuaCallCSharp]
public class LuaBehaviourBase : MonoBehaviour
{
    /// <summary>
    /// Lua回调函数表
    /// </summary>
    private LuaTable luaCallbacks;
    
    /// <summary>
    /// 设置Lua回调函数
    /// </summary>
    public void SetLuaCallbacks(LuaTable callbacks)
    {
        luaCallbacks = callbacks;
    }
    
    /// <summary>
    /// 调用Lua函数
    /// </summary>
    private void CallLuaFunction(string functionName, params object[] args)
    {
        if (luaCallbacks != null)
        {
            var func = luaCallbacks.Get<LuaFunction>(functionName);
            if (func != null)
            {
                try
                {
                    func.Call(args);
                }
                finally
                {
                    func.Dispose();
                }
            }
        }
    }
    
    void Start()
    {
        CallLuaFunction("Start", this);
    }
    
    void Update()
    {
        CallLuaFunction("Update", this);
    }
    
    void FixedUpdate()
    {
        CallLuaFunction("FixedUpdate", this);
    }
    
    void LateUpdate()
    {
        CallLuaFunction("LateUpdate", this);
    }
    
    void OnEnable()
    {
        CallLuaFunction("OnEnable", this);
    }
    
    void OnDisable()
    {
        CallLuaFunction("OnDisable", this);
    }
    
    void OnDestroy()
    {
        CallLuaFunction("OnDestroy", this);
        if (luaCallbacks != null)
        {
            luaCallbacks.Dispose();
            luaCallbacks = null;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        CallLuaFunction("OnDrawGizmosSelected", this);
    }
}

