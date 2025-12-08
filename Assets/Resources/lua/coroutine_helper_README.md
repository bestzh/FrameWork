# Lua协程辅助使用指南

## 概述

`coroutine_helper.lua` 提供了便捷的Lua协程API，让Lua脚本可以方便地使用Unity协程功能。

## 功能特性

- ✅ 启动/停止协程
- ✅ 等待秒数/帧数
- ✅ 延迟执行
- ✅ 等待条件满足
- ✅ 重复执行
- ✅ 等待多个协程完成

## 基本使用

### 1. 启动协程

```lua
local CoroutineHelper = require("coroutine_helper")

-- 定义协程函数
function MyCoroutine()
    print("协程开始")
    
    -- 等待1秒（注意：必须使用coroutine.yield）
    coroutine.yield(CoroutineHelper.WaitForSeconds(1.0))
    print("1秒后")
    
    -- 等待60帧
    coroutine.yield(CoroutineHelper.WaitForFrames(60))
    print("60帧后")
    
    -- 等待一帧
    coroutine.yield(CoroutineHelper.WaitForEndOfFrame())
    print("一帧后")
    
    print("协程结束")
end

-- 启动协程
local coroutineId = CoroutineHelper.Start(MyCoroutine)

-- 停止协程
-- CoroutineHelper.Stop(coroutineId)
```

### 2. 延迟执行

```lua
-- 延迟2秒后执行
CoroutineHelper.DelayCall(function()
    print("延迟执行")
end, 2.0)

-- 等待3秒后执行
CoroutineHelper.WaitSeconds(3.0, function()
    print("3秒后执行")
end)

-- 等待60帧后执行
CoroutineHelper.WaitFrames(60, function()
    print("60帧后执行")
end)
```

### 3. 等待条件

```lua
local someCondition = false

-- 等待条件满足（最多等待10秒）
CoroutineHelper.WaitUntil(function()
    return someCondition == true
end, function()
    print("条件满足")
end, 10.0)
```

### 4. 重复执行

```lua
-- 每秒执行一次，共5次
local repeatId = CoroutineHelper.Repeat(function()
    print("重复执行")
end, 1.0, 5)

-- 停止重复执行
-- CoroutineHelper.StopRepeat(repeatId)

-- 无限重复（直到手动停止）
local infiniteRepeatId = CoroutineHelper.Repeat(function()
    print("无限重复")
end, 1.0)  -- 不指定repeatCount，默认-1（无限）
```

## 等待对象

在协程函数中，可以使用以下等待对象：

### WaitForSeconds
```lua
coroutine.yield(CoroutineHelper.WaitForSeconds(1.0))  -- 等待1秒
```

### WaitForFrames
```lua
coroutine.yield(CoroutineHelper.WaitForFrames(60))  -- 等待60帧
```

### WaitForEndOfFrame
```lua
coroutine.yield(CoroutineHelper.WaitForEndOfFrame())  -- 等待一帧
```

### WaitForFixedUpdate
```lua
coroutine.yield(CoroutineHelper.WaitForFixedUpdate())  -- 等待固定更新
```

## 高级用法

### 在协程中循环

```lua
function LoopCoroutine()
    local count = 0
    while count < 10 do
        print("循环: " .. count)
        coroutine.yield(CoroutineHelper.WaitForSeconds(1.0))
        count = count + 1
    end
    print("循环结束")
end

CoroutineHelper.Start(LoopCoroutine)
```

### 协程嵌套

```lua
function OuterCoroutine()
    print("外层协程开始")
    
    -- 启动内层协程
    local innerId = CoroutineHelper.Start(function()
        print("内层协程开始")
        coroutine.yield(CoroutineHelper.WaitForSeconds(1.0))
        print("内层协程结束")
    end)
    
    -- 等待内层协程完成（简化处理）
    coroutine.yield(CoroutineHelper.WaitForSeconds(1.5))
    
    print("外层协程结束")
end

CoroutineHelper.Start(OuterCoroutine)
```

### 条件等待示例

