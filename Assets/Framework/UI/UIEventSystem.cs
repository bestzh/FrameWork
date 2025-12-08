using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI事件系统，用于处理UI之间的通信
    /// </summary>
    public class UIEventSystem : MonoBehaviour
    {
        private static UIEventSystem instance;
        public static UIEventSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIEventSystem>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UIEventSystem");
                        instance = go.AddComponent<UIEventSystem>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // 事件字典
        private Dictionary<string, List<Action<object>>> eventDictionary = new Dictionary<string, List<Action<object>>>();
        private Dictionary<string, List<Action>> eventDictionaryNoParam = new Dictionary<string, List<Action>>();
        
        // 泛型事件字典（用于精确注销）
        private Dictionary<string, Dictionary<object, Action<object>>> genericEventDictionary = new Dictionary<string, Dictionary<object, Action<object>>>();
        
        // 优先级事件字典（优先级 -> 回调列表）
        private Dictionary<string, SortedDictionary<int, List<Action>>> priorityEventDictionary = new Dictionary<string, SortedDictionary<int, List<Action>>>();
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        #region Event Registration
        
        /// <summary>
        /// 注册事件（带参数）
        /// </summary>
        public void RegisterEvent(string eventName, Action<object> callback)
        {
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = new List<Action<object>>();
            }
            
            if (!eventDictionary[eventName].Contains(callback))
            {
                eventDictionary[eventName].Add(callback);
            }
        }
        
        /// <summary>
        /// 注册事件（无参数）
        /// </summary>
        public void RegisterEvent(string eventName, Action callback)
        {
            if (!eventDictionaryNoParam.ContainsKey(eventName))
            {
                eventDictionaryNoParam[eventName] = new List<Action>();
            }
            
            if (!eventDictionaryNoParam[eventName].Contains(callback))
            {
                eventDictionaryNoParam[eventName].Add(callback);
            }
        }
        
        /// <summary>
        /// 注册泛型事件
        /// </summary>
        public void RegisterEvent<T>(string eventName, Action<T> callback)
        {
            // 创建包装器，用于精确注销
            Action<object> wrapper = (obj) => {
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
        
        #endregion
        
        #region Event Unregistration
        
        /// <summary>
        /// 注销事件（带参数）
        /// </summary>
        public void UnregisterEvent(string eventName, Action<object> callback)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName].Remove(callback);
                
                if (eventDictionary[eventName].Count == 0)
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }
        
        /// <summary>
        /// 注销事件（无参数）
        /// </summary>
        public void UnregisterEvent(string eventName, Action callback)
        {
            if (eventDictionaryNoParam.ContainsKey(eventName))
            {
                eventDictionaryNoParam[eventName].Remove(callback);
                
                if (eventDictionaryNoParam[eventName].Count == 0)
                {
                    eventDictionaryNoParam.Remove(eventName);
                }
            }
        }
        
        /// <summary>
        /// 注销泛型事件
        /// </summary>
        public void UnregisterEvent<T>(string eventName, Action<T> callback)
        {
            // 从泛型事件字典中查找包装器
            if (genericEventDictionary.ContainsKey(eventName) && 
                genericEventDictionary[eventName].ContainsKey(callback))
            {
                Action<object> wrapper = genericEventDictionary[eventName][callback];
                
                // 从事件字典中移除
                UnregisterEvent(eventName, wrapper);
                
                // 从泛型事件字典中移除
                genericEventDictionary[eventName].Remove(callback);
                if (genericEventDictionary[eventName].Count == 0)
                {
                    genericEventDictionary.Remove(eventName);
                }
            }
        }
        
        /// <summary>
        /// 注册一次性事件（触发后自动注销）
        /// </summary>
        public void RegisterEventOnce(string eventName, Action callback)
        {
            Action wrappedCallback = null;
            wrappedCallback = () => {
                callback?.Invoke();
                UnregisterEvent(eventName, wrappedCallback);
            };
            RegisterEvent(eventName, wrappedCallback);
        }
        
        /// <summary>
        /// 注册一次性泛型事件（触发后自动注销）
        /// </summary>
        public void RegisterEventOnce<T>(string eventName, Action<T> callback)
        {
            Action<T> wrappedCallback = null;
            wrappedCallback = (data) => {
                callback?.Invoke(data);
                UnregisterEvent(eventName, wrappedCallback);
            };
            RegisterEvent(eventName, wrappedCallback);
        }
        
        /// <summary>
        /// 注册带优先级的事件（优先级高的先执行，数字越大优先级越高）
        /// </summary>
        public void RegisterEventWithPriority(string eventName, Action callback, int priority = 0)
        {
            if (!priorityEventDictionary.ContainsKey(eventName))
            {
                priorityEventDictionary[eventName] = new SortedDictionary<int, List<Action>>(Comparer<int>.Create((a, b) => b.CompareTo(a))); // 降序排序
            }
            
            if (!priorityEventDictionary[eventName].ContainsKey(priority))
            {
                priorityEventDictionary[eventName][priority] = new List<Action>();
            }
            
            if (!priorityEventDictionary[eventName][priority].Contains(callback))
            {
                priorityEventDictionary[eventName][priority].Add(callback);
            }
        }
        
        #endregion
        
        #region Event Triggering
        
        /// <summary>
        /// 触发事件（带参数）
        /// </summary>
        public void TriggerEvent(string eventName, object data = null)
        {
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
                        Debug.LogError($"Error triggering event {eventName}: {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 触发事件（无参数）
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            // 先触发优先级事件（按优先级从高到低）
            if (priorityEventDictionary.ContainsKey(eventName))
            {
                foreach (var priorityGroup in priorityEventDictionary[eventName])
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
                            Debug.LogError($"Error triggering priority event {eventName} (priority {priorityGroup.Key}): {e.Message}");
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
                        Debug.LogError($"Error triggering event {eventName}: {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 触发泛型事件
        /// </summary>
        public void TriggerEvent<T>(string eventName, T data)
        {
            TriggerEvent(eventName, data);
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// 检查事件是否已注册
        /// </summary>
        public bool HasEvent(string eventName)
        {
            return eventDictionary.ContainsKey(eventName) || eventDictionaryNoParam.ContainsKey(eventName);
        }
        
        /// <summary>
        /// 获取事件监听器数量
        /// </summary>
        public int GetEventListenerCount(string eventName)
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
            
            return count;
        }
        
        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            eventDictionary.Clear();
            eventDictionaryNoParam.Clear();
            genericEventDictionary.Clear();
            priorityEventDictionary.Clear();
        }
        
        /// <summary>
        /// 清除指定事件
        /// </summary>
        public void ClearEvent(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary.Remove(eventName);
            }
            
            if (eventDictionaryNoParam.ContainsKey(eventName))
            {
                eventDictionaryNoParam.Remove(eventName);
            }
            
            // 清除泛型事件
            if (genericEventDictionary.ContainsKey(eventName))
            {
                genericEventDictionary.Remove(eventName);
            }
            
            // 清除优先级事件
            if (priorityEventDictionary.ContainsKey(eventName))
            {
                priorityEventDictionary.Remove(eventName);
            }
        }
        
        /// <summary>
        /// 获取所有已注册的事件名称
        /// </summary>
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
            
            return new List<string>(eventNames);
        }
        
        #endregion
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                ClearAllEvents();
            }
        }
    }
    
    /// <summary>
    /// UI事件常量定义
    /// </summary>
    public static class UIEventNames
    {
        // 通用事件
        public const string UI_SHOW = "UI_SHOW";
        public const string UI_HIDE = "UI_HIDE";
        public const string UI_LOAD = "UI_LOAD";
        public const string UI_UNLOAD = "UI_UNLOAD";
        
        // 游戏相关事件
        public const string GAME_START = "GAME_START";
        public const string GAME_PAUSE = "GAME_PAUSE";
        public const string GAME_RESUME = "GAME_RESUME";
        public const string GAME_OVER = "GAME_OVER";
        
        // 玩家相关事件
        public const string PLAYER_DEATH = "PLAYER_DEATH";
        public const string PLAYER_REVIVE = "PLAYER_REVIVE";
        public const string SCORE_CHANGED = "SCORE_CHANGED";
        
        // 系统相关事件
        public const string SETTINGS_CHANGED = "SETTINGS_CHANGED";
        public const string LANGUAGE_CHANGED = "LANGUAGE_CHANGED";
        public const string AUDIO_VOLUME_CHANGED = "AUDIO_VOLUME_CHANGED";
    }
} 