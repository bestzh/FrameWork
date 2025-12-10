# NPC和传送门表格配置使用指南

## 📖 概述

NPC和传送门现在支持通过表格配置来管理数据，这样可以：
- ✅ 统一管理所有NPC和传送门的配置
- ✅ 方便策划修改配置，无需打开Unity
- ✅ 支持批量配置和修改
- ✅ 保留Inspector配置作为备用方案

---

## 🚀 快速开始

### 步骤1：生成表格代码

1. 在Unity编辑器中，点击菜单：**Tools → 表 → 一键处理**
2. 等待处理完成（会自动生成C#代码和二进制文件）
3. 生成的代码位于：`Scripts/Table/Gen/` 目录

### 步骤2：配置NPC

#### 方法1：使用表格配置（推荐）

1. 打开 `Resources/Table/NPCConfig.csv` 文件
2. 添加或修改NPC配置行，例如：
   ```
   ID	Name	DialogueText	InteractionDistance	InteractionKey
   UINT32	STRING	STRING	FLOAT	STRING
   1	测试NPC	你好，冒险者！欢迎来到城镇。	2	E
   2	商店NPC	欢迎光临！需要什么装备吗？	2.5	E
   ```
3. 在Unity场景中选择NPC对象
4. 在NPCController组件中设置 **Config ID** 为对应的ID（如1、2等）
5. 重新生成表格代码（步骤1）

#### 方法2：使用Inspector配置（备用）

1. 在NPCController组件中设置 **Config ID** 为 **0**
2. 直接在Inspector中填写NPC信息

### 步骤3：配置传送门

#### 方法1：使用表格配置（推荐）

1. 打开 `Resources/Table/PortalConfig.csv` 文件
2. 添加或修改传送门配置行，例如：
   ```
   ID	Name	TargetSceneName	Description	InteractionDistance	InteractionKey	TeleportDelay
   UINT32	STRING	STRING	STRING	FLOAT	STRING	FLOAT
   1	测试传送门	Town	传送到另一个场景	3	E	0.5
   2	城镇传送门	Town	返回城镇	3	E	0.5
   ```
3. 在Unity场景中选择传送门对象
4. 在PortalController组件中设置 **Config ID** 为对应的ID（如1、2等）
5. 重新生成表格代码（步骤1）

#### 方法2：使用Inspector配置（备用）

1. 在PortalController组件中设置 **Config ID** 为 **0**
2. 直接在Inspector中填写传送门信息

---

## 📋 表格字段说明

### NPCConfig.csv

| 字段名 | 类型 | 说明 | 示例 |
|--------|------|------|------|
| ID | UINT32 | 配置ID（主键，必须唯一） | 1 |
| Name | STRING | NPC名称 | "测试NPC" |
| DialogueText | STRING | 对话文本 | "你好，冒险者！" |
| InteractionDistance | FLOAT | 交互距离（米） | 2.0 |
| InteractionKey | STRING | 交互按键（Unity KeyCode名称） | "E" |
| PositionX | FLOAT | X坐标位置 | 5.0 |
| PositionY | FLOAT | Y坐标位置 | 0.0 |
| PositionZ | FLOAT | Z坐标位置 | 0.0 |
| RotationY | FLOAT | Y轴旋转角度（度） | 0.0 |

**支持的按键：** E, F, Space, Enter, Return 等（Unity KeyCode枚举值）

### PortalConfig.csv

| 字段名 | 类型 | 说明 | 示例 |
|--------|------|------|------|
| ID | UINT32 | 配置ID（主键，必须唯一） | 1 |
| Name | STRING | 传送门名称 | "测试传送门" |
| TargetSceneName | STRING | 目标场景名称（必须在Build Settings中） | "Town" |
| Description | STRING | 传送门描述 | "传送到另一个场景" |
| InteractionDistance | FLOAT | 交互距离（米） | 3.0 |
| InteractionKey | STRING | 交互按键（Unity KeyCode名称） | "E" |
| TeleportDelay | FLOAT | 传送延迟（秒） | 0.5 |
| PositionX | FLOAT | X坐标位置 | 0.0 |
| PositionY | FLOAT | Y坐标位置 | 0.0 |
| PositionZ | FLOAT | Z坐标位置 | 10.0 |
| RotationY | FLOAT | Y轴旋转角度（度） | 0.0 |

---

## 💡 使用示例

### 示例1：创建商店NPC

**表格配置：**
```csv
ID	Name	DialogueText	InteractionDistance	InteractionKey	PositionX	PositionY	PositionZ	RotationY
UINT32	STRING	STRING	FLOAT	STRING	FLOAT	FLOAT	FLOAT	FLOAT
10	商店老板	欢迎来到我的商店！这里有各种装备和道具。	2.5	E	10	0	0	0
```