```lua
-- 等待玩家血量恢复
local playerHP = 50
local maxHP = 100

CoroutineHelper.WaitUntil(function()
    return playerHP >= maxHP
end, function()
    print("玩家血量已满")
end)

-- 模拟血量恢复
CoroutineHelper.Repeat(function()
    playerHP = playerHP + 10
    print("当前血量: " .. playerHP)
end, 0.5, 10)  -- 每0.5秒恢复10点，共10次
```

## 注意事项

1. **必须使用 coroutine.yield**：在协程函数中，必须使用 `coroutine.yield()` 来等待，不能直接调用等待函数。

2. **协程函数不能有参数**：使用 `CoroutineHelper.Start()` 启动的协程函数不能有参数。如果需要参数，可以使用闭包：

```lua
local delay = 2.0
CoroutineHelper.Start(function()
    coroutine.yield(CoroutineHelper.WaitForSeconds(delay))
    print("延迟完成")
end)
```

3. **内存管理**：记得在适当的时候停止协程，避免内存泄漏：

```lua
local coroutineId = CoroutineHelper.Start(MyCoroutine)

-- 在对象销毁时停止
-- CoroutineHelper.Stop(coroutineId)
```

4. **性能考虑**：协程会持续运行，避免创建过多的协程。

## 与C#协程的区别

- **Lua协程**：使用 `CoroutineHelper.Start()` 启动，在Lua函数中使用 `coroutine.yield()` 等待
- **C#协程**：使用 `MonoBehaviour.StartCoroutine()` 启动，在C#方法中使用 `yield return` 等待

两者可以同时使用，互不干扰。

## 完整示例

```lua
local CoroutineHelper = require("coroutine_helper")

-- 示例1：简单的延迟执行
CoroutineHelper.DelayCall(function()
    print("延迟执行完成")
end, 2.0)

-- 示例2：协程循环
function CountdownCoroutine()
    for i = 5, 1, -1 do
        print("倒计时: " .. i)
        coroutine.yield(CoroutineHelper.WaitForSeconds(1.0))
    end
    print("倒计时结束！")
end

CoroutineHelper.Start(CountdownCoroutine)

-- 示例3：等待资源加载
local resourceLoaded = false

CoroutineHelper.WaitUntil(function()
    return resourceLoaded == true
end, function()
    print("资源加载完成")
end, 10.0)  -- 最多等待10秒

-- 模拟资源加载
CoroutineHelper.DelayCall(function()
    resourceLoaded = true
end, 3.0)

-- 示例4：定时更新
local updateId = CoroutineHelper.Repeat(function()
    print("定时更新")
end, 1.0)  -- 每秒更新一次

-- 5秒后停止更新
CoroutineHelper.DelayCall(function()
    CoroutineHelper.StopRepeat(updateId)
    print("更新已停止")
end, 5.0)
```

## API参考

### 启动/停止

- `CoroutineHelper.Start(coroutineFunc)` - 启动协程，返回协程ID
- `CoroutineHelper.Stop(coroutineId)` - 停止指定协程
- `CoroutineHelper.StopAll()` - 停止所有协程

### 等待对象（在协程中使用）

- `CoroutineHelper.WaitForSeconds(seconds)` - 等待秒数
- `CoroutineHelper.WaitForFrames(frames)` - 等待帧数
- `CoroutineHelper.WaitForEndOfFrame()` - 等待一帧
- `CoroutineHelper.WaitForFixedUpdate()` - 等待固定更新

### 便捷方法

- `CoroutineHelper.DelayCall(callback, delay)` - 延迟执行回调
- `CoroutineHelper.WaitSeconds(seconds, callback)` - 等待秒数后执行
- `CoroutineHelper.WaitFrames(frames, callback)` - 等待帧数后执行
- `CoroutineHelper.WaitUntil(condition, callback, timeout)` - 等待条件满足
- `CoroutineHelper.Repeat(callback, interval, repeatCount)` - 重复执行

### 工具方法

- `CoroutineHelper.GetActiveCount()` - 获取活动协程数量
- `CoroutineHelper.Cleanup()` - 清理所有协程

