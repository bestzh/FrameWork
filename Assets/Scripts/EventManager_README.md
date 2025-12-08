# 全局事件系统使用指南

## 概述

`EventManager` 是一个全局事件管理器，用于处理游戏中的全局事件通信。它支持事件订阅/发布、优先级、泛型事件等功能。

## 功能特性

- ✅ 事件订阅/发布
- ✅ 支持无参数和带参数事件
- ✅ 支持泛型事件（类型安全）
- ✅ 支持优先级事件（数字越大优先级越高）
- ✅ 完善的错误处理
- ✅ Lua 封装支持
- ✅ 事件日志记录（可选）

## C# 使用示例

### 基本使用

```csharp
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // 注册事件（无参数）
        EventManager.Instance.RegisterEvent(GlobalEventNames.GAME_START, OnGameStart);
        
        // 注册事件（带参数）
        EventManager.Instance.RegisterEvent(GlobalEventNames.PLAYER_HP_CHANGED, OnPlayerHPChanged);
        
        // 触发事件（无参数）
        EventManager.Instance.TriggerEvent(GlobalEventNames.GAME_START);
        
        // 触发事件（带参数）
        EventManager.Instance.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, 100);
    }
    
    void OnGameStart()
    {
        Debug.Log("游戏开始！");
    }
    
    void OnPlayerHPChanged(object data)
    {
        int hp = (int)data;
        Debug.Log($"玩家血量变化: {hp}");
    }
    
    void OnDestroy()
    {
        // 注销事件
        EventManager.Instance.UnregisterEvent(GlobalEventNames.GAME_START, OnGameStart);
        EventManager.Instance.UnregisterEvent(GlobalEventNames.PLAYER_HP_CHANGED, OnPlayerHPChanged);
    }
}
```

### 优先级事件

```csharp
// 注册优先级事件（优先级越高越先执行）
EventManager.Instance.RegisterEvent(GlobalEventNames.GAME_START, OnGameStartHigh, 100);
EventManager.Instance.RegisterEvent(GlobalEventNames.GAME_START, OnGameStartLow, 0);

// 触发时，OnGameStartHigh 会先执行
EventManager.Instance.TriggerEvent(GlobalEventNames.GAME_START);
```

### 泛型事件（类型安全）

```csharp
// 注册泛型事件
EventManager.Instance.RegisterEvent<int>(GlobalEventNames.PLAYER_HP_CHANGED, OnPlayerHPChangedTyped);

void OnPlayerHPChangedTyped(int hp)
{
    Debug.Log($"玩家血量: {hp}");
}

// 触发泛型事件
EventManager.Instance.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, 100);
```

### 注销所有事件

```csharp
// 注销指定事件的所有监听者
EventManager.Instance.UnregisterAll(GlobalEventNames.GAME_START);

// 清空所有事件
EventManager.Instance.ClearAllEvents();
```

## Lua 使用示例

### 基本使用

```lua
-- 加载事件辅助模块
local EventHelper = require("event_helper")

-- 注册事件（无参数）
EventHelper.Register(EventHelper.Events.GAME_START, function()
    print("游戏开始！")
end)

-- 注册事件（带参数）
EventHelper.RegisterWithData(EventHelper.Events.PLAYER_HP_CHANGED, function(data)
    print("玩家血量变化: " .. tostring(data))
end)

-- 触发事件（无参数）
EventHelper.Trigger(EventHelper.Events.GAME_START)

-- 触发事件（带参数）
EventHelper.TriggerWithData(EventHelper.Events.PLAYER_HP_CHANGED, 100)
```

### 优先级事件

```lua
-- 注册优先级事件
local callback1 = function()
    print("高优先级回调")
end

local callback2 = function()
    print("低优先级回调")
end

EventHelper.RegisterWithPriority(EventHelper.Events.GAME_START, callback1, 100)
EventHelper.RegisterWithPriority(EventHelper.Events.GAME_START, callback2, 0)

-- 触发时，callback1 会先执行
EventHelper.Trigger(EventHelper.Events.GAME_START)
```

### 一次性注册（自动注销）

```lua
-- 注册事件并获取注销函数
local unregister = EventHelper.RegisterOnce(EventHelper.Events.GAME_START, function()
    print("游戏开始！")
end)

-- 稍后注销
unregister()
```

### 注销事件

```lua
local callback = function()
    print("游戏开始！")
end

-- 注册
EventHelper.Register(EventHelper.Events.GAME_START, callback)

-- 注销
EventHelper.Unregister(EventHelper.Events.GAME_START, callback)

-- 或者注销所有指定事件
EventHelper.UnregisterAll(EventHelper.Events.GAME_START)
```

## 事件常量

系统预定义了一些常用事件常量，位于 `GlobalEventNames` 类中：

### 游戏生命周期事件
- `GAME_START` - 游戏开始
- `GAME_PAUSE` - 游戏暂停
- `GAME_RESUME` - 游戏恢复
- `GAME_OVER` - 游戏结束
- `GAME_RESTART` - 游戏重启

### 场景事件
- `SCENE_LOAD_START` - 场景加载开始
- `SCENE_LOAD_COMPLETE` - 场景加载完成
- `SCENE_UNLOAD_START` - 场景卸载开始
- `SCENE_UNLOAD_COMPLETE` - 场景卸载完成

### 玩家事件
- `PLAYER_SPAWN` - 玩家生成
- `PLAYER_DEATH` - 玩家死亡
- `PLAYER_REVIVE` - 玩家复活
- `PLAYER_LEVEL_UP` - 玩家升级
- `PLAYER_EXP_CHANGED` - 玩家经验变化
- `PLAYER_HP_CHANGED` - 玩家血量变化
- `PLAYER_MP_CHANGED` - 玩家魔法值变化

### 系统事件
- `SETTINGS_CHANGED` - 设置改变
- `LANGUAGE_CHANGED` - 语言改变
- `AUDIO_VOLUME_CHANGED` - 音频音量改变
- `NETWORK_CONNECTED` - 网络连接
- `NETWORK_DISCONNECTED` - 网络断开

### 资源事件
- `RESOURCE_LOAD_START` - 资源加载开始
- `RESOURCE_LOAD_COMPLETE` - 资源加载完成
- `RESOURCE_LOAD_FAILED` - 资源加载失败

### 热更新事件
- `HOTUPDATE_START` - 热更新开始
- `HOTUPDATE_PROGRESS` - 热更新进度
- `HOTUPDATE_COMPLETE` - 热更新完成
- `HOTUPDATE_FAILED` - 热更新失败

## 注意事项

1. **内存泄漏**：记得在对象销毁时注销事件，避免内存泄漏
2. **事件名称**：建议使用预定义的事件常量，避免拼写错误
3. **优先级**：优先级数字越大越先执行，相同优先级按注册顺序执行
4. **线程安全**：事件系统不是线程安全的，请在主线程中使用
5. **性能**：事件系统使用字典和列表存储，性能良好，但避免在频繁调用的函数中注册/注销事件

## 与 UIEventSystem 的区别

- `EventManager`：全局事件系统，用于游戏逻辑层面的通信
- `UIEventSystem`：UI专用事件系统，用于UI之间的通信

两者可以同时使用，互不干扰。

## 调试

启用事件日志记录：

```csharp
EventManager.Instance.SetLogEvents(true);
```

启用后，所有事件注册和触发都会记录到Unity控制台。

