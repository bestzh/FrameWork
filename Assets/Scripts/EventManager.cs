using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 全局事件管理器 - 用于处理游戏全局事件通信
/// 支持事件订阅/发布、优先级、泛型事件等功能
/// </summary>
[XLua.LuaCallCSharp]
public class EventManager : MonoBehaviour
{
    private static EventManager m_instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static EventManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<EventManager>();
                if (m_instance == null)
                {
                    GameObject go = new GameObject("EventManager");
                    m_instance = go.AddComponent<EventManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return m_instance;
        }
    }
    
    // 事件字典（带参数）
    private Dictionary<string, List<Action<object>>> eventDictionary = new Dictionary<string, List<Action<object>>>();
    
    // 事件字典（无参数）
    private Dictionary<string, List<Action>> eventDictionaryNoParam = new Dictionary<string, List<Action>>();
    
    // 泛型事件字典（用于精确注销）
    private Dictionary<string, Dictionary<object, Action<object>>> genericEventDictionary = new Dictionary<string, Dictionary<object, Action<object>>>();
    
    // 优先级事件字典（优先级 -> 回调列表）
    private Dictionary<string, SortedDictionary<int, List<Action>>> priorityEventDictionary = new Dictionary<string, SortedDictionary<int, List<Action>>>();
    
    // 优先级事件字典（带参数）
    private Dictionary<string, SortedDictionary<int, List<Action<object>>>> priorityEventDictionaryWithParam = new Dictionary<string, SortedDictionary<int, List<Action<object>>>>();
    
    [Header("事件系统设置")]
    [SerializeField] private bool logEvents = false;  // 是否记录事件日志
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    #region 事件注册
    
    /// <summary>
    /// 注册事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void RegisterEvent(string eventName, Action<object> callback)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        if (callback == null)
        {
            Debug.LogWarning($"[EventManager] 事件回调不能为空: {eventName}");
            return;
        }
        
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = new List<Action<object>>();
        }
        
        if (!eventDictionary[eventName].Contains(callback))
        {
            eventDictionary[eventName].Add(callback);
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] ✓ 注册事件: {eventName}");
            }
        }
    }
    
    /// <summary>
    /// 注册事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void RegisterEvent(string eventName, Action callback)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        if (callback == null)
        {
            Debug.LogWarning($"[EventManager] 事件回调不能为空: {eventName}");
            return;
        }
        
        if (!eventDictionaryNoParam.ContainsKey(eventName))
        {
            eventDictionaryNoParam[eventName] = new List<Action>();
        }
        
        if (!eventDictionaryNoParam[eventName].Contains(callback))
        {
            eventDictionaryNoParam[eventName].Add(callback);
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] ✓ 注册事件: {eventName}");
            }
        }
    }
    
    /// <summary>
    /// 注册泛型事件
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void RegisterEvent<T>(string eventName, Action<T> callback)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        if (callback == null)
        {
            Debug.LogWarning($"[EventManager] 事件回调不能为空: {eventName}");
            return;
        }
        
        // 创建包装器，用于精确注销
        Action<object> wrapper = (obj) =>
        {
            if (obj is T typedObj)
            {
                callback(typedObj);
            }
        };
        
        // 存储包装器引用，用于注销
        if (!genericEventDictionary.ContainsKey(eventName))
        {
            genericEventDictionary[eventName] = new Dictionary<object, Action<object>>();
        }
        genericEventDictionary[eventName][callback] = wrapper;
        
        // 注册事件
        RegisterEvent(eventName, wrapper);
    }
    
    /// <summary>
    /// 注册优先级事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级（数字越大优先级越高）</param>
    public void RegisterEvent(string eventName, Action callback, int priority)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        if (callback == null)
        {
            Debug.LogWarning($"[EventManager] 事件回调不能为空: {eventName}");
            return;
        }
        
        if (!priorityEventDictionary.ContainsKey(eventName))
        {
            priorityEventDictionary[eventName] = new SortedDictionary<int, List<Action>>();
        }
        
        if (!priorityEventDictionary[eventName].ContainsKey(priority))
        {
            priorityEventDictionary[eventName][priority] = new List<Action>();
        }
        
        if (!priorityEventDictionary[eventName][priority].Contains(callback))
        {
            priorityEventDictionary[eventName][priority].Add(callback);
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] ✓ 注册优先级事件: {eventName} (优先级: {priority})");
            }
        }
    }
    
    /// <summary>
    /// 注册优先级事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级（数字越大优先级越高）</param>
    public void RegisterEvent(string eventName, Action<object> callback, int priority)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        if (callback == null)
        {
            Debug.LogWarning($"[EventManager] 事件回调不能为空: {eventName}");
            return;
        }
        
        if (!priorityEventDictionaryWithParam.ContainsKey(eventName))
        {
            priorityEventDictionaryWithParam[eventName] = new SortedDictionary<int, List<Action<object>>>();
        }
        
        if (!priorityEventDictionaryWithParam[eventName].ContainsKey(priority))
        {
            priorityEventDictionaryWithParam[eventName][priority] = new List<Action<object>>();
        }
        
        if (!priorityEventDictionaryWithParam[eventName][priority].Contains(callback))
        {
            priorityEventDictionaryWithParam[eventName][priority].Add(callback);
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] ✓ 注册优先级事件: {eventName} (优先级: {priority})");
            }
        }
    }
    
    #endregion
    
    #region 事件注销
    
    /// <summary>
    /// 注销事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void UnregisterEvent(string eventName, Action<object> callback)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName].Remove(callback);
            
            if (eventDictionary[eventName].Count == 0)
            {
                eventDictionary.Remove(eventName);
            }
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] 注销事件: {eventName}");
            }
        }
    }
    
    /// <summary>
    /// 注销事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void UnregisterEvent(string eventName, Action callback)
    {
        if (eventDictionaryNoParam.ContainsKey(eventName))
        {
            eventDictionaryNoParam[eventName].Remove(callback);
            
            if (eventDictionaryNoParam[eventName].Count == 0)
            {
                eventDictionaryNoParam.Remove(eventName);
            }
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] 注销事件: {eventName}");
            }
        }
    }
    
    /// <summary>
    /// 注销泛型事件
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void UnregisterEvent<T>(string eventName, Action<T> callback)
    {
        if (genericEventDictionary.ContainsKey(eventName) && 
            genericEventDictionary[eventName].ContainsKey(callback))
        {
            Action<object> wrapper = genericEventDictionary[eventName][callback];
            UnregisterEvent(eventName, wrapper);
            genericEventDictionary[eventName].Remove(callback);
            
            if (genericEventDictionary[eventName].Count == 0)
            {
                genericEventDictionary.Remove(eventName);
            }
        }
    }
    
    /// <summary>
    /// 注销优先级事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级</param>
    public void UnregisterEvent(string eventName, Action callback, int priority)
    {
        if (priorityEventDictionary.ContainsKey(eventName) &&
            priorityEventDictionary[eventName].ContainsKey(priority))
        {
            priorityEventDictionary[eventName][priority].Remove(callback);
            
            if (priorityEventDictionary[eventName][priority].Count == 0)
            {
                priorityEventDictionary[eventName].Remove(priority);
            }
            
            if (priorityEventDictionary[eventName].Count == 0)
            {
                priorityEventDictionary.Remove(eventName);
            }
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] 注销优先级事件: {eventName} (优先级: {priority})");
            }
        }
    }
    
    /// <summary>
    /// 注销优先级事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    /// <param name="priority">优先级</param>
    public void UnregisterEvent(string eventName, Action<object> callback, int priority)
    {
        if (priorityEventDictionaryWithParam.ContainsKey(eventName) &&
            priorityEventDictionaryWithParam[eventName].ContainsKey(priority))
        {
            priorityEventDictionaryWithParam[eventName][priority].Remove(callback);
            
            if (priorityEventDictionaryWithParam[eventName][priority].Count == 0)
            {
                priorityEventDictionaryWithParam[eventName].Remove(priority);
            }
            
            if (priorityEventDictionaryWithParam[eventName].Count == 0)
            {
                priorityEventDictionaryWithParam.Remove(eventName);
            }
            
            if (logEvents)
            {
                Debug.Log($"[EventManager] 注销优先级事件: {eventName} (优先级: {priority})");
            }
        }
    }
    
    /// <summary>
    /// 注销所有指定事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void UnregisterAll(string eventName)
    {
        bool removed = false;
        
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary.Remove(eventName);
            removed = true;
        }
        
        if (eventDictionaryNoParam.ContainsKey(eventName))
        {
            eventDictionaryNoParam.Remove(eventName);
            removed = true;
        }
        
        if (genericEventDictionary.ContainsKey(eventName))
        {
            genericEventDictionary.Remove(eventName);
            removed = true;
        }
        
        if (priorityEventDictionary.ContainsKey(eventName))
        {
            priorityEventDictionary.Remove(eventName);
            removed = true;
        }
        
        if (priorityEventDictionaryWithParam.ContainsKey(eventName))
        {
            priorityEventDictionaryWithParam.Remove(eventName);
            removed = true;
        }
        
        if (removed && logEvents)
        {
            Debug.Log($"[EventManager] 注销所有事件: {eventName}");
        }
    }
    
    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void ClearAllEvents()
    {
        eventDictionary.Clear();
        eventDictionaryNoParam.Clear();
        genericEventDictionary.Clear();
        priorityEventDictionary.Clear();
        priorityEventDictionaryWithParam.Clear();
        
        if (logEvents)
        {
            Debug.Log("[EventManager] 已清空所有事件");
        }
    }
    
    #endregion
    
    #region 事件触发
    
    /// <summary>
    /// 触发事件（带参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="data">事件数据</param>
    public void TriggerEvent(string eventName, object data = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        // 先触发优先级事件（按优先级从高到低）
        if (priorityEventDictionaryWithParam.ContainsKey(eventName))
        {
            foreach (var priorityGroup in priorityEventDictionaryWithParam[eventName].Reverse())
            {
                var callbacks = priorityGroup.Value.ToArray();
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback?.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[EventManager] 触发优先级事件失败: {eventName} (优先级 {priorityGroup.Key}): {e.Message}\n{e.StackTrace}");
                    }
                }
            }
        }
        
        // 再触发普通事件
        if (eventDictionary.ContainsKey(eventName))
        {
            var callbacks = eventDictionary[eventName].ToArray();
            foreach (var callback in callbacks)
            {
                try
                {
                    callback?.Invoke(data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventManager] 触发事件失败: {eventName}: {e.Message}\n{e.StackTrace}");
                }
            }
        }
        
        if (logEvents)
        {
            Debug.Log($"[EventManager] 触发事件: {eventName}");
        }
    }
    
    /// <summary>
    /// 触发事件（无参数）
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void TriggerEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("[EventManager] 事件名称不能为空");
            return;
        }
        
        // 先触发优先级事件（按优先级从高到低）
        if (priorityEventDictionary.ContainsKey(eventName))
        {
            foreach (var priorityGroup in priorityEventDictionary[eventName].Reverse())
            {
                var callbacks = priorityGroup.Value.ToArray();
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[EventManager] 触发优先级事件失败: {eventName} (优先级 {priorityGroup.Key}): {e.Message}\n{e.StackTrace}");
                    }
                }
            }
        }
        
        // 再触发普通事件
        if (eventDictionaryNoParam.ContainsKey(eventName))
        {
            var callbacks = eventDictionaryNoParam[eventName].ToArray();
            foreach (var callback in callbacks)
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventManager] 触发事件失败: {eventName}: {e.Message}\n{e.StackTrace}");
                }
            }
        }
        
        if (logEvents)
        {
            Debug.Log($"[EventManager] 触发事件: {eventName}");
        }
    }
    
    /// <summary>
    /// 触发泛型事件
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="data">事件数据</param>
    public void TriggerEvent<T>(string eventName, T data)
    {
        TriggerEvent(eventName, data);
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 检查事件是否已注册
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>是否已注册</returns>
    public bool HasEvent(string eventName)
    {
        return eventDictionary.ContainsKey(eventName) || 
               eventDictionaryNoParam.ContainsKey(eventName) ||
               priorityEventDictionary.ContainsKey(eventName) ||
               priorityEventDictionaryWithParam.ContainsKey(eventName);
    }
    
    /// <summary>
    /// 获取事件监听者数量
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>监听者数量</returns>
    public int GetListenerCount(string eventName)
    {
        int count = 0;
        
        if (eventDictionary.ContainsKey(eventName))
        {
            count += eventDictionary[eventName].Count;
        }
        
        if (eventDictionaryNoParam.ContainsKey(eventName))
        {
            count += eventDictionaryNoParam[eventName].Count;
        }
        
        if (priorityEventDictionary.ContainsKey(eventName))
        {
            foreach (var list in priorityEventDictionary[eventName].Values)
            {
                count += list.Count;
            }
        }
        
        if (priorityEventDictionaryWithParam.ContainsKey(eventName))
        {
            foreach (var list in priorityEventDictionaryWithParam[eventName].Values)
            {
                count += list.Count;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// 获取所有已注册的事件名称
    /// </summary>
    /// <returns>事件名称列表</returns>
    public List<string> GetAllEventNames()
    {
        HashSet<string> eventNames = new HashSet<string>();
        
        foreach (var key in eventDictionary.Keys)
        {
            eventNames.Add(key);
        }
        
        foreach (var key in eventDictionaryNoParam.Keys)
        {
            eventNames.Add(key);
        }
        
        foreach (var key in priorityEventDictionary.Keys)
        {
            eventNames.Add(key);
        }
        
        foreach (var key in priorityEventDictionaryWithParam.Keys)
        {
            eventNames.Add(key);
        }
        
        return new List<string>(eventNames);
    }
    
    /// <summary>
    /// 设置是否记录事件日志
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public void SetLogEvents(bool enabled)
    {
        logEvents = enabled;
    }
    
    #endregion
    
    void OnDestroy()
    {
        if (m_instance == this)
        {
            ClearAllEvents();
        }
    }
}

/// <summary>
/// 全局事件常量定义
/// </summary>
public static class GlobalEventNames
{
    // 游戏生命周期事件
    public const string GAME_START = "GAME_START";
    public const string GAME_PAUSE = "GAME_PAUSE";
    public const string GAME_RESUME = "GAME_RESUME";
    public const string GAME_OVER = "GAME_OVER";
    public const string GAME_RESTART = "GAME_RESTART";
    
    // 场景事件
    public const string SCENE_LOAD_START = "SCENE_LOAD_START";
    public const string SCENE_LOAD_COMPLETE = "SCENE_LOAD_COMPLETE";
    public const string SCENE_UNLOAD_START = "SCENE_UNLOAD_START";
    public const string SCENE_UNLOAD_COMPLETE = "SCENE_UNLOAD_COMPLETE";
    
    // 玩家事件
    public const string PLAYER_SPAWN = "PLAYER_SPAWN";
    public const string PLAYER_DEATH = "PLAYER_DEATH";
    public const string PLAYER_REVIVE = "PLAYER_REVIVE";
    public const string PLAYER_LEVEL_UP = "PLAYER_LEVEL_UP";
    public const string PLAYER_EXP_CHANGED = "PLAYER_EXP_CHANGED";
    public const string PLAYER_HP_CHANGED = "PLAYER_HP_CHANGED";
    public const string PLAYER_MP_CHANGED = "PLAYER_MP_CHANGED";
    
    // 系统事件
    public const string SETTINGS_CHANGED = "SETTINGS_CHANGED";
    public const string LANGUAGE_CHANGED = "LANGUAGE_CHANGED";
    public const string AUDIO_VOLUME_CHANGED = "AUDIO_VOLUME_CHANGED";
    public const string NETWORK_CONNECTED = "NETWORK_CONNECTED";
    public const string NETWORK_DISCONNECTED = "NETWORK_DISCONNECTED";
    
    // 资源事件
    public const string RESOURCE_LOAD_START = "RESOURCE_LOAD_START";
    public const string RESOURCE_LOAD_COMPLETE = "RESOURCE_LOAD_COMPLETE";
    public const string RESOURCE_LOAD_FAILED = "RESOURCE_LOAD_FAILED";
    
    // 热更新事件
    public const string HOTUPDATE_START = "HOTUPDATE_START";
    public const string HOTUPDATE_PROGRESS = "HOTUPDATE_PROGRESS";
    public const string HOTUPDATE_COMPLETE = "HOTUPDATE_COMPLETE";
    public const string HOTUPDATE_FAILED = "HOTUPDATE_FAILED";
}

