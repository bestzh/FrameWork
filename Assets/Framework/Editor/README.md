# Editor工具说明

本目录包含UI框架相关的Editor工具，用于在Unity编辑器中辅助开发。

## 📁 目录结构

```
Framework/Editor/
├── FileUtilTool.cs          # 文件工具类（Editor专用）
└── Tools/
    └── UI/
        ├── UIGenerator.cs              # UI代码生成器核心
        ├── UIGeneratorWindow.cs        # UI代码生成器窗口
        ├── UIGeneratorTemplate.cs      # UI代码生成模板
        ├── AddressableBatchTool.cs     # Addressable批量标记工具
        └── *.md                        # 工具文档
```

## 🛠️ 工具列表

### 1. UI代码生成器

自动扫描UI预制体，识别UI元素（Button、Text、Image等），并生成对应的UI类代码。

**使用方法：**
1. 菜单栏：`Tools > UI > UI代码生成器` - 打开窗口
2. 菜单栏：`Tools > UI > 生成UI代码（选择预制体）` - 快速生成
3. 菜单栏：`Tools > UI > 批量生成UI代码` - 批量生成

**功能：**
- 自动识别UI元素（Button、Text、Image、Toggle等）
- 生成字段声明
- 生成事件绑定代码
- 生成Show/Hide方法
- 支持UIQuery自动查找
- 支持数据绑定代码生成

**详细文档：** [UIGENERATOR_README.md](Tools/UI/UIGENERATOR_README.md)

### 2. Addressable批量标记工具

批量将资源标记为Addressable，支持批量操作和自动分组。

**使用方法：**
- 菜单栏：`Tools > UI > Addressable批量标记工具`

**功能：**
- 扫描指定目录下的资源
- 批量标记为Addressable
- 自动生成地址
- 支持自定义分组
- 支持清除标记

**详细文档：** [ADDRESSABLE_BATCH_TOOL_README.md](Tools/UI/ADDRESSABLE_BATCH_TOOL_README.md)

## 📝 使用说明

### 在新项目中使用

1. **复制工具文件**
   - 将 `Framework/Editor/` 目录复制到新项目的 `Assets/` 目录下

2. **使用工具**
   - 工具会自动出现在Unity菜单栏的 `Tools > UI` 下
   - 无需额外配置

3. **自定义路径**
   - 如果需要修改生成路径，可以编辑工具代码中的路径常量
   - `UIGenerator.cs` 中的 `UI_SCRIPT_PATH` 常量

## ⚙️ 依赖项

- Unity Editor（2019.4或更高版本）
- Addressables包（可选，仅AddressableBatchTool需要）

## 🔧 自定义配置

### 修改UI代码生成路径

编辑 `Framework/Editor/Tools/UI/UIGenerator.cs`：

```csharp
private const string UI_SCRIPT_PATH = "Assets/Scripts/UI/";  // 修改为你的路径
```

### 修改命名空间

编辑 `Framework/Editor/Tools/UI/UIGenerator.cs`：

```csharp
private const string UI_NAMESPACE = "";  // 修改为你的命名空间
```

## 📚 相关文档

- [UI框架README](../README.md)
- [UI代码生成器使用指南](Tools/UI/UIGENERATOR_README.md)
- [Addressable批量标记工具说明](Tools/UI/ADDRESSABLE_BATCH_TOOL_README.md)

