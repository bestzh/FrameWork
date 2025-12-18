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
│   ├── OptionsContainer (GameObject) - 选项容器（VerticalLayoutGroup）
│   │   ├── OptionButton1 (Button) - 选项按钮1（可选，作为模板）
│   │   │   └── Text (TextMeshProUGUI) - 选项文本
│   │   └── OptionButton2 (Button) - 选项按钮2（可选，作为模板）
│   │       └── Text (TextMeshProUGUI) - 选项文本
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

#### OptionsContainer (GameObject)
- **位置**: DialogueText下方
- **组件**: 添加 `Vertical Layout Group` 组件（Unity UI）
- **用途**: 容纳对话选项按钮
- **设置**:
  - Spacing: 10（选项间距）
  - Child Alignment: Upper Center
  - Child Force Expand: Width = true, Height = false
- **注意**: 如果UI中没有预设选项按钮，脚本会自动创建（需要至少有一个子对象作为模板）

#### OptionButton (Button) - 选项按钮模板（可选）
- **位置**: OptionsContainer内
- **大小**: Width: 700, Height: 50
- **组件**: Button + TextMeshProUGUI子对象
- **文本**: "选项文本"
- **用途**: 作为选项按钮的模板（如果使用动态创建）
- **注意**: 可以创建多个模板按钮，脚本会自动使用或复制

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
  - `DialoguePanel/OptionsContainer` 或 `DialoguePanel/OptionsPanel`（用于显示选项）
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

#### 基本对话（无选项）

```lua
-- 加载对话管理器模块
local DialogueManager = require("rpg.dialogue_manager")

-- 显示对话
DialogueManager.ShowDialogue(
    "NPC名称",
    "对话内容",
    "UI/Avatar/NPC1",  -- 头像路径（可选）
    nil,  -- 选项列表（可选）
    nil,  -- 选项选择回调（可选）
    function()
        print("对话关闭")
    end  -- 关闭回调（可选）
)

-- 关闭对话
DialogueManager.CloseDialogue()
```

#### 带选项的对话

```lua
-- 加载对话管理器模块
local DialogueManager = require("rpg.dialogue_manager")

-- 定义对话选项
local options = {
    {
        text = "打开商店",
        action = "open_shop",
        nextDialogueId = 1  -- 可选：下一个对话ID
    },
    {
        text = "查看任务",
        action = "open_quest",
        nextDialogueId = 2
    },
    {
        text = "再见",
        action = "close"
    }
}

-- 显示带选项的对话
DialogueManager.ShowDialogue(
    "铁匠·史密斯",
    "欢迎来到我的商店！你想要什么？",
    "UI/Avatar/Blacksmith",
    options,  -- 选项列表
    function(option, index)  -- 选项选择回调
        print("选择了选项: " .. option.text)
        if option.action == "open_shop" then
            -- 打开商店
            print("打开商店")
        elseif option.action == "open_quest" then
            -- 打开任务界面
            print("打开任务界面")
        elseif option.action == "close" then
            -- 关闭对话
            DialogueManager.CloseDialogue()
        end
    end,
    function()
        print("对话关闭")
    end
)
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

## 📝 添加选项按钮到UI预制体

### 方法1：使用预设按钮（推荐）

1. **创建选项容器**
   - 在 `DialoguePanel` 下创建空GameObject，命名为 `OptionsContainer`
   - 添加 `Vertical Layout Group` 组件
   - 设置：
     - Spacing: 10
     - Child Alignment: Upper Center
     - Child Force Expand: Width = true, Height = false
     - Padding: Left/Right = 20, Top/Bottom = 10

2. **创建选项按钮模板**
   - 在 `OptionsContainer` 下创建 Button，命名为 `OptionButton1`
   - 设置按钮大小：Width = 700, Height = 50
   - 在按钮下创建 TextMeshPro - Text 子对象，命名为 `Text`
   - 设置文本：字体大小 18，颜色白色，居中对齐
   - **重要**：默认隐藏这个按钮（SetActive(false)），它只作为模板

3. **可选**：创建更多模板按钮（OptionButton2, OptionButton3等）
   - 脚本会自动使用或复制这些按钮

### 方法2：动态创建（如果UI中没有预设按钮）

如果 `OptionsContainer` 中没有任何子对象，脚本会尝试创建按钮，但需要确保：
- `OptionsContainer` 存在
- `OptionsContainer` 有 `Vertical Layout Group` 组件

**注意**：推荐使用方法1，因为可以更好地控制按钮样式。

## ✅ 完成检查清单

- [ ] 对话UI预制体已创建
- [ ] 预制体路径为 `Resources/UI/DialogueUI.prefab`
- [ ] LuaUIBase组件已添加并配置
- [ ] 所有UI组件引用已设置
- [ ] OptionsContainer已创建并配置
- [ ] 选项按钮模板已创建（至少一个）
- [ ] 测试NPC交互，对话UI正常显示
- [ ] 测试带选项的对话
- [ ] 测试关闭按钮和快捷键
- [ ] 测试打字机效果（如果启用）

## 🎯 对话选项系统

对话选项系统已经集成！现在NPC可以显示多个选项供玩家选择。

### 选项数据结构

每个选项是一个表（table），包含以下字段：

```lua
{
    text = "选项文本",           -- 必填：显示的文本
    action = "open_shop",       -- 可选：动作类型（自定义字符串）
    nextDialogueId = 1,         -- 可选：下一个对话ID
    data = {}                   -- 可选：自定义数据
}
```

### 选项动作类型

你可以定义任何动作类型，常见的有：
- `"open_shop"` - 打开商店
- `"open_quest"` - 打开任务界面
- `"accept_quest"` - 接取任务
- `"complete_quest"` - 完成任务
- `"close"` - 关闭对话
- 自定义动作...

### 使用示例

```lua
-- 示例：商店NPC对话
local shopOptions = {
    {text = "我想买东西", action = "open_shop"},
    {text = "我想卖东西", action = "open_sell"},
    {text = "再见", action = "close"}
}

DialogueManager.ShowDialogue(
    "铁匠·史密斯",
    "欢迎！需要什么帮助吗？",
    nil,
    shopOptions,
    function(option, index)
        if option.action == "open_shop" then
            -- 打开商店UI
            print("打开商店")
        elseif option.action == "open_sell" then
            -- 打开出售界面
            print("打开出售界面")
        elseif option.action == "close" then
            DialogueManager.CloseDialogue()
        end
    end
)
```

## 🚀 下一步

完成对话UI和选项系统后，你可以：

1. ✅ **对话选项系统** - 已完成！现在可以显示选项了
2. **集成任务系统** - 让NPC可以通过对话选项给玩家任务
3. **添加商店系统** - NPC可以打开商店界面
4. **对话分支系统** - 实现多段对话，根据选项跳转到不同对话
5. **美化UI** - 添加动画、特效等

---

**提示**: 如果遇到任何问题，请查看Console错误信息，或参考 `UIManager` 的使用文档。

