# UI代码生成工具使用示例

## 🎯 使用场景

### 场景1：新建UI类

**步骤：**
1. 在Unity中创建UI预制体（如：`SettingsPanel`）
2. 添加UI元素（Button、Text等）
3. 选择预制体
4. 菜单：`Tools > UI > 生成UI代码（选择预制体）`
5. 代码自动生成到 `Scripts/UI/SettingsPanelUI.cs`

**生成的代码：**
```csharp
public class SettingsPanelUI : UIBase
{
    [Header("UI Elements")]
    public Button BackBtn;
    public Button SaveBtn;
    public Text TitleText;
    
    protected override void Start()
    {
        base.Start();
        BackBtn.onClick.AddListener(OnBackBtnClick);
        SaveBtn.onClick.AddListener(OnSaveBtnClick);
    }
    
    void OnBackBtnClick() { }
    void OnSaveBtnClick() { }
}
```

### 场景2：使用可视化窗口

**步骤：**
1. 菜单：`Tools > UI > UI代码生成器`
2. 在窗口中选择预制体
3. 设置类名和选项
4. 预览UI元素
5. 点击"生成代码"

**优势：**
- 可视化预览
- 可配置选项
- 实时预览生成的元素

### 场景3：批量生成

**步骤：**
1. 将所有UI预制体放在 `Resources/UI/` 目录
2. 菜单：`Tools > UI > 批量生成UI代码`
3. 自动为所有预制体生成代码

**适用场景：**
- 项目初始化
- 大量UI需要生成
- 统一代码风格

## 📝 生成代码示例

### 示例1：简单UI

**预制体结构：**
```
LoginUI
├── UsernameInput (InputField)
├── PasswordInput (InputField)
├── LoginBtn (Button)
└── RegisterBtn (Button)
```

**生成的代码：**
```csharp
public class LoginUIUI : UIBase
{
    [Header("UI Elements")]
    public InputField Username;
    public InputField Password;
    public Button Login;
    public Button Register;
    
    protected override void Start()
    {
        base.Start();
        Login.onClick.AddListener(OnLoginClick);
        Register.onClick.AddListener(OnRegisterClick);
    }
    
    void OnLoginClick()
    {
        Debug.Log("Login Button Clicked");
        // TODO: 添加登录逻辑
    }
    
    void OnRegisterClick()
    {
        Debug.Log("Register Button Clicked");
        // TODO: 添加注册逻辑
    }
}
```

### 示例2：带查询代码

**启用"生成查询代码"选项后：**

```csharp
public class MainMenuUI : UIBase
{
    [Header("UI Elements")]
    public Button StartBtn;
    public Button SettingsBtn;
    
    protected override void Awake()
    {
        base.Awake();
        // 使用UIQuery自动查找UI元素
        StartBtn = UIQuery.Q<Button>(gameObject, "StartBtn") ?? StartBtn;
        SettingsBtn = UIQuery.Q<Button>(gameObject, "SettingsBtn") ?? SettingsBtn;
    }
    
    protected override void Start()
    {
        base.Start();
        if (StartBtn != null)
            StartBtn.onClick.AddListener(OnStartBtnClick);
        if (SettingsBtn != null)
            SettingsBtn.onClick.AddListener(OnSettingsBtnClick);
    }
}
```

### 示例3：复杂UI结构

**预制体结构：**
```
ShopUI
├── Header
│   ├── TitleText (Text)
│   └── CloseBtn (Button)
├── Content
│   └── ItemList (ScrollRect)
└── Footer
    ├── TotalText (Text)
    └── BuyBtn (Button)
```

**生成的代码：**
```csharp
public class ShopUIUI : UIBase
{
    [Header("UI Elements")]
    public Text Title;
    public Button Close;
    public GameObject Content;
    public ScrollRect ItemList;
    public Text Total;
    public Button Buy;
    
    protected override void Start()
    {
        base.Start();
        Close.onClick.AddListener(OnCloseClick);
        Buy.onClick.AddListener(OnBuyClick);
    }
    
    void OnCloseClick() { }
    void OnBuyClick() { }
}
```

