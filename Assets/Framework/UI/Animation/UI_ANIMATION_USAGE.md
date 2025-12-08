# UI动画系统使用指南

## 🎉 概述

UI动画系统提供了丰富的动画效果，支持多种动画类型，并集成了DOTween（如果可用）。你可以轻松地为UI添加专业的加载和关闭动画效果。

---

## ✨ 支持的动画类型

### 显示动画类型

| 动画类型 | 效果描述 | 适用场景 |
|---------|---------|---------|
| **None** | 无动画 | 需要立即显示 |
| **Fade** | 淡入 | 通用，优雅 |
| **Scale** | 缩放 | 弹窗、对话框 |
| **SlideFromLeft** | 从左侧滑入 | 侧边栏、菜单 |
| **SlideFromRight** | 从右侧滑入 | 侧边栏、菜单 |
| **SlideFromTop** | 从上方滑入 | 通知、提示 |
| **SlideFromBottom** | 从下方滑入 | 底部弹窗 |
| **Bounce** | 弹跳 | 游戏UI、奖励 |
| **Elastic** | 弹性 | 特殊效果 |
| **Back** | 回弹 | 强调效果 |

### 隐藏动画类型

支持与显示动画相同的类型，但效果相反。

---

## 🚀 快速开始

### 1. 在Inspector中配置动画

1. 选择你的UI预制体（继承自`UIBase`）
2. 在Inspector中找到 **Animation Settings** 部分
3. 勾选 **Use Advanced Animation**
4. 配置 **Animation Config**：
   - **Show Animation Type**: 选择显示动画类型
   - **Show Duration**: 显示动画时长（秒）
   - **Hide Animation Type**: 选择隐藏动画类型
   - **Hide Duration**: 隐藏动画时长（秒）
   - **Scale From**: 缩放起始值（0-1）
   - **Slide Distance**: 滑动距离（像素）

### 2. 代码中使用

```csharp
public class MyUI : UIBase
{
    public override void Show()
    {
        base.Show(); // 会自动使用配置的动画
    }
    
    public override void Hide()
    {
        base.Hide(); // 会自动使用配置的动画
    }
}
```

---

## 📝 详细使用示例

### 示例1：弹窗使用缩放动画

```csharp
public class PopupUI : UIBase
{
    // 在Inspector中配置：
    // Show Animation Type: Scale
    // Hide Animation Type: Scale
    // Scale From: 0.8
    
    public void ShowPopup()
    {
        Show(); // 会播放缩放动画
    }
    
    public void ClosePopup()
    {
        Hide(); // 会播放缩放隐藏动画
    }
}
```

### 示例2：侧边栏使用滑动动画

```csharp
public class SidebarUI : UIBase
{
    // 在Inspector中配置：
    // Show Animation Type: SlideFromLeft
    // Hide Animation Type: SlideFromLeft
    // Slide Distance: 500
    
    public void ToggleSidebar()
    {
        if (IsVisible)
        {
            Hide(); // 滑出到左侧
        }
        else
        {
            Show(); // 从左侧滑入
        }
    }
}
```

### 示例3：通知使用弹跳动画

```csharp
public class NotificationUI : UIBase
{
    // 在Inspector中配置：
    // Show Animation Type: Bounce
    // Hide Animation Type: Fade
    // Show Duration: 0.5
    
    public void ShowNotification(string message)
    {
        Show(); // 弹跳显示
    }
}
```

### 示例4：底部弹窗

```csharp
public class BottomPanelUI : UIBase
{
    // 在Inspector中配置：
    // Show Animation Type: SlideFromBottom
    // Hide Animation Type: SlideFromBottom
    // Slide Distance: 800
    
    public void ShowPanel()
    {
        Show(); // 从底部滑入
    }
}
```

---

## ⚙️ 动画配置详解

### UIAnimationConfig 参数

```csharp
[Serializable]
public class UIAnimationConfig
{
    // 显示动画
    public UIAnimationType showAnimationType = UIAnimationType.Fade;
    public float showDuration = 0.3f;        // 动画时长（秒）
    public float showDelay = 0f;              // 延迟时间（秒）
    
    // 隐藏动画
    public UIAnimationType hideAnimationType = UIAnimationType.Fade;
    public float hideDuration = 0.2f;         // 动画时长（秒）
    public float hideDelay = 0f;              // 延迟时间（秒）
    
    // 动画参数
    public float scaleFrom = 0.8f;            // 缩放起始值（0-1）
    public float slideDistance = 500f;         // 滑动距离（像素）
    public float rotateAngle = 360f;          // 旋转角度（度）
}
```

### 参数说明

- **showDuration / hideDuration**: 动画持续时间，建议0.2-0.5秒
- **showDelay / hideDelay**: 动画延迟时间，可用于序列动画
- **scaleFrom**: 缩放动画的起始缩放值，0.8表示从80%缩放到100%
- **slideDistance**: 滑动动画的移动距离，单位是像素

---

## 🎨 动画效果预览

