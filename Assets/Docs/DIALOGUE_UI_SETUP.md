# 对话UI系统设置指南

## 🎯 概述

对话UI系统已经集成到NPC系统中，现在NPC交互时会显示美观的对话界面，而不是Console输出。

**重要：UI逻辑使用Lua实现，支持热更新！**

## ✅ 已完成的工作

1. ✅ 创建了 `DialogueUI` Lua脚本（`Resources/lua/ui/dialogue_ui.lua.txt`）
2. ✅ 创建了 `DialogueManager` Lua脚本（`Resources/lua/rpg/dialogue_manager.lua.txt`）
3. ✅ 更新了 `NPCController` 直接调用Lua对话管理器（无需C#桥接层）
4. ✅ **完全使用Lua实现，支持热更新！**

## 📋 下一步：创建对话UI预制体

### 步骤1：创建UI预制体结构

在Unity中创建对话UI预制体：

```
1. 右键点击 Hierarchy → UI → Canvas
2. 重命名为 "DialogueUI"
3. 设置Canvas的Render Mode为 "Screen Space - Overlay"
4. 添加CanvasScaler组件（可选，用于适配不同分辨率）
```

### 步骤2：创建UI结构

在Canvas下创建以下子对象：

```
DialogueUI (Canvas)
├── DialoguePanel (Image)
│   ├── NPCNameText (TextMeshProUGUI) - 显示NPC名称
│   ├── DialogueText (TextMeshProUGUI) - 显示对话内容
│   ├── NPCAvatar (Image) - NPC头像（可选）
│   └── CloseButton (Button) - 关闭按钮
```

### 步骤3：设置UI组件

#### DialoguePanel (Image)
- **位置**: 屏幕底部居中
- **大小**: Width: 800, Height: 200
- **锚点**: Bottom Center
- **颜色**: 半透明黑色背景（例如：RGBA(0, 0, 0, 200)）

#### NPCNameText (TextMeshProUGUI)
- **位置**: Panel左上角
- **文本**: "NPC名称"
- **字体大小**: 24
- **颜色**: 白色
- **对齐**: 左上对齐

#### DialogueText (TextMeshProUGUI)
- **位置**: NPCNameText下方
- **文本**: "对话内容..."
- **字体大小**: 18
- **颜色**: 白色
- **对齐**: 左上对齐
- **换行**: 启用自动换行
- **大小**: 宽度填满Panel，高度自适应

#### NPCAvatar (Image) (可选)
- **位置**: Panel左侧
- **大小**: 100x100
- **用途**: 显示NPC头像（如果不需要可以隐藏）

#### CloseButton (Button)
- **位置**: Panel右上角
- **大小**: 40x40
- **文本**: "X" 或使用图标
- **功能**: 点击关闭对话

### 步骤4：添加LuaUIBase组件

```
1. 选择DialogueUI根对象（Canvas）
2. Add Component → LuaUIBase（不是DialogueUI！）
3. 注意：UI逻辑在Lua脚本中，不需要设置组件引用
4. Lua脚本会自动查找UI组件（通过Transform.Find）
```

**重要说明：**
- UI逻辑完全在Lua中实现（`Resources/lua/ui/dialogue_ui.lua.txt`）
- 使用 `LuaUIBase` 组件，而不是 `DialogueUI` 组件
- Lua脚本会自动查找UI组件，组件名称需要匹配：
  - `DialoguePanel/NPCNameText` 或 `DialoguePanel/NPCName`
  - `DialoguePanel/DialogueText` 或 `DialoguePanel/Content`
  - `DialoguePanel/NPCAvatar` 或 `DialoguePanel/Avatar`
  - `DialoguePanel/CloseButton` 或 `DialoguePanel/CloseBtn`

### 步骤5：保存预制体

```
1. 在Project窗口中创建文件夹: Resources/UI/
2. 将DialogueUI对象拖拽到 Resources/UI/ 文件夹
3. 保存为预制体: DialogueUI.prefab
4. 删除场景中的DialogueUI对象（预制体已保存）
```

### 步骤6：配置DialogueManager（可选）

如果预制体路径不是 `UI/DialogueUI`，可以在代码中修改：

```csharp
// 在DialogueManager的Inspector中设置
Dialogue UIPath = "UI/你的路径"
```

或者在运行时设置：

```csharp
DialogueManager.Instance.SetDialogueUIPath("UI/你的路径");
```

## 🎮 使用方法

### 基本使用

NPC交互时会自动显示对话UI，无需额外代码。所有逻辑都在Lua脚本中实现。

### 手动调用（C#）

**注意：** C#代码需要直接调用Lua模块，没有C#桥接层。

```csharp
// 显示对话（通过LuaManager）
var luaManager = LuaManager.Instance;
if (luaManager != null && luaManager.LuaEnv != null)
{
    var dialogueManager = luaManager.Require("rpg.dialogue_manager");
    if (dialogueManager != null)
    {
        var showDialogueFunc = dialogueManager.Get<XLua.LuaFunction>("ShowDialogue");
        if (showDialogueFunc != null)
        {
            showDialogueFunc.Call(
                "NPC名称",
                "对话内容",
                null, // 头像路径（可选）
                (System.Action)(() => { Debug.Log("对话关闭"); }) // 关闭回调（可选）
            );
            showDialogueFunc.Dispose();
        }
        dialogueManager.Dispose();
    }
}

// 关闭对话
var dialogueManager = luaManager.Require("rpg.dialogue_manager");
if (dialogueManager != null)
{
    var closeDialogueFunc = dialogueManager.Get<XLua.LuaFunction>("CloseDialogue");
    if (closeDialogueFunc != null)
    {
        closeDialogueFunc.Call();
        closeDialogueFunc.Dispose();
    }
    dialogueManager.Dispose();
}
```