## 🔧 自定义和扩展

### 1. 修改生成的代码

生成后可以根据需求修改：

```csharp
// 生成的代码
void OnLoginClick()
{
    Debug.Log("Login Button Clicked");
    // TODO: 添加登录逻辑
}

// 修改后
void OnLoginClick()
{
    string username = Username.text;
    string password = Password.text;
    
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        Debug.LogWarning("用户名或密码不能为空");
        return;
    }
    
    // 调用登录API
    LoginManager.Instance.Login(username, password);
}
```

### 2. 添加数据绑定

在生成的代码基础上添加数据绑定：

```csharp
public class PlayerInfoUI : UIBase
{
    public Text NameText;
    public Text LevelText;
    public Image AvatarImage;
    
    protected override void Start()
    {
        base.Start();
        
        // 添加数据绑定
        TextBinding nameBinding = NameText.gameObject.AddComponent<TextBinding>();
        nameBinding.Bind(PlayerData.Instance.Name);
    }
}
```

### 3. 添加本地化

在生成的代码基础上添加本地化：

```csharp
public class SettingsUI : UIBase
{
    public Text TitleText;
    
    protected override void Start()
    {
        base.Start();
        
        // 添加本地化
        UILocalization localization = TitleText.gameObject.AddComponent<UILocalization>();
        localization.SetKey("ui.settings.title");
    }
}
```

## 💡 技巧和提示

### 1. 命名建议

**好的命名：**
- `Back_Btn` → 生成 `Back`
- `Title_Text` → 生成 `Title`
- `Icon_Image` → 生成 `Icon`

**避免的命名：**
- `Button1`、`Button2` → 生成 `Button1`、`Button2`（不清晰）
- `btn` → 生成 `Btn`（不够描述性）

### 2. 组织结构

**推荐结构：**
```
MainUI
├── Header
│   ├── Title_Text
│   └── Close_Btn
├── Content
│   └── ItemList (ScrollRect)
└── Footer
    └── Confirm_Btn
```

### 3. 代码优化

生成后建议：
1. 添加空值检查
2. 完善事件处理逻辑
3. 添加必要的注释
4. 优化代码结构

## 🎯 工作流程

### 推荐工作流程

1. **设计UI** → 在Unity中创建UI预制体
2. **命名规范** → 使用规范的命名（`Back_Btn`、`Title_Text`）
3. **生成代码** → 使用工具生成基础代码
4. **完善逻辑** → 添加业务逻辑
5. **测试验证** → 测试UI功能

### 迭代开发

1. 修改UI预制体
2. 重新生成代码（会覆盖，注意备份）
3. 或手动更新代码

## 📊 效率提升

### 手动编写 vs 代码生成

**手动编写（10分钟）：**
- 声明字段：2分钟
- 编写Start方法：3分钟
- 编写事件处理：5分钟

**代码生成（1分钟）：**
- 选择预制体：10秒
- 生成代码：10秒
- 完善逻辑：40秒

**效率提升：10倍！**

## ⚠️ 注意事项

1. **备份代码**：生成前备份现有代码
2. **命名规范**：使用规范的命名便于生成
3. **手动调整**：生成后需要根据需求调整
4. **版本控制**：生成的代码建议提交到版本控制

## 🔍 常见问题

### Q: 生成的字段名不合适怎么办？
A: 生成后可以手动修改字段名，或重命名UI元素后重新生成。

### Q: 如何避免覆盖现有代码？
A: 生成前先备份，或使用版本控制。

### Q: 可以自定义生成模板吗？
A: 可以修改 `UIGenerator.cs` 中的代码生成逻辑。

### Q: 支持哪些UI组件？
A: 支持Unity标准UI组件，详见文档。

