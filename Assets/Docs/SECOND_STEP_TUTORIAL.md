# 第二步详细教程：添加NPC和传送门

## 🎯 目标
在城镇场景中添加NPC（非玩家角色）和传送门，实现基础交互功能。

## ⏱️ 预计时间
30-45分钟

## 📋 前置条件
- ✅ 已完成第一步教程（创建城镇场景）
- ✅ 场景中有玩家对象（Tag: Player）
- ✅ 玩家可以正常移动

---

## 📋 步骤1：添加NPC

### 1.1 创建NPC对象
```
1. 在Unity场景中，GameObject → Create Empty
2. 重命名为 "NPC_Test"
3. 设置位置：(5, 0, 0) - 在玩家附近
```

### 1.2 添加NPC模型（临时）
```
方法1：使用Capsule（推荐）
1. GameObject → 3D Object → Capsule
2. 作为NPC_Test的子对象
3. 重命名为 "NPC_Model"
4. 设置位置：(0, 1, 0)
5. 添加不同颜色材质（比如红色或蓝色）以区分NPC

方法2：使用已有模型（如果有）
1. 导入你的NPC模型
2. 作为NPC_Test的子对象
```

### 1.3 添加NPCController组件
```
1. 选择NPC_Test对象
2. Add Component → NPCController
3. 设置组件参数：
   - NPC Name: "测试NPC"
   - Dialogue Text: "你好，冒险者！欢迎来到城镇。"
   - Interaction Distance: 2
   - Interaction Key: E（默认）
```

### 1.4 设置Tag（可选）
```
1. 选择NPC_Test对象
2. Tag设置为 "NPC"（如果已创建）或保持 "Untagged"
```

### 1.5 测试NPC交互
```
1. 点击Play按钮
2. 控制玩家靠近NPC
3. 应该看到交互提示："按 E 交互"
4. 按E键，查看Console输出对话
```

---

## 📋 步骤2：添加传送门

### 2.1 创建传送门对象
```
1. GameObject → Create Empty
2. 重命名为 "Portal_Test"
3. 设置位置：(0, 0, 10) - 在玩家前方
```

### 2.2 添加传送门模型（临时）
```
方法1：使用Cylinder（推荐）
1. GameObject → 3D Object → Cylinder
2. 作为Portal_Test的子对象
3. 重命名为 "Portal_Model"
4. 设置位置：(0, 1, 0)
5. 设置缩放：(2, 0.5, 2) - 让它看起来像传送门
6. 添加特殊颜色材质（比如紫色或蓝色）

方法2：使用已有模型（如果有）
1. 导入你的传送门模型
2. 作为Portal_Test的子对象
```

### 2.3 添加PortalController组件
```
1. 选择Portal_Test对象
2. Add Component → PortalController
3. 设置组件参数：
   - Portal Name: "测试传送门"
   - Target Scene Name: "Town"（或你的目标场景名称）
   - Portal Description: "传送到另一个场景"
   - Interaction Distance: 3
   - Interaction Key: E（默认）
   - Teleport Delay: 0.5
```

### 2.4 准备目标场景
```
重要：目标场景必须在Build Settings中！

1. File → Build Settings
2. 点击 "Add Open Scenes" 添加当前场景（如果还没有）
3. 确保目标场景也在Build Settings中
4. 记录目标场景的名称（用于Target Scene Name）
```

### 2.5 测试传送门
```
1. 点击Play按钮
2. 控制玩家靠近传送门
3. 应该看到交互提示："按 E 进入 测试传送门"
4. 按E键，等待0.5秒后场景切换
```

---

## 📋 步骤3：添加多个NPC（可选）

### 3.1 创建更多NPC
```
1. 复制NPC_Test对象（Ctrl+D）
2. 重命名为 "NPC_Shop"、"NPC_Quest" 等
3. 设置不同位置
4. 修改NPCController参数：
   - 不同的NPC Name
   - 不同的Dialogue Text
```

