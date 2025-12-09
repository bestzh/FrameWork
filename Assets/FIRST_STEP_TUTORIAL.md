# 第一步详细教程：创建城镇场景

## 🎯 目标
创建一个简单的城镇场景，玩家可以在其中移动。

## ⏱️ 预计时间
30-60分钟

---

## 📋 步骤1：创建新场景

### 1.1 在Unity中创建场景
```
1. 打开Unity编辑器
2. File → New Scene
3. 选择 "Basic (Built-in)" 或 "URP"（根据你的项目设置）
4. File → Save Scene As...
5. 保存到：Assets/Scenes/Town.unity
```

### 1.2 清理场景
```
1. 删除默认的Main Camera（如果有）
2. 保留Directional Light（或根据需要调整）
```

---

## 📋 步骤2：添加地面

### 2.1 创建地面
```
1. GameObject → 3D Object → Plane
2. 重命名为 "Ground"
3. 设置位置：(0, 0, 0)
4. 设置缩放：(10, 1, 10) - 让地面更大
```

### 2.2 添加材质（可选）
```
1. 创建材质：Assets/Materials/Ground.mat
2. 设置颜色或纹理
3. 拖拽到Ground上
```

---

## 📋 步骤3：添加玩家

### 3.1 创建玩家对象
```
1. GameObject → Create Empty
2. 重命名为 "Player"
3. 设置位置：(0, 1, 0) - 稍微高于地面
```

### 3.2 添加玩家模型（临时）
```
方法1：使用Cube（最简单）
1. GameObject → 3D Object → Cube
2. 作为Player的子对象
3. 重命名为 "PlayerModel"
4. 设置位置：(0, 0.5, 0)

方法2：使用已有模型（如果有）
1. 导入你的玩家模型
2. 作为Player的子对象
```

### 3.3 添加PlayerController组件
```
1. 选择Player对象
2. Add Component → PlayerController（你的已有脚本）
3. 检查组件设置：
   - MoveSpeed: 5
   - MaxHealth: 100
   - 其他设置根据需要调整
```

### 3.4 添加Rigidbody（如果需要）
```
1. Add Component → Rigidbody
2. 设置：
   - Freeze Rotation X, Y, Z（防止旋转）
   - Drag: 5（增加阻力，让移动更平滑）
```

### 3.5 设置Tag和Layer
```
1. Tag设置为 "Player"
2. Layer设置为 "Default"（或创建"Player"层）
```

---

## 📋 步骤4：添加相机

### 4.1 创建相机
```
1. GameObject → Camera
2. 重命名为 "MainCamera"
3. 设置位置：(0, 5, -10)
4. 设置旋转：(15, 0, 0) - 稍微向下看
```

### 4.2 相机跟随（可选，简单版）
```
创建脚本：Scripts/RPG/Town/SimpleCameraFollow.cs

using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target; // 玩家
    public Vector3 offset = new Vector3(0, 5, -10);
    
    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}

添加到MainCamera，设置target为Player
```

---

## 📋 步骤5：添加光照

### 5.1 调整方向光
```
1. 选择Directional Light
2. 设置：
   - Intensity: 1
   - Color: 白色或暖色
   - Rotation: (50, -30, 0) - 模拟太阳角度
```

### 5.2 添加环境光（可选）
```
1. Window → Rendering → Lighting
2. Environment Lighting → Sky Color
3. 设置Ambient Intensity: 0.5
```

---

## 📋 步骤6：测试

### 6.1 运行场景
```
1. 点击Play按钮
2. 使用WASD移动
3. 使用鼠标旋转（如果PlayerController支持）
```

### 6.2 检查问题
```
如果移动不工作：
- 检查PlayerController是否正确添加
- 检查Input设置（Edit → Project Settings → Input Manager）
- 检查Console是否有错误

如果相机不跟随：
- 检查SimpleCameraFollow脚本是否正确添加
- 检查target是否设置
```

---

## 📋 步骤7：添加基础装饰（可选）

### 7.1 添加一些Cube作为建筑
```
1. 创建几个Cube
2. 设置不同的大小和位置
3. 重命名为 "Building1", "Building2" 等
4. 添加不同颜色材质
```

### 7.2 添加NPC占位符
```
1. 创建Sphere或Capsule
2. 重命名为 "NPC_Test"
3. 设置位置：(5, 1, 0)
4. 添加不同颜色材质（比如红色）
5. 后续会添加NPCController
```

---

## ✅ 完成检查清单

- [ ] 场景已创建并保存
- [ ] 地面已添加
- [ ] 玩家已添加并可以移动
- [ ] 相机已设置（跟随或固定）
- [ ] 光照已调整
- [ ] 场景可以正常运行
- [ ] 玩家可以移动

---

## 🎯 下一步

完成这一步后，下一步是：
1. 添加NPC系统（基础交互）
2. 添加传送门（场景切换）

---

## 💡 提示

### 如果遇到问题：

**问题：玩家移动不工作**
- 检查PlayerController组件是否正确添加
- 检查Input Manager设置
- 查看Console是否有错误信息

**问题：相机不跟随**
- 检查SimpleCameraFollow脚本
- 检查target是否设置正确

**问题：场景太暗**
- 调整Directional Light的Intensity
- 增加Ambient Light

---

## 📝 代码模板

### SimpleCameraFollow.cs（完整版）
```csharp
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // 相机始终看向玩家
        transform.LookAt(target);
    }
}
```

---

## 🚀 开始吧！

**现在就做：**
1. 打开Unity
2. 按照步骤1-6创建场景
3. 测试玩家移动
4. 完成检查清单

**完成后告诉我，我们继续下一步！** 🎮