**Unity设置：**
1. 选择NPC对象
2. NPCController → Config ID = 10
3. Use Table Position = true（使用表格中的位置）
4. 其他字段会自动从表格加载，位置会自动设置

### 示例2：创建副本传送门

**表格配置：**
```csv
ID	Name	TargetSceneName	Description	InteractionDistance	InteractionKey	TeleportDelay	PositionX	PositionY	PositionZ	RotationY
UINT32	STRING	STRING	STRING	FLOAT	STRING	FLOAT	FLOAT	FLOAT	FLOAT	FLOAT
20	副本入口	Dungeon1	进入地下城副本	3.5	E	0.5	-15	0	0	-90
```

**Unity设置：**
1. 选择传送门对象
2. PortalController → Config ID = 20
3. Use Table Position = true（使用表格中的位置）
4. 确保目标场景"Dungeon1"在Build Settings中
5. 位置会自动设置到表格中配置的坐标

---

## 🔧 配置优先级

1. **Config ID > 0**：从表格读取配置（优先）
2. **Config ID = 0**：使用Inspector中的值（备用）

**位置配置：**
- **Use Table Position = true**：使用表格中的位置，会覆盖GameObject的当前位置
- **Use Table Position = false**：保持GameObject的当前位置，不使用表格位置

**注意：** 如果表格中找不到对应的Config ID，会自动回退到使用Inspector中的值，并显示警告。

---

## ⚠️ 注意事项

### 1. 表格格式要求

- CSV文件使用Tab分隔符（不是逗号）
- 第一行：字段名
- 第二行：字段类型
- 第三行开始：数据行
- ID必须唯一，不能重复

### 2. 修改配置后

每次修改CSV文件后，需要：
1. 重新运行 **Tools → 表 → 一键处理**
2. 重新运行游戏（或重新加载场景）

### 3. 场景名称

传送门的 `TargetSceneName` 必须：
- 与Build Settings中的场景名称完全一致（区分大小写）
- 场景必须在Build Settings中

### 4. 交互按键

`InteractionKey` 字段必须是Unity KeyCode枚举的有效值，例如：
- ✅ "E", "F", "Space", "Enter", "Return"
- ❌ "e", "f", "空格"（不支持小写或中文）

---

## 🐛 常见问题

### Q1: 修改表格后配置没有生效？

**A:** 需要重新运行 **Tools → 表 → 一键处理** 来生成新的代码。

### Q2: 表格中找不到配置ID？

**A:** 
- 检查Config ID是否正确
- 检查CSV文件格式是否正确
- 查看Console是否有错误信息
- 如果找不到，会自动使用Inspector中的值

### Q3: 传送门无法传送？

**A:**
- 检查TargetSceneName是否正确（区分大小写）
- 检查目标场景是否在Build Settings中
- 查看Console错误信息

### Q4: 交互按键无效？

**A:**
- 检查InteractionKey字段值是否为有效的KeyCode名称
- 支持的按键：E, F, Space, Enter, Return等
- 不支持小写或中文

### Q5: 位置没有按表格设置？

**A:**
- 检查 **Use Table Position** 是否勾选
- 如果未勾选，会保持GameObject的当前位置
- 位置会在Start时设置，如果需要在运行时调整，可以手动调用LoadConfigFromTable()

---

## 📁 相关文件

- **NPC配置表格**: `Resources/Table/NPCConfig.csv`
- **传送门配置表格**: `Resources/Table/PortalConfig.csv`
- **NPC控制器**: `Scripts/RPG/Town/NPCController.cs`
- **传送门控制器**: `Scripts/RPG/Town/PortalController.cs`
- **表格生成代码**: `Scripts/Table/Gen/`（自动生成）

---

## 🎯 最佳实践

1. **统一使用表格配置**：所有NPC和传送门都通过表格管理
2. **ID规划**：建议使用有意义的ID范围，如：
   - NPC: 1-999
   - 传送门: 1000-1999
3. **位置管理**：
   - 使用表格位置可以统一管理布局，方便调整
   - 如果某个NPC/传送门需要特殊位置，可以设置 `Use Table Position = false`
   - 建议在表格中记录所有位置，方便后续调整
4. **版本控制**：CSV文件可以加入版本控制，方便团队协作
5. **备份Inspector配置**：即使使用表格，也建议在Inspector中保留默认值作为备份

---

## 📝 更新日志

- **2024-01-XX**: 初始版本，支持NPC和传送门表格配置
- **2024-01-XX**: 添加位置配置支持（PositionX/Y/Z, RotationY），支持从表格统一管理NPC和传送门位置