### 3.2 NPC布局建议
```
- NPC_Shop: (10, 0, 0) - 商店NPC
- NPC_Quest: (-10, 0, 0) - 任务NPC
- NPC_Trainer: (0, 0, 10) - 训练师NPC
```

---

## 📋 步骤4：优化和美化

### 4.1 调整交互提示
```
NPC和传送门的交互提示会自动创建，但你可以：
1. 自定义提示文本
2. 调整提示位置（在NPCController/PortalController中）
3. 替换为更美观的UI预制体
```

### 4.2 添加特效（可选）
```
1. 为传送门添加粒子特效
2. 将特效对象拖拽到PortalController的Portal Effect字段
3. 特效会在传送时自动显示
```

### 4.3 调整交互距离
```
根据你的场景大小，调整：
- NPC Interaction Distance: 2-3
- Portal Interaction Distance: 3-5
```

---

## ✅ 完成检查清单

- [ ] NPC已创建并可以交互
- [ ] 靠近NPC时显示交互提示
- [ ] 按E键可以与NPC对话（Console输出）
- [ ] 传送门已创建
- [ ] 靠近传送门时显示交互提示
- [ ] 按E键可以传送（场景切换）
- [ ] 目标场景在Build Settings中
- [ ] 场景切换正常工作

---

## 🎯 下一步

完成这一步后，下一步是：
1. 添加对话UI系统（替换Console输出）
2. 添加任务系统
3. 添加商店系统

---

## 💡 提示

### 如果遇到问题：

**问题：NPC交互提示不显示**
- 检查Player对象的Tag是否为"Player"
- 检查NPCController组件是否正确添加
- 检查Interaction Distance是否足够大
- 查看Console是否有错误信息

**问题：传送门不工作**
- 检查Target Scene Name是否正确
- 检查目标场景是否在Build Settings中
- 检查GameSceneManager是否存在
- 查看Console是否有错误信息

**问题：场景切换失败**
- 确认场景名称拼写正确（区分大小写）
- 确认场景在Build Settings中
- 检查GameSceneManager是否正常工作
- 查看Console错误信息

**问题：交互提示位置不对**
- 交互提示会自动跟随NPC/传送门
- 可以在脚本中调整提示高度（UpdateHintPosition方法）
- 提示会自动面向相机

---

## 📝 代码说明

### NPCController 主要功能
- ✅ 自动检测玩家距离
- ✅ 显示/隐藏交互提示
- ✅ 响应交互按键（默认E）
- ✅ 发送交互事件（EventManager）
- ✅ 显示对话（目前使用Debug.Log）

### PortalController 主要功能
- ✅ 自动检测玩家距离
- ✅ 显示/隐藏交互提示
- ✅ 响应交互按键（默认E）
- ✅ 异步加载场景（带进度回调）
- ✅ 发送传送事件（EventManager）

---

## 🔧 高级用法

### 自定义交互按键
```
在Inspector中修改：
- NPCController.Interaction Key
- PortalController.Interaction Key
```

### 使用事件系统
```
NPC和传送门会发送事件：
- "NPC_Interact" - NPC交互时
- "Portal_Enter" - 进入传送门时

可以订阅这些事件：
EventManager.Instance.Subscribe("NPC_Interact", OnNPCInteract);
```

### 多个场景设置
```
1. 创建多个场景（Town, Dungeon1, Dungeon2等）
2. 在Build Settings中添加所有场景
3. 为每个传送门设置不同的Target Scene Name
```

---

## 🚀 开始吧！

**现在就做：**
1. 打开Unity
2. 按照步骤1-2添加NPC和传送门
3. 测试交互功能
4. 完成检查清单

**完成后告诉我，我们继续下一步！** 🎮

---

## 📚 相关文件

- `Scripts/RPG/Town/NPCController.cs` - NPC控制器
- `Scripts/RPG/Town/PortalController.cs` - 传送门控制器
- `Scripts/SceneManager.cs` - 场景管理器（GameSceneManager）
- `Scripts/EventManager.cs` - 事件管理器

