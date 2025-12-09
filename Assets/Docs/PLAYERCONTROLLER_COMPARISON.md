# PlayerController 对比说明

## 📋 两个PlayerController的位置

### 1. `Scripts/RPG/Player/PlayerController.cs` ⭐ **推荐使用这个**
- **位置：** `Assets/Scripts/RPG/Player/PlayerController.cs`
- **来源：** 我们为RPG项目专门移植和设计的
- **功能：** 完整的RPG玩家控制器

### 2. `Suntail Village/Scripts/PlayerController.cs`
- **位置：** `Assets/Suntail Village/Scripts/PlayerController.cs`
- **来源：** 从RPG-Silent项目复制过来的资源包
- **功能：** 简单的移动控制器（第三方资源）

---

## 🔍 详细对比

### PlayerController #1（推荐）✅
**路径：** `Scripts/RPG/Player/PlayerController.cs`

**功能特性：**
- ✅ **状态机系统** - 完整的玩家状态机（Idle, Move, Attack, Jump, Roll, Hurt, Dead）
- ✅ **技能系统集成** - 集成了SkillSystem
- ✅ **动画系统集成** - 集成了PlayerAnimationController
- ✅ **战斗系统** - 支持攻击、受击、死亡
- ✅ **角色数据关联** - 关联到CharacterData
- ✅ **框架集成** - 使用EventManager、CharacterManager等框架系统
- ✅ **Lua支持** - 有[XLua.LuaCallCSharp]标记，支持热更新

**适用场景：**
- ✅ RPG游戏
- ✅ 需要战斗系统
- ✅ 需要技能系统
- ✅ 需要状态机

**代码特点：**
```csharp
public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine;  // 状态机
    public SkillSystem SkillSystem;          // 技能系统
    public CharacterData CharacterData;       // 角色数据
    // ... 完整的RPG功能
}
```

---

### PlayerController #2（不推荐用于RPG）
**路径：** `Suntail Village/Scripts/PlayerController.cs`

**功能特性：**
- ✅ **基础移动** - 简单的移动控制
- ✅ **脚步声系统** - 根据地面纹理播放脚步声
- ✅ **相机控制** - 鼠标视角控制
- ❌ **无状态机** - 没有状态机系统
- ❌ **无战斗系统** - 没有战斗功能
- ❌ **无技能系统** - 没有技能功能
- ❌ **无框架集成** - 没有集成到我们的框架

**适用场景：**
- ✅ 简单的第三人称移动
- ✅ 需要脚步声效果
- ❌ 不适合RPG游戏

**代码特点：**
```csharp
namespace Suntail
{
    public class PlayerController : MonoBehaviour
    {
        // 简单的移动控制
        // 脚步声系统
        // 没有战斗、技能等功能
    }
}
```

---

## ✅ 推荐使用

### 使用：`Scripts/RPG/Player/PlayerController.cs`

**原因：**
1. ✅ **专为RPG设计** - 包含所有RPG需要的功能
2. ✅ **框架集成** - 已经集成到我们的框架系统
3. ✅ **功能完整** - 状态机、技能、战斗、动画都有
4. ✅ **可扩展** - 易于扩展新功能
5. ✅ **Lua支持** - 支持热更新

---

## 🚫 如何处理另一个PlayerController

### 选项1：保留但不使用（推荐）
- 保留 `Suntail Village/Scripts/PlayerController.cs`
- 它不会影响你的项目（因为命名空间不同）
- 如果以后需要简单的移动控制器可以参考

### 选项2：删除
- 如果确定不需要，可以删除
- 但建议先保留，以防以后需要参考

---

## 📝 使用步骤

### 在Unity中使用推荐的PlayerController：

1. **创建玩家对象**
   ```
   GameObject → Create Empty
   重命名为 "Player"
   ```

2. **添加PlayerController组件**
   ```
   Add Component → Scripts → RPG → Player → PlayerController
   ```

3. **配置组件**
   - MoveSpeed: 5
   - MaxHealth: 100
   - 其他设置根据需要调整

4. **添加必要的组件**
   - Rigidbody（如果需要物理）
   - Animator（如果需要动画）
   - CharacterController（可选，如果使用）

5. **设置Tag**
   ```
   Tag设置为 "Player"
   ```

---

## ⚠️ 注意事项

### 命名空间冲突
- `Suntail Village` 的PlayerController在 `Suntail` 命名空间下
- 我们的PlayerController没有命名空间
- 所以不会有冲突

### 如果Unity显示两个PlayerController
- 选择 `Scripts/RPG/Player/PlayerController`（没有命名空间的）
- 不要选择 `Suntail.PlayerController`

---

## 🎯 总结

**使用：** `Scripts/RPG/Player/PlayerController.cs` ✅

**原因：**
- 专为RPG设计
- 功能完整
- 框架集成
- 支持热更新

**另一个：** `Suntail Village/Scripts/PlayerController.cs`
- 可以保留作为参考
- 但不用于RPG游戏

---

现在你知道应该使用哪个了！继续创建城镇场景吧！🚀