**推荐：** 如果可能，尽量在Lua中调用，代码更简洁。

### 手动调用（Lua）

```lua
-- 加载对话管理器模块
local DialogueManager = require("rpg.dialogue_manager")

-- 显示对话
DialogueManager.ShowDialogue(
    "NPC名称",
    "对话内容",
    "UI/Avatar/NPC1",  -- 头像路径（可选）
    function()
        print("对话关闭")
    end  -- 关闭回调（可选）
)

-- 关闭对话
DialogueManager.CloseDialogue()
```

### 快捷键

- **ESC键**: 关闭对话
- **鼠标左键/空格键**: 
  - 如果正在打字机效果中：跳过打字机，直接显示全部文本
  - 如果文本已全部显示：关闭对话

## ⚙️ 自定义设置

### 打字机效果

在 `Resources/lua/ui/dialogue_ui.lua.txt` 中修改：
```lua
local typingSpeed = 0.05  -- 打字机效果速度（秒/字符）
local useTypingEffect = true  -- 是否使用打字机效果
```

### 对话UI路径

在 `Resources/lua/rpg/dialogue_manager.lua.txt` 中修改：
```lua
local dialogueUIPath = "UI/DialogueUI"  -- UI预制体路径
```

或在运行时修改：
```lua
DialogueManager.SetDialogueUIPath("UI/你的路径")
```

### UI样式

可以自定义：
- 对话面板背景颜色和透明度
- 文本字体、大小、颜色
- 按钮样式
- 整体布局和位置

## 🔧 故障排除

### 问题：对话UI不显示

**检查清单：**
1. ✅ 预制体路径是否正确：`Resources/UI/DialogueUI.prefab`
2. ✅ 预制体上是否有 `LuaUIBase` 组件（不是DialogueUI！）
3. ✅ `DialogueManager` 是否已初始化（会自动创建）
4. ✅ Lua脚本是否存在：`Resources/lua/ui/dialogue_ui.lua.txt`
5. ✅ UI组件名称是否匹配（见步骤4说明）
6. ✅ 查看Console是否有错误信息

**调试方法：**
```csharp
// 检查DialogueManager是否正常
if (DialogueManager.Instance == null)
{
    Debug.LogError("DialogueManager未初始化！");
}

// 检查Lua是否加载成功（查看Console日志）
```

**Lua调试：**
```lua
-- 检查Lua模块是否加载成功
local DialogueManager = require("rpg.dialogue_manager")
if not DialogueManager then
    print("错误: 无法加载DialogueManager模块")
end
```

### 问题：组件引用丢失

**解决方法：**
1. 打开预制体
2. 确保UI组件名称匹配（见步骤4说明）
3. Lua脚本会自动查找组件，不需要手动设置引用
4. 如果组件名称不匹配，修改Lua脚本中的查找路径

### 问题：文本不显示

**检查：**
1. TextMeshProUGUI组件是否正确设置
2. 文本颜色是否与背景色相同（导致看不见）
3. 文本区域大小是否足够

## 📝 快速创建模板

如果你想要快速创建，可以使用以下Unity菜单：

```
1. GameObject → UI → Canvas
2. 右键Canvas → UI → Image (作为Panel)
3. 右键Panel → UI → TextMeshPro - Text (作为NPC名称)
4. 右键Panel → UI → TextMeshPro - Text (作为对话内容)
5. 右键Panel → UI → Button (作为关闭按钮)
6. 按照上面的步骤4添加DialogueUI组件
```

## 🎨 UI设计建议

### 推荐布局

```
┌─────────────────────────────────────┐
│  [头像]  NPC名称              [X]    │
│          ──────────────────────     │
│          对话内容文本...             │
│          可以多行显示                │
└─────────────────────────────────────┘
```

### 推荐颜色

- **背景**: 半透明黑色 (RGBA: 0, 0, 0, 200)
- **文本**: 白色或浅色
- **按钮**: 红色或灰色

### 推荐位置

- **底部居中**: 不遮挡游戏画面
- **大小**: 宽度800-1000，高度200-300

## ✅ 完成检查清单

- [ ] 对话UI预制体已创建
- [ ] 预制体路径为 `Resources/UI/DialogueUI.prefab`
- [ ] DialogueUI组件已添加并配置
- [ ] 所有UI组件引用已设置
- [ ] 测试NPC交互，对话UI正常显示
- [ ] 测试关闭按钮和快捷键
- [ ] 测试打字机效果（如果启用）

## 🚀 下一步

完成对话UI设置后，你可以：

1. **添加对话选项系统** - 让玩家可以选择不同的对话选项
2. **添加任务系统** - NPC可以给玩家任务
3. **添加商店系统** - NPC可以打开商店界面
4. **美化UI** - 添加动画、特效等

---

**提示**: 如果遇到任何问题，请查看Console错误信息，或参考 `UIManager` 的使用文档。

