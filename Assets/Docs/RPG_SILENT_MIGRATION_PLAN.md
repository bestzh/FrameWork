# RPG-Silent 项目移植计划

## 📋 项目分析

### 原项目核心系统

1. **玩家控制系统**
   - `PlayerController` - 玩家控制器
   - `PlayerStateMachine` - 状态机（Idle, Move, Attack, Jump, Roll, Hurt, Dead）
   - `EnemyController` - 敌人控制器

2. **技能系统**
   - `SkillCastManager` - 技能释放管理
   - `SkillData` - 技能数据（ScriptableObject）
   - `ComboManager` - 连招管理
   - `PlayerSkillController` - 技能动画控制

3. **UI系统**
   - `UIManager` - UI管理器（基于Addressables）
   - `UIBase` - UI基类
   - `MainUI`, `StartUI`, `LoadingUI` - 具体UI

4. **管理器系统**
   - `InputManager` - 输入管理
   - `SceneLoaderManager` - 场景加载
   - `ScreenShakeManager` - 屏幕震动
   - `AddressableManager` - Addressables管理

5. **动画系统**
   - `PlayerAnimationController` - 玩家动画控制
   - `AnimationData` - 动画数据

## 🎯 移植策略

### 策略1：保持原有功能，适配框架接口

**原则：**
- 保持原有游戏逻辑不变
- 替换底层框架调用（资源加载、数据存储等）
- 利用框架的UI系统、事件系统等

### 策略2：逐步迁移到框架系统

**步骤：**
1. 先移植核心系统（玩家控制、技能系统）
2. 适配UI系统到框架UI
3. 使用框架的数据存储和事件系统
4. 最后优化和整合

## 📝 移植清单

### 第一阶段：核心系统移植

#### 1. 玩家控制系统 ✅
- [x] 创建 `Scripts/RPG/Player/PlayerController.cs`（适配框架）
- [ ] 移植 `PlayerStateMachine` 状态机
- [ ] 移植各种状态（Idle, Move, Attack等）
- [ ] 适配到框架的输入系统

#### 2. 技能系统 ✅
- [x] 创建 `Scripts/RPG/Battle/BattleManager.cs`（即时战斗）
- [ ] 移植 `SkillCastManager` → `Scripts/RPG/Battle/SkillSystem.cs`
- [ ] 移植 `SkillData` → `Scripts/RPG/Battle/SkillData.cs`
- [ ] 移植 `ComboManager` → `Scripts/RPG/Battle/ComboManager.cs`
- [ ] 使用框架的 `ObjectPool` 管理技能特效

#### 3. 敌人系统
- [ ] 移植 `EnemyController` → `Scripts/RPG/Battle/EnemyController.cs`
- [ ] 适配到框架的 `BattleManager`

### 第二阶段：UI系统适配

#### 1. UI系统迁移
- [ ] 将原项目的UI适配到框架的 `UIManager`
- [ ] 使用框架的 `LuaUIBase` 支持热更新
- [ ] 迁移 `MainUI`, `StartUI`, `LoadingUI`

#### 2. 战斗UI
- [ ] 创建战斗UI（血条、技能栏等）
- [ ] 使用框架的UI系统

### 第三阶段：资源和管理器适配

#### 1. 资源加载
- [ ] 将原项目的 `AddressableManager` 替换为框架的 `ResManager`
- [ ] 统一使用框架的资源加载接口

#### 2. 场景管理
- [ ] 将 `SceneLoaderManager` 替换为框架的 `GameSceneManager`

#### 3. 其他管理器
- [ ] 移植 `ScreenShakeManager`（可选）
- [ ] 移植 `InputManager`（适配框架）

### 第四阶段：数据系统

#### 1. 数据存储
- [ ] 使用框架的 `SaveManager` 存储玩家数据
- [ ] 使用框架的 `TableManager` 读取配置表

#### 2. 事件系统
- [ ] 使用框架的 `EventManager` 替代自定义事件
- [ ] 注册RPG相关事件

## 🔧 具体移植步骤

### 步骤1：复制核心脚本

将原项目的核心脚本复制到框架项目中，放在 `Scripts/RPG/` 目录下。

### 步骤2：适配资源加载

**原代码：**
```csharp
Addressables.InstantiateAsync(uiKey, UIRoot);
```

**框架代码：**
```csharp
ResManager.LoadAsync<GameObject>(uiKey, (prefab) => {
    GameObject uiObj = Instantiate(prefab, UIRoot);
});
```

### 步骤3：适配UI系统

**原代码：**
```csharp
UIManager.Instance.OpenUI("UI/StartUI");
```

**框架代码：**
```csharp
UI.UIManager.Instance.ShowUI("StartUI");
// 或使用Lua驱动UI
LuaHelper.LoadLuaUI("StartUI", "UI/StartUI", callbacks);
```

### 步骤4：适配数据存储

**原代码：**
```csharp
PlayerPrefs.SetInt("Level", level);
```

**框架代码：**
```csharp
SaveManager.Instance.SaveInt("Level", level);
```

### 步骤5：适配事件系统

**原代码：**
```csharp
// 自定义事件系统
```

**框架代码：**
```csharp
EventManager.Instance.TriggerEvent(GlobalEventNames.PLAYER_LEVEL_UP, level);
```

## 📁 移植后的目录结构

```
Scripts/RPG/
├── Player/                    # 玩家系统
│   ├── PlayerController.cs   # 玩家控制器（适配框架）
│   ├── PlayerStateMachine.cs  # 状态机
│   ├── States/                # 各种状态
│   └── EnemyController.cs     # 敌人控制器
├── Battle/                    # 战斗系统
│   ├── BattleManager.cs       # 战斗管理器（已创建，即时战斗）
│   ├── SkillSystem.cs         # 技能系统（待移植）
│   ├── SkillData.cs           # 技能数据（待移植）
│   ├── ComboManager.cs        # 连招管理（待移植）
│   └── AIController.cs        # AI控制器（待创建）
├── Inventory/                 # 背包系统（已创建）
├── Quest/                     # 任务系统（已创建）
└── UI/                        # UI系统（待适配）
```

## ⚠️ 注意事项

1. **保持原有功能**
   - 不要改变游戏逻辑
   - 只替换底层框架调用

2. **逐步迁移**
   - 先移植核心功能
   - 再优化和整合

3. **测试验证**
   - 每移植一个系统都要测试
   - 确保功能正常

4. **资源路径**
   - 统一使用框架的资源加载方式
   - 适配Addressables路径

5. **命名空间**
   - 避免命名冲突
   - 统一命名规范

## 🚀 开始移植

我将帮你逐步移植各个系统，保持原有功能的同时利用框架的优势。

