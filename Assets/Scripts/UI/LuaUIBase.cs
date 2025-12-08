using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XLua;

namespace UI
{
    /// <summary>
    /// Lua驱动的UI基类 - 支持热更新
    /// 所有Lua控制的UI都使用这个类，无需为每个UI创建单独的C#脚本
    /// </summary>
    public class LuaUIBase : UIBase
    {
        /// <summary>
        /// Lua回调函数表
        /// </summary>
        private LuaTable luaCallbacks;
        
        /// <summary>
        /// 跟踪所有绑定了Lua回调的按钮（需要在销毁时清理）
        /// </summary>
        private List<Button> trackedButtons = new List<Button>();
        
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
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            CallLuaFunction("OnInitialize", this);
        }
        
        public override void Show()
        {
            base.Show();
            CallLuaFunction("OnShow", this);
        }
        
        public override void Hide()
        {
            base.Hide();
            CallLuaFunction("OnHide", this);
        }
        
        /// <summary>
        /// Unity生命周期：Update（每帧调用）
        /// </summary>
        void Update()
        {
            CallLuaFunction("Update", this);
        }
        
        /// <summary>
        /// Unity生命周期：FixedUpdate（固定时间间隔调用，用于物理更新）
        /// </summary>
        void FixedUpdate()
        {
            CallLuaFunction("FixedUpdate", this);
        }
        
        /// <summary>
        /// Unity生命周期：LateUpdate（每帧最后调用）
        /// </summary>
        void LateUpdate()
        {
            CallLuaFunction("LateUpdate", this);
        }
        
        /// <summary>
        /// Unity生命周期：OnEnable（对象激活时调用）
        /// </summary>
        void OnEnable()
        {
            CallLuaFunction("OnEnable", this);
        }
        
        /// <summary>
        /// Unity生命周期：OnDisable（对象禁用时调用）
        /// </summary>
        void OnDisable()
        {
            CallLuaFunction("OnDisable", this);
        }
        
        /// <summary>
        /// 清理所有按钮事件监听器（防止LuaEnv释放时报错）
        /// </summary>
        private void ClearAllButtonListeners()
        {
            // 清理所有跟踪的按钮
            foreach (var btn in trackedButtons)
            {
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                }
            }
            trackedButtons.Clear();
            
            // 清理UI下所有按钮的事件（保险措施）
            Button[] allButtons = GetComponentsInChildren<Button>(true);
            foreach (var btn in allButtons)
            {
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                }
            }
        }
        
        protected override void OnDestroy()
        {
            // 先清理所有按钮事件监听器（必须在释放Lua回调之前）
            // 这是最关键的步骤：清理所有C#委托引用，避免LuaEnv释放时报错
            ClearAllButtonListeners();
            
            // 调用Lua的OnDestroy回调（此时LuaEnv还存在，可以调用）
            try
            {
                CallLuaFunction("OnDestroy", this);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[LuaUIBase] 调用OnDestroy回调失败: {e.Message}");
            }
            
            // 释放Lua回调（必须在清理按钮事件之后）
            if (luaCallbacks != null)
            {
                try
                {
                    luaCallbacks.Dispose();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[LuaUIBase] 释放Lua回调失败: {e.Message}");
                }
                luaCallbacks = null;
            }
            
            base.OnDestroy();
        }
        
        /// <summary>
        /// 注册按钮（用于跟踪，方便清理）
        /// 注意：这个方法主要是为了跟踪，实际绑定在Lua中完成
        /// </summary>
        public void RegisterButton(Button button)
        {
            if (button != null && !trackedButtons.Contains(button))
            {
                trackedButtons.Add(button);
            }
        }
    }
}

