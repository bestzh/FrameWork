# UI代码生成工具使用指南

## 📋 功能概述

UI代码生成工具可以自动扫描UI预制体，识别UI元素（Button、Text、Image等），并生成对应的UI类代码，大大减少手动编写的工作量。

## 🚀 快速开始

### 方式1：使用窗口（推荐）

1. 在Unity菜单栏选择：`Tools > UI > UI代码生成器`
2. 在打开的窗口中选择UI预制体
3. 设置类名和生成选项
4. 点击"生成代码"按钮

### 方式2：使用菜单

1. 在Project窗口选择UI预制体
2. 在Unity菜单栏选择：`Tools > UI > 生成UI代码（选择预制体）`
3. 代码会自动生成到 `Scripts/UI/` 目录

### 方式3：批量生成

1. 在Unity菜单栏选择：`Tools > UI > 批量生成UI代码`
2. 工具会自动扫描 `Resources/UI/` 目录下的所有预制体
3. 为每个预制体生成对应的UI代码

## 📝 生成内容

### 1. 字段声明

自动识别UI元素并生成字段：

```csharp
[Header("UI Elements")]
public Button BackBtn;
public Button AddReminderBtn;
public Text TitleText;
public Image IconImage;
public GameObject Reminders;
```

### 2. Start方法

自动生成事件绑定代码：

```csharp
protected override void Start()
{
    base.Start();
    
    BackBtn.onClick.AddListener(OnBackBtnClick);
    AddReminderBtn.onClick.AddListener(OnAddReminderBtnClick);
}
```

### 3. 事件处理方法

自动生成按钮点击处理方法：

```csharp
void OnBackBtnClick()
{
    Debug.Log("BackBtn Button Clicked");
    // TODO: 添加按钮点击逻辑
}

void OnAddReminderBtnClick()
{
    Debug.Log("AddReminderBtn Button Clicked");
    // TODO: 添加按钮点击逻辑
}
```

### 4. Show/Hide方法

生成标准的Show和Hide方法：

```csharp
public override void Show()
{
    base.Show();
    // TODO: 添加显示时的逻辑
}

public override void Hide()
{
    base.Hide();
    // TODO: 添加隐藏时的逻辑
}
```

## ⚙️ 生成选项

### 1. 生成事件处理方法

勾选后会自动生成按钮点击事件处理方法。

### 2. 生成数据绑定代码

勾选后会生成数据绑定相关的注释和代码结构。

### 3. 生成查询代码（UIQuery）

勾选后会使用UIQuery自动查找UI元素：

```csharp
protected override void Awake()
{
    base.Awake();
    // 使用UIQuery自动查找UI元素
    BackBtn = UIQuery.Q<Button>(gameObject, "BackBtn") ?? BackBtn;
    TitleText = UIQuery.Q<Text>(gameObject, "TitleText") ?? TitleText;
}
```

## 🎯 支持的UI组件

工具会自动识别以下UI组件：

- ✅ Button（按钮）
- ✅ Text（文本）
- ✅ Image（图片）
- ✅ Toggle（开关）
- ✅ Slider（滑块）
- ✅ InputField（输入框）
- ✅ ScrollRect（滚动视图）
- ✅ Scrollbar（滚动条）
- ✅ Dropdown（下拉菜单）
- ✅ GameObject（容器对象）

## 📋 命名规则

### 字段名转换规则

工具会自动将GameObject名称转换为合适的字段名：

- `Back_Btn` → `Back`
- `Title_Text` → `Title`
- `Icon_Image` → `Icon`
- `Settings_Toggle` → `Settings`

### 类名规则

- 如果预制体名称以"UI"结尾，直接使用
- 否则自动添加"UI"后缀
- 例如：`MainMenu` → `MainMenuUI`

## 🔧 高级功能

### 1. 自定义字段名

如果自动生成的字段名不合适，可以在生成后手动修改。

### 2. 添加自定义代码

生成后可以在TODO标记处添加自定义逻辑。

### 3. 批量处理

使用批量生成功能可以一次性为多个UI生成代码。

## ⚠️ 注意事项

### 1. 预制体要求

- 预制体必须包含UI元素（Button、Text等）
- 建议使用规范的命名（如：`Back_Btn`、`Title_Text`）

### 2. 代码覆盖

- 如果目标文件已存在，会被覆盖
- 建议先备份现有代码

### 3. 命名冲突

- 如果多个UI元素生成相同的字段名，只会保留第一个
- 建议使用唯一的命名

### 4. 手动调整

- 生成的代码是模板代码，需要根据实际需求调整
- TODO标记处需要添加具体逻辑

## 📊 生成示例

### 输入：UI预制体

```
MainMenu (GameObject)
├── BackBtn (Button)
├── TitleText (Text)
├── IconImage (Image)
└── Content (GameObject)
    └── ItemList (ScrollRect)
```

### 输出：生成的代码

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class MainMenuUI : UIBase
{
    [Header("UI Elements")]
    public Button Back;
    public Text Title;
    public Image Icon;
    public GameObject Content;
    public ScrollRect ItemList;
    
    protected override void Start()
    {
        base.Start();
        
        if (Back != null)
            Back.onClick.AddListener(OnBackClick);
    }
    
    void OnBackClick()
    {
        Debug.Log("Back Button Clicked");
        // TODO: 添加按钮点击逻辑
    }
    
    public override void Show()
    {
        base.Show();
        // TODO: 添加显示时的逻辑
    }
    
    public override void Hide()
    {
        base.Hide();
        // TODO: 添加隐藏时的逻辑
    }
}
```

## 🎯 最佳实践

### 1. 命名规范

- UI元素使用下划线命名：`Back_Btn`、`Title_Text`
- 容器使用描述性名称：`Content`、`ItemList`

### 2. 组织结构

- 相关UI元素放在同一父对象下
- 使用Header分组管理

### 3. 代码维护

- 生成后立即测试
- 添加必要的空值检查
- 完善TODO标记的逻辑

## 🔍 故障排除

### 问题1：生成的字段名为空

**原因**：UI元素名称不符合命名规则

**解决**：重命名UI元素，使用下划线命名（如：`Back_Btn`）

### 问题2：找不到UI元素

**原因**：UI元素被跳过（如遮罩、背景等）

**解决**：检查UI元素名称，避免使用`mask`、`background`等关键词

### 问题3：代码生成失败

**原因**：文件路径不存在或权限问题

**解决**：检查`Scripts/UI/`目录是否存在，确保有写入权限

## 📖 相关文档

- [UIM Framework README](../UIManager/README.md)
- [使用示例](../UIManager/USAGE_EXAMPLES.md)