### Fade（淡入淡出）
- **效果**: 透明度从0到1（或相反）
- **适用**: 通用场景
- **时长**: 0.2-0.3秒

### Scale（缩放）
- **效果**: 从指定缩放值缩放到1（或相反）
- **适用**: 弹窗、对话框
- **时长**: 0.3-0.5秒
- **参数**: scaleFrom = 0.8

### Slide（滑动）
- **效果**: 从指定方向滑入/滑出
- **适用**: 侧边栏、底部弹窗
- **时长**: 0.3-0.4秒
- **参数**: slideDistance = 500

### Bounce（弹跳）
- **效果**: 弹跳式出现
- **适用**: 游戏UI、奖励提示
- **时长**: 0.5-0.8秒

### Elastic（弹性）
- **效果**: 弹性效果
- **适用**: 特殊强调
- **时长**: 0.6-1.0秒

### Back（回弹）
- **效果**: 回弹效果
- **适用**: 强调效果
- **时长**: 0.4-0.6秒

---

## 🔧 高级用法

### 1. 运行时切换动画类型

```csharp
public class DynamicUI : UIBase
{
    public void SetAnimationType(UIAnimationType showType, UIAnimationType hideType)
    {
        animationConfig.showAnimationType = showType;
        animationConfig.hideAnimationType = hideType;
    }
    
    public void ShowWithBounce()
    {
        animationConfig.showAnimationType = UIAnimationType.Bounce;
        Show();
    }
}
```

### 2. 自定义动画参数

```csharp
public class CustomAnimationUI : UIBase
{
    void Start()
    {
        // 自定义动画参数
        animationConfig.scaleFrom = 0.5f;      // 从50%缩放
        animationConfig.slideDistance = 1000f;  // 滑动1000像素
        animationConfig.showDuration = 0.5f;    // 0.5秒动画
    }
}
```

### 3. 动画回调

```csharp
public class AnimationCallbackUI : UIBase
{
    public override void Show()
    {
        base.Show();
        // 动画会在UIAnimationManager中自动播放
        // 如果需要回调，可以重写OnShow
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        Debug.Log("动画开始播放");
    }
}
```

---

## ⚠️ 注意事项

### 1. DOTween依赖

- 如果项目中有DOTween，会自动使用DOTween播放动画（性能更好）
- 如果没有DOTween，会使用协程播放简单动画（仅支持Fade）

### 2. CanvasGroup要求

- 大部分动画需要`CanvasGroup`组件
- 如果没有`CanvasGroup`，某些动画可能无法正常工作
- 框架会自动添加`CanvasGroup`（如果不存在）

### 3. 向后兼容

- 默认使用传统动画（向后兼容）
- 需要勾选 **Use Advanced Animation** 才能使用新动画系统
- 传统动画配置仍然有效

### 4. 性能考虑

- DOTween动画性能更好
- 建议动画时长不超过1秒
- 避免同时播放过多动画

---

## 🎯 最佳实践

### 1. 选择合适的动画类型

- **弹窗**: Scale 或 Bounce
- **侧边栏**: SlideFromLeft/Right
- **底部面板**: SlideFromBottom
- **通知**: SlideFromTop 或 Bounce
- **通用**: Fade

### 2. 动画时长建议

- **快速操作**: 0.2秒
- **正常操作**: 0.3秒
- **强调效果**: 0.5秒
- **特殊效果**: 0.6-1.0秒

### 3. 保持一致性

- 同一类型的UI使用相同的动画
- 显示和隐藏动画可以不同
- 保持动画风格统一

---

## 📊 动画对比

| 动画类型 | 性能 | 视觉效果 | 适用场景 |
|---------|------|---------|---------|
| Fade | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | 通用 |
| Scale | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | 弹窗 |
| Slide | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | 侧边栏 |
| Bounce | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 游戏UI |
| Elastic | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | 特殊效果 |

---

## 🚀 快速参考

```csharp
// 1. 在Inspector中配置动画
// - 勾选 Use Advanced Animation
// - 选择动画类型
// - 设置动画参数

// 2. 代码中使用
ui.Show();  // 自动播放配置的显示动画
ui.Hide();  // 自动播放配置的隐藏动画

// 3. 运行时修改
ui.animationConfig.showAnimationType = UIAnimationType.Bounce;
ui.Show();
```

---

## 💡 常见问题

### Q: 动画不播放？
A: 检查是否勾选了 **Use Advanced Animation**，并确保有`CanvasGroup`组件。

### Q: 如何禁用动画？
A: 设置 `showAnimationType` 和 `hideAnimationType` 为 `None`。

### Q: DOTween是必需的吗？
A: 不是，但使用DOTween会有更好的性能和更多动画效果。

### Q: 可以同时使用多个动画吗？
A: 目前不支持，但可以通过动画链实现（未来功能）。

---

## 🎉 总结

UI动画系统提供了：
- ✅ 10+种动画类型
- ✅ DOTween集成（可选）
- ✅ Inspector可视化配置
- ✅ 向后兼容
- ✅ 性能优化

开始使用动画系统，让你的UI更加生动和专业！

