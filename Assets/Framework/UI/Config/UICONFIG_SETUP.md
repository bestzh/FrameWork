# UI配置文件设置指南

## 📖 概述

UI配置系统允许你通过JSON文件统一管理UI的属性，包括路径、层级、动画时长等。

**重要：配置文件是可选的！** 只有需要特殊设置的UI才需要配置，普通UI可以直接使用代码指定路径。

---

## 🚀 快速设置

### 方法1：使用默认配置文件（推荐）

框架已经创建了默认配置文件：
- **位置**: `Resources/Config/UIConfig.json`
- **自动加载**: 启动时自动加载

如果文件不存在，框架会静默处理（不会报错）。

### 方法2：创建自定义配置文件

1. 在Unity中创建文件夹：`Assets/Resources/Config/`
2. 创建JSON文件：`UIConfig.json`
3. 复制示例配置内容（参考 `UIConfig.example.json`）

---

## 📝 配置文件格式

```json
{
  "configs": [
    {
      "uiName": "RemindersUI",
      "uiPath": "UI/RemindersUI",
      "layer": 2,
      "isModal": false,
      "cacheOnLoad": true,
      "preload": false,
      "poolSize": 0,
      "showAnimationDuration": 0.3,
      "hideAnimationDuration": 0.2,
      "customProperties": []
    }
  ]
}
```

### 配置字段说明

| 字段 | 类型 | 说明 | 默认值 |
|------|------|------|--------|
| `uiName` | string | UI类名（必须） | - |
| `uiPath` | string | UI预制体路径 | - |
| `layer` | int | UI层级（0-5） | 1 |
| `isModal` | bool | 是否为模态UI | false |
| `cacheOnLoad` | bool | 加载时是否缓存 | true |
| `preload` | bool | 是否预加载 | false |
| `poolSize` | int | 对象池大小（0=不使用） | 0 |
| `showAnimationDuration` | float | 显示动画时长 | 0.3 |
| `hideAnimationDuration` | float | 隐藏动画时长 | 0.2 |
| `customProperties` | array | 自定义属性 | [] |

---

## 🎯 使用配置

### 1. 根据配置加载UI

```csharp
// 自动使用配置文件中的路径和设置
var ui = UIManager.Instance.LoadUIFromConfig<RemindersUI>(true);
```

### 2. 获取配置

```csharp
// 获取UI配置
UIConfig config = UIConfigManager.Instance.GetConfig<RemindersUI>();
if (config != null)
{
    Debug.Log($"UI路径: {config.uiPath}");
    Debug.Log($"UI层级: {config.layer}");
}
```

### 3. 运行时修改配置

```csharp
// 添加或更新配置
UIConfig config = new UIConfig
{
    uiName = "RemindersUI",
    uiPath = "UI/RemindersUI",
    layer = UIHierarchyManager.UILayer.Popup
};
UIConfigManager.Instance.SetConfig(config);
```

---

## ⚙️ UIConfigManager设置

在Inspector中可以配置：

- **Config Path**: 配置文件路径（相对于Resources文件夹）
  - 默认: `Config/UIConfig`
  - 实际文件: `Resources/Config/UIConfig.json`

- **Load On Start**: 是否在Start时自动加载
  - 默认: true

- **Show Warning If Not Found**: 文件不存在时是否显示警告
  - 默认: false（静默处理）

---

## 🔧 常见问题

### Q: 所有UI都需要在配置文件中写入吗？

**A:** **不需要！** 配置文件是可选的。只有需要特殊设置的UI才需要配置：
- ✅ 需要特殊层级（Dialog、Top等）
- ✅ 需要特殊动画时长
- ✅ 需要使用对象池
- ✅ 需要预加载

普通UI可以直接使用代码指定路径：
```csharp
var ui = UIManager.Instance.LoadUI<MyUI>("UI/MyUI", true);
```

### Q: 配置文件未找到警告？

**A:** 这是正常的，配置文件是可选的。如果不需要配置，可以：
1. 设置 `Show Warning If Not Found` 为 `false`（默认）
2. 或者不创建配置文件

### Q: 如何禁用配置系统？

**A:** 设置 `Load On Start` 为 `false`，或者不创建配置文件。

### Q: 配置文件路径在哪里？

**A:** 配置文件必须放在 `Resources` 文件夹下，路径是相对于 `Resources` 的。

例如：
- 配置路径: `Config/UIConfig`
- 实际文件: `Assets/Resources/Config/UIConfig.json`

### Q: 如何添加新的UI配置？

**A:** 编辑 `Resources/Config/UIConfig.json`，添加新的配置项：

```json
{
  "uiName": "NewUI",
  "uiPath": "UI/NewUI",
  "layer": 2,
  ...
}
```

### Q: 可以混合使用吗？

**A:** **可以！** 可以部分UI用配置，部分UI不用配置：

```csharp
// 使用配置
var mainUI = UIManager.Instance.LoadUIFromConfig<MainUI>(true);

// 不使用配置，直接指定路径
var otherUI = UIManager.Instance.LoadUI<OtherUI>("UI/OtherUI", true);
```

---

## 📊 配置优先级

1. **代码中指定**: 最高优先级
   ```csharp
   UIManager.Instance.LoadUI<RemindersUI>("UI/CustomPath", true);
   ```

2. **配置文件**: 中等优先级
   ```csharp
   UIManager.Instance.LoadUIFromConfig<RemindersUI>(true);
   ```

3. **默认值**: 最低优先级
   - 使用默认路径和设置

---

## 🎉 总结

- ✅ 配置文件是可选的
- ✅ 文件不存在时不会报错（默认）
- ✅ 可以在Inspector中配置路径
- ✅ 支持运行时修改配置

配置文件让UI管理更加灵活和统一！

